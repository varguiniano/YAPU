using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Badges;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.UI.Map;
using Varguiniano.YAPU.Runtime.World.OutOfBattleWeathers;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move Mist.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Ice/Mist", fileName = "Mist")]
    public class Mist : SetSideStatusMove
    {
        /// <summary>
        /// Reference to the animation prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private BasicSpriteAnimation AnimationPrefab;

        /// <summary>
        /// Reference to the move's audio.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

        /// <summary>
        /// Weather to set out of battle.
        /// </summary>
        [FoldoutGroup("Out of battle")]
        [SerializeField]
        private OutOfBattleWeather OutOfBattleWeatherToSet;

        /// <summary>
        /// Play the move animation.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// <param name="userType">Type of battler the user is.</param>
        /// <param name="userIndex">User index.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="userPosition">Position of the user.</param>
        /// <param name="targets">Targets types and indexes.</param>
        /// <param name="targetPositions">Position of the targets.</param>
        /// <param name="ignoresAbilities"></param>
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
            List<BasicSpriteAnimation> hazes = targetPositions
                                              .Select(targetPosition => Instantiate(AnimationPrefab, targetPosition))
                                              .ToList();

            yield return WaitAFrame;

            battleManager.Audio.PlayAudio(Audio, pitch: speed);

            foreach (BasicSpriteAnimation haze in hazes) CoroutineRunner.RunRoutine(haze.PlayAnimation(speed, true));

            yield return new WaitForSeconds(.4f / speed);

            DOVirtual.DelayedCall(3,
                                  () =>
                                  {
                                      foreach (BasicSpriteAnimation haze in hazes) Destroy(haze.gameObject);
                                  });
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