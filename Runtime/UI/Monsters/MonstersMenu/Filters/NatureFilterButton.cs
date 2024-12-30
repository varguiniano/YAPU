using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Natures;
using WhateverDevs.Localization.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.MonstersMenu.Filters
{
    /// <summary>
    /// Button in charge of filtering the storage by nature.
    /// </summary>
    public class NatureFilterButton  : ToggableFilterButton
    {
        /// <summary>
        /// Reference to the species filter menu.
        /// </summary>
        [SerializeField]
        private NatureFilterMenu Menu;

        /// <summary>
        /// Reference to the panel that contains the filter selectors.
        /// </summary>
        [SerializeField]
        private FilterSelectorPanel Panel;

        /// <summary>
        /// Selected nature.
        /// </summary>
        private Nature nature;
        
        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Subscribe to the species menu.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            Menu.OnButtonSelected += OnNatureSelected;
        }

        /// <summary>
        /// Unsubscribe from the species menu.
        /// </summary>
        protected override void OnDisable()
        {
            Menu.OnButtonSelected -= OnNatureSelected;

            base.OnDisable();
        }

        /// <summary>
        /// Filter by the species.
        /// </summary>
        /// <param name="original">Original list.</param>
        /// <returns>Filtered list.</returns>
        protected override IEnumerable<MonsterInstance> ApplyEnabledFilter(IEnumerable<MonsterInstance> original) =>
            original.Where(monster => monster.StatData.Nature == nature);
        
        /// <summary>
        /// Display the species selector.
        /// </summary>
        public override void OnButtonSelected()
        {
            StartCoroutine(Panel.SlideIn());
            Menu.Show();
        }

        /// <summary>
        /// Called when a nature has been selected.
        /// </summary>
        /// <param name="index">Index of the selected nature.</param>
        private void OnNatureSelected(int index)
        {
            StartCoroutine(Panel.SlideOut());

            FilterEnabled = true;

            nature = Menu.Data[index];

            UpdateText();
        }
        
        /// <summary>
        /// Update the text in the button.
        /// </summary>
        protected override void UpdateText() =>
            Text.SetText(localizer["Menu/Pokemon/Cloud/SortAndFilter/Nature"]
                       + " "
                       + (FilterEnabled ? localizer[nature.LocalizableName] : "-"));
    }
}