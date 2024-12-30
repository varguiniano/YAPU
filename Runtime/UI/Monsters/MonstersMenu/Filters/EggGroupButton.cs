using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.EggGroups;
using WhateverDevs.Core.Runtime.DependencyInjection;
using WhateverDevs.Localization.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.MonstersMenu.Filters
{
    /// <summary>
    /// Controller for a button in the storage filters that filters by egg group.
    /// </summary>
    public class EggGroupButton : VirtualizedMenuItem
    {
        /// <summary>
        /// Reference to the button text.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro Text;

        /// <summary>
        /// Set the data for this button.
        /// </summary>
        /// <param name="entry">Entry to display.</param>
        public void SetData(EggGroup entry) => Text.SetValue(entry.LocalizableName);

        /// <summary>
        /// Dependency injection factory.
        /// </summary>
        public class Factory : GameObjectFactory<EggGroupButton>
        {
        }
    }
}