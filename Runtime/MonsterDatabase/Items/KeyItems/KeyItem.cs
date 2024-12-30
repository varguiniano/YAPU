namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.KeyItems
{
    /// <summary>
    /// Base class for a key item.
    /// </summary>
    public abstract class KeyItem : Item
    {
        /// <summary>
        /// Key items can never stack.
        /// </summary>
        public override bool CanStack => false;
    }
}