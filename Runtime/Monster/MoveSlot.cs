using System;
using Sirenix.OdinInspector;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.Monster
{
    /// <summary>
    /// Class that stores a move slot in a monster.
    /// </summary>
    [Serializable]
    public struct MoveSlot
    {
        /// <summary>
        /// Reference to the move.
        /// </summary>
        [OnValueChanged(nameof(UpdatePP))]
        public Move Move;

        /// <summary>
        /// Maximum PP of this move.
        /// </summary>
        public byte MaxPP;

        /// <summary>
        /// Current PP of this move.
        /// </summary>
        public byte CurrentPP;

        /// <summary>
        /// Creates a new move slot from a move.
        /// </summary>
        /// <param name="move">The move to use in the slot.</param>
        internal MoveSlot(Move move)
        {
            Move = move;

            MaxPP = Move == null ? (byte) 0 : Move.BasePowerPoints;

            CurrentPP = MaxPP;
        }

        /// <summary>
        /// Update the move PP.
        /// </summary>
        private void UpdatePP()
        {
            if (Move == null) return;
            MaxPP = Move.BasePowerPoints;
            CurrentPP = MaxPP;
        }
    }
}