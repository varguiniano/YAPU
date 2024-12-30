namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Other
{
    /// <summary>
    /// Base class for other items.
    /// </summary>
    public abstract class OtherItem : Item
    {
        /// <summary>
        /// All other items can stack.
        /// </summary>
        public override bool CanStack => true;
    }
}