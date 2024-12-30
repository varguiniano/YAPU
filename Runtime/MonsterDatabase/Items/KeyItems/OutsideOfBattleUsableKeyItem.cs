using UnityEngine;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.KeyItems
{
    /// <summary>
    /// Data class for an item that can only be used outside of battle and with no targets.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Key/OutsideOfBattleUsableKeyItem",
                     fileName = "OutsideOfBattleUsableKeyItem")]
    public class OutsideOfBattleUsableKeyItem : KeyItem
    {
        /// <summary>
        /// This item can be used only outside of battle with no targets.
        /// </summary>
        public override bool CanBeUsed => true;

        /// <summary>
        /// Key items can be registered.
        /// </summary>
        public override bool CanBeRegistered => true;

        /// <summary>
        /// This item can be used only outside of battle with no targets.
        /// </summary>
        public override bool CanBeUsedOnTarget => false;

        /// <summary>
        /// This item can be used only outside of battle with no targets.
        /// </summary>
        public override bool CanBeUsedOnTargetMove => false;

        /// <summary>
        /// This item can be used only outside of battle with no targets.
        /// </summary>
        public override bool CanBeUsedInBattle => false;

        /// <summary>
        /// This item can be used only outside of battle with no targets.
        /// </summary>
        public override bool CanBeUsedInBattleOnTarget => false;

        /// <summary>
        /// This item can be used only outside of battle with no targets.
        /// </summary>
        public override bool CanBeUsedInBattleOnTargetMove => false;
    }
}