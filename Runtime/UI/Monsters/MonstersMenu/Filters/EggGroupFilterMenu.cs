using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.EggGroups;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.MonstersMenu.Filters
{
    /// <summary>
    /// Menu to select the egg group by which to filter the storage.
    /// </summary>
    public class EggGroupFilterMenu : VirtualizedMenuSelector<EggGroup, EggGroupButton, EggGroupButton.Factory>
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
            if (show) SetButtons(database.GetEggGroups());
            base.Show(show);
        }

        /// <summary>
        /// Set the species on the button.
        /// </summary>
        /// <param name="child">Button.</param>
        /// <param name="childData">Data to set.</param>
        protected override void PopulateChildData(EggGroupButton child, EggGroup childData) => child.SetData(childData);
    }
}