namespace Varguiniano.YAPU.Runtime.UI.Dex
{
    /// <summary>
    /// Controller for menus that display monster relationships.
    /// </summary>
    public class MonsterRelationshipMenu : VirtualizedMenuSelector<DexMonsterRelationshipData, MonsterRelationshipButton
        , MonsterRelationshipButton.Factory>
    {
        /// <summary>
        /// Set the data into a button.
        /// </summary>
        protected override void
            PopulateChildData(MonsterRelationshipButton child, DexMonsterRelationshipData childData) =>
            child.SetRelationship(childData);
    }
}