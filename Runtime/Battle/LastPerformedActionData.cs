using System;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Data class to store information about the last performed action.
    /// </summary>
    [Serializable]
    public class LastPerformedActionData
    {
        /// <summary>
        /// Last performed action.
        /// </summary>
        public BattleAction.Type LastAction;

        /// <summary>
        /// Was the last move successful?
        /// </summary>
        public bool LastMoveSuccessful { get; private set; }

        /// <summary>
        /// Last performed move.
        /// </summary>
        public Move LastMove { get; private set; }

        /// <summary>
        /// Set the last move performed and if it was successful.
        /// </summary>
        /// <param name="move">Move to set.</param>
        /// <param name="wasSuccessful">Was the move successful?</param>
        internal void SetLastMove(Move move, bool wasSuccessful)
        {
            LastMove = move;
            LastMoveSuccessful = wasSuccessful;
        }
    }
}