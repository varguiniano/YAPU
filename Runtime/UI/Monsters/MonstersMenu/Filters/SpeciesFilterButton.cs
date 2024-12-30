using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using WhateverDevs.Localization.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.MonstersMenu.Filters
{
    /// <summary>
    /// Button in charge of filtering the storage by species.
    /// </summary>
    public class SpeciesFilterButton : ToggableFilterButton
    {
        /// <summary>
        /// Reference to the species filter menu.
        /// </summary>
        [SerializeField]
        private SpeciesFilterMenu Menu;

        /// <summary>
        /// Reference to the panel that contains the filter selectors.
        /// </summary>
        [SerializeField]
        private FilterSelectorPanel Panel;

        /// <summary>
        /// Selected species.
        /// </summary>
        private MonsterEntry species;

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

            Menu.OnButtonSelected += OnSpeciesButtonSelected;
        }

        /// <summary>
        /// Unsubscribe from the species menu.
        /// </summary>
        protected override void OnDisable()
        {
            Menu.OnButtonSelected -= OnSpeciesButtonSelected;

            base.OnDisable();
        }

        /// <summary>
        /// Filter by the species.
        /// </summary>
        /// <param name="original">Original list.</param>
        /// <returns>Filtered list.</returns>
        protected override IEnumerable<MonsterInstance> ApplyEnabledFilter(IEnumerable<MonsterInstance> original) =>
            original.Where(monster => monster.Species == species);

        /// <summary>
        /// Display the species selector.
        /// </summary>
        public override void OnButtonSelected()
        {
            StartCoroutine(Panel.SlideIn());
            Menu.Show();
        }

        /// <summary>
        /// Called when a species is chosen.
        /// </summary>
        /// <param name="index">Index of the chosen species.</param>
        private void OnSpeciesButtonSelected(int index)
        {
            StartCoroutine(Panel.SlideOut());
            
            FilterEnabled = true;

            species = Menu.Data[index];

            UpdateText();
        }

        /// <summary>
        /// Update the text in the button.
        /// </summary>
        protected override void UpdateText() =>
            Text.SetText(localizer["Menu/Pokemon/Cloud/SortAndFilter/Species"]
                       + " "
                       + (FilterEnabled ? localizer[species.LocalizableName] : "-"));
    }
}