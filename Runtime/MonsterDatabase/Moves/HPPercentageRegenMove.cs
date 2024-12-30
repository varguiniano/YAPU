using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for a move that regenerates HP.
    /// </summary>
    public abstract class HPPercentageRegenMove : SetVolatileStatusMove
    {
        /// <summary>
        /// Fail if can't heal.
        /// </summary>
        internal override bool WillMoveFail(BattleManager battleManager,
                                            ILocalizer localizer,
                                            BattlerType userType,
                                            int userIndex,
                                            BattlerType targetType,
                                            int targetIndex,
                                            bool ignoresAbilities)
        {
            Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex);

            return base.WillMoveFail(battleManager, localizer, userType, userIndex, targetType, targetIndex, ignoresAbilities)
                || !target.CanHeal(battleManager)
                || target.IsAtMaxHP();
        }

        /// <summary>
        /// Get the percentage of HP to regen.
        /// </summary>
        /// <param name="battleManager"></param>
        /// <param name="userType"></param>
        /// <param name="userIndex"></param>
        /// <param name="targetType"></param>
        /// <param name="targetIndex"></param>
        /// <returns>A number between 0 and 1.</returns>
        protected abstract float GetRegenPercentage(BattleManager battleManager,
                                                    BattlerType userType,
                                                    int userIndex,
                                                    BattlerType targetType,
                                                    int targetIndex);

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
            foreach ((BattlerType targetType, int targetIndex) in targets)
            {
                Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex);

                if (!target.CanHeal(battleManager))
                {
                    yield return DialogManager.ShowDialogAndWait("Battle/Move/NoEffect",
                                                                 switchToNextAfterSeconds: 1.5f
                                                                   / battleManager.BattleSpeed);

                    yield break;
                }

                int hpToRegen =
                    (int) (MonsterMathHelper.CalculateStat(target,
                                                           Stat.Hp,
                                                           battleManager)
                         * GetRegenPercentage(battleManager, userType, userIndex, targetType, targetIndex));

                int amount = 0;

                yield return battleManager.BattlerHealth.ChangeLife(targetType,
                                                                    targetIndex,
                                                                    userType,
                                                                    userIndex,
                                                                    hpToRegen,
                                                                    finished: (regenerated, _) => amount = regenerated);

                yield return DialogManager.ShowDialogAndWait("Battle/RecoverHP",
                                                             localizableModifiers: false,
                                                             modifiers: new[]
                                                                        {
                                                                            target.GetNameOrNickName(localizer),
                                                                            amount.ToString()
                                                                        },
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);

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
                                                _ =>
                                                {
                                                });
            }

            finishedCallback.Invoke(true);
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
            foreach ((BattlerType targetType, int targetIndex) in targets)
                yield return battleManager.GetMonsterSprite(targetType, targetIndex)
                                          .FXAnimator.PlayBoostRoutine(battleManager.BattleSpeed);
        }
    }
}