namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.BattleItems
{
    /// <summary>
    /// Data class for a battle item.
    /// </summary>
    public abstract class BattleItem : Item
    {
        /// <summary>
        /// All battle items can stack.
        /// </summary>
        public override bool CanStack => true;

        /// <summary>
        /// Battle items can't be used outside of battle.
        /// </summary>
        public override bool CanBeUsed => false;
        
        /// <summary>
        /// This type of items can't be registered.
        /// </summary>
        public override bool CanBeRegistered => false;

        /// <summary>
        /// Battle items can't be used outside of battle.
        /// </summary>
        public override bool CanBeUsedOnTarget => false;

        /// <summary>
        /// Battle items can't be used outside of battle.
        /// </summary>
        public override bool CanBeUsedOnTargetMove => false;
    }
}