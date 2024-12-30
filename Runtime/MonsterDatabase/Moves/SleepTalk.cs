using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move SleepTalk.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/SleepTalk", fileName = "SleepTalk")]
    public class SleepTalk : SleepOnlyMove
    {
        /// <summary>
        /// Moves that are incompatible with Sleep Talk.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMoves))]
        #endif
        [FoldoutGroup("Effect")]
        [SerializeField]
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
            Battler battler = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            Battler preferredTarget = BattleAI.GetEnemies(battleManager, userType).Random();

            Move move = battleManager.RandomProvider.RandomElement(battler.CurrentMoves
                                                                          .Where(slot => slot.Move != null
                                                                            && !IncompatibleMoves
                                                                                  .Contains(slot.Move))
                                                                          .ToList())
                                     .Move;

            yield return MoveUtils.TryGenerateRandomTargetsForMove(battleManager,
                                                                   move,
                                                                   userType,
                                                                   userIndex,
                                                                   preferredTarget,
                                                                   Logger,
                                                                   newTargets => targets = newTargets);

            if (targets == null)
            {
                finishedCallback.Invoke(false);
                yield break;
            }

            yield return battleManager.Moves.ForcePerformMove(userType, userIndex, targets, move, ignoreStatus: true);
            finishedCallback.Invoke(true);
        }
    }
}