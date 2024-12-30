namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Medicines
{
    /// <summary>
    /// Data class for a medicine item.
    /// </summary>
    public abstract class Medicine : Item
    {
        /// <summary>
        /// All medicines can stack.
        /// </summary>
        public override bool CanStack => true;
    }
}