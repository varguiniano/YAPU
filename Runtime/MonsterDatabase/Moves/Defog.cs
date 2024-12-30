using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Badges;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Side;
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
    /// Data class for the move Defog.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Flying/Defog", fileName = "Defog")]
    public class Defog : StageChangeMove
    {
        /// <summary>
        /// Weathers cleared by this move.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private List<Weather> ClearedWeathers;

        // Terrains are all cleared, so no need for a list.

        /// <summary>
        /// Side statuses cleared on both sides by this move.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private List<SideStatus> ClearedSideStatusesOnBothSides;

        /// <summary>
        /// Side statuses cleared on the opponent side by this move.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private List<SideStatus> ClearedSideStatusesOnOpponentSide;

        /// <summary>
        /// Does this move clear substitutes?
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private bool ClearSubstitutes;

        /// <summary>
        /// Weathers cleared by this move.
        /// </summary>
        [FoldoutGroup("Out of battle")]
        [SerializeField]
        private List<OutOfBattleWeather> OutOfBattleClearedWeathers;

        /// <summary>
        /// Change the weathers, side statuses and terrains.
        /// </summary>
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

            if (ClearedWeathers.Contains(weather)) yield return battleManager.Scenario.ClearWeather();

            yield return battleManager.Scenario.ClearTerrain();

            Queue<SideStatus> toRemove = new();

            foreach ((BattlerType targetType, int _) in targets)
            {
                foreach (KeyValuePair<SideStatus, int> statusSlot in battleManager.Statuses.GetSideStatuses(targetType)
                            .Where(statusSlot =>
                                       ClearedSideStatusesOnBothSides.Contains(statusSlot.Key)
                                    || ClearedSideStatusesOnOpponentSide.Contains(statusSlot.Key)))
                    toRemove.Enqueue(statusSlot.Key);

                while (toRemove.TryDequeue(out SideStatus status))
                    yield return battleManager.Statuses.RemoveStatus(status, targetType);
            }

            foreach (KeyValuePair<SideStatus, int> statusSlot in battleManager.Statuses.GetSideStatuses(userType)
                                                                              .Where(statusSlot =>
                                                                                   ClearedSideStatusesOnBothSides
                                                                                      .Contains(statusSlot
                                                                                          .Key)))
                toRemove.Enqueue(statusSlot.Key);

            while (toRemove.TryDequeue(out SideStatus status))
                yield return battleManager.Statuses.RemoveStatus(status, userType);

            if (ClearSubstitutes)
                foreach (Battler battler in battleManager.Battlers.GetBattlersFighting()
                                                         .Where(battler => battler.Substitute.SubstituteEnabled))
                {
                    yield return DialogManager.ShowDialogAndWait("Battle/Substitute/Broken",
                                                                 localizableModifiers: false,
                                                                 modifiers: battler.GetNameOrNickName(battleManager
                                                                    .Localizer),
                                                                 switchToNextAfterSeconds: 1.5f
                                                                   / battleManager.BattleSpeed);

                    battler.Substitute.SubstituteEnabled = false;

                    yield return battleManager.GetMonsterSprite(battler).HideSubstitute(battleManager);
                }

            yield return base.ExecuteEffect(battleManager,
                                            localizer,
                                            userType,
                                            userIndex,
                                            user,
                                            targets,
                                            hitNumber,
                                            expectedHits,
                                            externalPowerMultiplier,
                                            ignoresAbilities,
                                            finishedCallback);
        }

        // TODO: Animation.

        /// <summary>
        /// Make the player use defog.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player.</param>
        /// <param name="monster">Monster using the move.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="mapSceneLauncher"></param>
        public override IEnumerator UseOutOfBattle(PlayerCharacter playerCharacter,
                                                   MonsterInstance monster,
                                                   ILocalizer localizer,
                                                   MapSceneLauncher mapSceneLauncher)
        {
            if (playerCharacter.CharacterController.CurrentGrid.SceneInfo.Region.IsMoveLockedByBadge(this,
                    out Badge badge))
                if (!playerCharacter.GlobalGameData.HasBadge(badge, playerCharacter.Region))
                {
                    yield return DialogManager.ShowDialogAndWait("Dialogs/Moves/BlockedByBadge",
                                                                 modifiers: new[]
                                                                            {
                                                                                badge.LocalizableName, LocalizableName
                                                                            });

                    yield break;
                }

            if (!OutOfBattleClearedWeathers.Contains(playerCharacter.CurrentWeather)
             || playerCharacter.ClearedWeathers.Contains(playerCharacter.CurrentWeather))
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

            yield return playerCharacter.ClearWeather();

            playerCharacter.GlobalGridManager.StopAllActors = false;
            playerCharacter.LockInput(false);
        }
    }
}