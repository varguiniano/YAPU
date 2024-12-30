using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Base class for moves that follow up on the order right after another move.
    /// </summary>
    public abstract class FollowUpDamageMove : DamageMove
    {
        /// <summary>
        /// List of moves to follow up to.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMoves))]
        #endif
        [FoldoutGroup("Follow Up")]
        [SerializeField]
        private List<Move> MovesToFollowUp;

        /// <summary>
        /// If one of the other moves has been added, follow up.
        /// </summary>
        /// <param name="moveOwner">Owner of the move.</param>
        /// <param name="lastAdded">Last added battler.</param>
        /// <param name="order">Current calculated order.</param>
        /// <param name="battlers">Battlers that will perform actions this turn.</param>
        /// <param name="actions">Actions that will be performed.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns> An int value.
        /// -1 -> Go immediately before the last action added.
        /// 0 -> Follow normal ordering.
        /// 1 -> Go immediately after the last action added.
        /// </returns>
        public override int OnActionAddedToOrder(Battler moveOwner,
                                                 Battler lastAdded,
                                                 ref Queue<Battler> order,
                                                 List<Battler> battlers,
                                                 SerializableDictionary<Battler, BattleAction> actions,
                                                 BattleManager battleManager)
        {
            BattleAction lastAction = actions[lastAdded];

            if (lastAction.ActionType != BattleAction.Type.Move) return 0;

            int lastAddedIndex = lastAction.Parameters[0];

            if (lastAddedIndex is < 0 or > 3) return 0;

            Move lastAddedMove = lastAdded.CurrentMoves[lastAddedIndex].Move;

            if (lastAddedMove == null) return 0;

            return MovesToFollowUp.Contains(lastAddedMove) ? 1 : 0;
        }
    }
}