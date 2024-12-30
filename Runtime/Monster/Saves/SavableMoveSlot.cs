using System;

namespace Varguiniano.YAPU.Runtime.Monster.Saves
{
    /// <summary>
    /// Version of the move slot class that can be serialized as a string.
    /// </summary>
    [Serializable]
    public class SavableMoveSlot
    {
        /// <summary>
        /// Move.
        /// </summary>
        public int MoveHash;

        /// <summary>
        /// Maximum PP of this move.
        /// </summary>
        public byte MaxPP;

        /// <summary>
        /// Current PP of this move.
        /// </summary>
        public byte CurrentPP;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="slot">Original slot.</param>
        public SavableMoveSlot(MoveSlot slot)
        {
            MoveHash = slot.Move == null ? "Null".GetHashCode() : slot.Move.name.GetHashCode();
            MaxPP = slot.MaxPP;
            CurrentPP = slot.CurrentPP;
        }
    }
}