using UnityEngine;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.KeyItems
{
    /// <summary>
    /// Data class for an item that just sits on the bag without use.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Key/InertItem", fileName = "InertItem")]
    public class InertKeyItem : KeyItem
    {
        /// <summary>
        /// This Item has no direct bag uses.
        /// </summary>
        public override bool CanBeUsed => false;

        /// <summary>
        /// This type of items can't be registered.
        /// </summary>
        public override bool CanBeRegistered => false;

        /// <summary>
        /// This Item has no direct bag uses.
        /// </summary>
        public override bool CanBeUsedOnTarget => false;

        /// <summary>
        /// This Item has no direct bag uses.
        /// </summary>
        public override bool CanBeUsedOnTargetMove => false;

        /// <summary>
        /// This Item has no direct bag uses.
        /// </summary>
        public override bool CanBeUsedInBattle => false;

        /// <summary>
        /// This Item has no direct bag uses.
        /// </summary>
        public override bool CanBeUsedInBattleOnTarget => false;

        /// <summary>
        /// This Item has no direct bag uses.
        /// </summary>
        public override bool CanBeUsedInBattleOnTargetMove => false;
    }
}