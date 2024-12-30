﻿using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Natures;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.MonstersMenu.Filters
{
    /// <summary>
    /// Menu to select the nature by which to filter the storage.
    /// </summary>
    public class NatureFilterMenu : VirtualizedMenuSelector<Nature, NatureButton, NatureButton.Factory>
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
            if (show) SetButtons(database.GetNatures());
            base.Show(show);
        }

        /// <summary>
        /// Set the species on the button.
        /// </summary>
        /// <param name="child">Button.</param>
        /// <param name="childData">Data to set.</param>
        protected override void PopulateChildData(NatureButton child, Nature childData) => child.SetData(childData);
    }
}