using System;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Data class to store information about items consumed or stolen in battle.
    /// </summary>
    [Serializable]
    public class ConsumedItemData
    {
        /// <summary>
        /// Item that has been consumed.
        /// </summary>
        public Item ConsumedItem;

        /// <summary>
        /// Can this item be recovered by effects like Recycle?
        /// </summary>
        public bool CanBeRecycled;

        /// <summary>
        /// Can this item be recovered after battle?
        /// </summary>
        public bool CanBeRecoveredAfterBattle;
        
        /// <summary>
        /// Has the battle consumed an item?
        /// </summary>
        public bool HasConsumedItem => ConsumedItem != null;
    }
}