using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move Roar.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/Roar", fileName = "Roar")]
    public class Roar : Move
    {
        /// <summary>
        /// Check if the move will fail reasons other than accuracy.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="targetIndex">Index of the target.</param>
        /// <param name="ignoresAbilities"></param>
        internal override bool WillMoveFail(BattleManager battleManager,
                                            ILocalizer localizer,
                                            BattlerType userType,
                                            int userIndex,
                                            BattlerType targetType,
                                            int targetIndex,
                                            bool ignoresAbilities)
        {
            // Fail if there are more than one wilds.
            if (targetType == BattlerType.Enemy
             && battleManager.EnemyType == EnemyType.Wild
             && battleManager.Battlers.GetBattlersFighting(BattlerType.Enemy).Count > 1)
                return true;

            // Fail if only one mon left in the target roster and not wild.
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (battleManager.Rosters.GetNumberNotFainted(targetType, targetIndex) == 1
             && (targetType != BattlerType.Enemy
              || battleManager.EnemyType != EnemyType.Wild))
                return true;

            return base.WillMoveFail(battleManager, localizer, userType, userIndex, targetType, targetIndex, ignoresAbilities);
        }

        /// <summary>
        /// If used against a wild and there is only one, run away. If it's a double wild, fail.
        /// If used against a trainer, force switch.
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
            // There should always be one target.
            (BattlerType targetType, int targetIndex) = targets[0];

            // Used against wilds.
            if (targetType == BattlerType.Enemy && battleManager.EnemyType == EnemyType.Wild)
            {
                if (battleManager.Battlers.GetBattlersFighting(BattlerType.Enemy).Count > 1)
                    // Fail if there are more than one wilds. It should never get here, should fail before effect.
                    yield return DialogManager.ShowDialogAndWait("Battle/Move/Failed",
                                                                 switchToNextAfterSeconds: 1.5f
                                                                   / battleManager.BattleSpeed);
                else
                    // Run away.
                    yield return battleManager.Battlers.RunAway(userType, userIndex, false, true);
            }
            else // Used against trainers.
                yield return battleManager.BattleManagerBattlerSwitch.ForceSwitchBattler(targetType,
                    targetIndex,
                    userType,
                    userIndex,
                    this,
                    null,
                    false,
                    ignoresAbilities,
                    true);

            finishedCallback.Invoke(true);
        }
    }
}