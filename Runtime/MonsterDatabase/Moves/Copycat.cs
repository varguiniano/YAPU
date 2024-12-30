using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move Copycat.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/Copycat", fileName = "Copycat")]
    public class Copycat : Move
    {
        /// <summary>
        /// Moves that can't be called
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
            Battler preferredTarget = BattleAI.GetEnemies(battleManager, userType).Random();

            Move move = battleManager.Moves.LastSuccessfullyPerformedMove;

            if (move == null || IncompatibleMoves.Contains(move))
            {
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
                finishedCallback.Invoke(false);
                yield break;
            }

            yield return battleManager.Moves.ForcePerformMove(userType, userIndex, targets, move, ignoreStatus: true);
            finishedCallback.Invoke(true);
        }
    }
}