using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move MeFirst.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/MeFirst", fileName = "MeFirst")]
    public class MeFirst : Move
    {
        /// <summary>
        /// Moves that are incompatible with me first.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllDamageMoves))]
        #endif
        private List<Move> IncompatibleMoves;

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
            List<Battler> order = battleManager.CurrentTurnActionOrder.ToList();
            SerializableDictionary<Battler, BattleAction> actions = battleManager.CurrentTurnActions;

            foreach ((BattlerType targetType, int targetIndex) in targets)
            {
                Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex);

                // Do nothing if the target went before the user.
                if (order.IndexOf(user) > order.IndexOf(target))
                {
                    yield return
                        DialogManager.ShowDialogAndWait("Battle/Move/NoEffect", switchToNextAfterSeconds: 1.5f);

                    finishedCallback.Invoke(false);
                    yield break;
                }

                // Do nothing if the target is not going to use a damage move.
                BattleAction targetAction = actions[target];
                int moveIndex = targetAction.Parameters.Length > 0 ? targetAction.Parameters[0] : -1;

                if (targetAction.ActionType != BattleAction.Type.Move || moveIndex is < 0 or > 3)
                {
                    yield return
                        DialogManager.ShowDialogAndWait("Battle/Move/NoEffect", switchToNextAfterSeconds: 1.5f);

                    finishedCallback.Invoke(false);
                    yield break;
                }

                DamageMove targetMove = target.CurrentMoves[moveIndex].Move as DamageMove;

                if (targetMove == null)
                {
                    yield return
                        DialogManager.ShowDialogAndWait("Battle/Move/NoEffect", switchToNextAfterSeconds: 1.5f);

                    finishedCallback.Invoke(false);
                    yield break;
                }

                // Do nothing if it is an incompatible move.
                if (IncompatibleMoves.Contains(targetMove))
                {
                    yield return
                        DialogManager.ShowDialogAndWait("Battle/Move/NoEffect", switchToNextAfterSeconds: 1.5f);

                    finishedCallback.Invoke(false);
                    yield break;
                }

                yield return MoveUtils.TryGenerateRandomTargetsForMove(battleManager,
                                                                       targetMove,
                                                                       userType,
                                                                       userIndex,
                                                                       target,
                                                                       Logger,
                                                                       newTargets => targets = newTargets);

                if (targets == null)
                {
                    yield return
                        DialogManager.ShowDialogAndWait("Battle/Move/NoEffect", switchToNextAfterSeconds: 1.5f);

                    finishedCallback.Invoke(false);
                    yield break;
                }

                // Use with 50% more power.
                yield return battleManager.Moves.ForcePerformMove(userType,
                                                                  userIndex,
                                                                  targets,
                                                                  targetMove,
                                                                  externalPowerMultiplier: 1.5f,
                                                                  ignoreStatus: true);

                finishedCallback.Invoke(true);
            }
        }
    }
}