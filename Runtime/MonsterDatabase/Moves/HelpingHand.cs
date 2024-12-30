using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation.Moves;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move Helping hand.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/HelpingHand", fileName = "HelpingHand")]
    public class HelpingHand : SetVolatileStatusMove
    {
        /// <summary>
        /// Reference to the clap animation.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private ClapAnimation ClapAnimationPrefab;

        /// <summary>
        /// Check if the move will fail reasons other than accuracy.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="hitNumber"></param>
        /// <param name="expectedHits"></param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="customFailMessage"></param>
        public override bool WillMoveFail(BattleManager battleManager,
                                          ILocalizer localizer,
                                          BattlerType userType,
                                          int userIndex,
                                          ref List<(BattlerType Type, int Index)> targets,
                                          int hitNumber,
                                          int expectedHits,
                                          bool ignoresAbilities,
                                          out string customFailMessage)
        {
            customFailMessage = "";

            return battleManager.BattleType != BattleType.DoubleBattle
                && base.WillMoveFail(battleManager,
                                     localizer,
                                     userType,
                                     userIndex,
                                     ref targets,
                                     hitNumber,
                                     expectedHits,
                                     ignoresAbilities,
                                     out customFailMessage);
        }

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
            ClapAnimation animation = Instantiate(ClapAnimationPrefab, null);

            yield return animation.Play(speed);

            Destroy(animation.gameObject);

            foreach ((BattlerType targetType, int targetIndex) in targets)
                battleManager.GetMonsterSprite(targetType, targetIndex).FXAnimator.PlayBoost(speed);
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

            foreach ((BattlerType type, int index) in targets)
                yield return DialogManager.ShowDialogAndWait("Moves/HelpingHand/ReadyToHelp",
                                                             localizableModifiers: false,
                                                             modifiers: new[]
                                                                        {
                                                                            battleManager.Battlers
                                                                               .GetBattlerFromBattleIndex(userType,
                                                                                    userIndex)
                                                                               .GetNameOrNickName(localizer),
                                                                            battleManager.Battlers
                                                                               .GetBattlerFromBattleIndex(type,
                                                                                    index)
                                                                               .GetNameOrNickName(localizer)
                                                                        },
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);
        }
    }
}