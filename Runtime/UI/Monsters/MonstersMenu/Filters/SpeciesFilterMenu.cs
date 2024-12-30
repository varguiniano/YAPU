using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.MonstersMenu.Filters
{
    /// <summary>
    /// Menu for selecting a species filter.
    /// </summary>
    public class SpeciesFilterMenu : VirtualizedMenuSelector<MonsterEntry, SpeciesButton, SpeciesButton.Factory>
    {
        /// <summary>
        /// Reference to the monster database.
        /// </summary>
        [Inject]
        private MonsterDatabaseInstance database;

        /// <summary>
        /// Set the data.
        /// </summary>
        public override void Show(bool show = true)
        {
            if (show) SetButtons(database.GetMonsterEntries());
            base.Show(show);
        }

        /// <summary>
        /// Set the species on the button.
        /// </summary>
        /// <param name="child">Button.</param>
        /// <param name="childData">Data to set.</param>
        protected override void PopulateChildData(SpeciesButton child, MonsterEntry childData) =>
            child.SetData(childData);
    }
}