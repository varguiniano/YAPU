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
    /// Data class for the move MirrorMove.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Flying/MirrorMove", fileName = "MirrorMove")]
    public class MirrorMove : Move
    {
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
            Battler preferredTarget = battleManager.Battlers.GetBattlerFromBattleIndex(targets[0]);

            Move move = preferredTarget.LastPerformedAction.LastMove;

            if (move == null || !move.AffectedByMirror)
            {
                yield return DialogManager.ShowDialogAndWait("Battle/Move/Failed",
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);

                finishedCallback.Invoke(false);
                yield break;
            }

            yield return MoveUtils.TryGenerateRandomTargetsForMove(battleManager,
                                                                   move,
                                                                   userType,
                                                                   userIndex,
                                                                   preferredTarget,
                                                                   Logger,
                                                                   newTargets => targets = newTargets);

            if (targets == null)
            {
                yield return DialogManager.ShowDialogAndWait("Battle/Move/Failed",
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);

                finishedCallback.Invoke(false);
                yield break;
            }

            yield return battleManager.Moves.ForcePerformMove(userType, userIndex, targets, move, ignoreStatus: true);
            finishedCallback.Invoke(true);
        }
    }
}