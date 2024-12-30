using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Badges;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.UI.Map;
using Varguiniano.YAPU.Runtime.World.OutOfBattleWeathers;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for a move that sets a weather effect.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/General/SetWeatherMove", fileName = "SetWeatherMove")]
    public class SetWeatherMove : Move
    {
        /// <summary>
        /// Reference to the weather the move will set.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private Weather WeatherToSet;

        /// <summary>
        /// Countdown for the weather to last.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private int Countdown;

        /// <summary>
        /// Reference to the weathers that are incompatible if set and will make the move fail.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private List<Weather> IncompatibleWeathers;

        /// <summary>
        /// Weather to set out of battle.
        /// </summary>
        [FoldoutGroup("Out of battle")]
        [SerializeField]
        private OutOfBattleWeather OutOfBattleWeatherToSet;

        /// <summary>
        /// No need for animation since the weather will have an entry animation.
        /// </summary>
        public override IEnumerator PlayAnimation(BattleManager battleManager,
                                                  float speed,
                                                  BattlerType userType,
                                                  int userIndex,
                                                  Battler user,
                                                  Transform userPosition,
                                                  List<(BattlerType Type, int Index)> targets,
                                                  List<Transform> targetPositions,
                                                  bool ignoresAbilities)
        {
            yield break;
        }

        /// <summary>
        /// Execute the effect of the move.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="user"></param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="hitNumber"></param>
        /// <param name="expectedHits"></param>
        /// <param name="externalPowerMultiplier"></param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="finishedCallback">Callback stating if the move successfully executed.</param>
        public override IEnumerator ExecuteEffect(BattleManager battleManager,
                                                  ILocalizer localizer,
                                                  BattlerType userType,
                                                  int userIndex,
                                                  Battler user,
                                                  List<(BattlerType Type, int Index)> targets,
                                                  int hitNumber,
                                                  int expectedHits,
                                                  float externalPowerMultiplier,
                                                  bool ignoresAbilities,
                                                  Action<bool> finishedCallback)
        {
            battleManager.Scenario.GetWeather(out Weather weather);

            // Check even if the weather has no effect.
            if (IncompatibleWeathers.Contains(weather))
            {
                yield return DialogManager.ShowDialogAndWait("Battle/Move/Failed",
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);

                finishedCallback.Invoke(false);
                yield break;
            }

            Battler battler = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            int customCountdown = battler.CanUseHeldItemInBattle(battleManager)
                                      ? battler.HeldItem.CalculateWeatherDuration(WeatherToSet, battler, battleManager)
                                      : -2;

            yield return
                battleManager.Scenario.SetWeather(WeatherToSet,
                                                  customCountdown != -2
                                                      ? customCountdown
                                                      : Countdown);

            finishedCallback.Invoke(true);
        }

        /// <summary>
        /// Use the move outside of battle.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="monster">Monster using the move.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="mapSceneLauncher"></param>
        public override IEnumerator UseOutOfBattle(PlayerCharacter playerCharacter,
                                                   MonsterInstance monster,
                                                   ILocalizer localizer,
                                                   MapSceneLauncher mapSceneLauncher)
        {
            if (playerCharacter.Region.IsMoveLockedByBadge(this, out Badge badge))
                if (!playerCharacter.GlobalGameData.HasBadge(badge, playerCharacter.Region))
                {
                    yield return DialogManager.ShowDialogAndWait("Dialogs/Moves/BlockedByBadge",
                                                                 modifiers: new[]
                                                                            {
                                                                                badge.LocalizableName, LocalizableName
                                                                            });

                    yield break;
                }

            if (OutOfBattleWeatherToSet == playerCharacter.CurrentWeather)
            {
                yield return DialogManager.ShowDialogAndWait("Dialogs/CantUseNow");
                yield break;
            }

            if (!playerCharacter.Scene.IsAffectedBySky
             && !playerCharacter.Scene.AllowedWeathers.Contains(OutOfBattleWeatherToSet))
            {
                yield return DialogManager.ShowDialogAndWait("Dialogs/CantUseNow");
                yield break;
            }

            yield return DialogManager.CloseMenus();

            yield return DialogManager.ShowDialogAndWait("Battle/Move/Used",
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        monster.GetNameOrNickName(localizer),
                                                                        localizer[LocalizableName]
                                                                    });

            yield return playerCharacter.UpdateWeather(OutOfBattleWeatherToSet, ignoredCleared: true);

            playerCharacter.GlobalGridManager.StopAllActors = false;
            playerCharacter.LockInput(false);
        }
    }
}