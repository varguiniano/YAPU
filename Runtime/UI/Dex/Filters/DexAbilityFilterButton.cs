using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.UI.Monsters.MonstersMenu.Filters;
using WhateverDevs.Localization.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Dex.Filters
{
    /// <summary>
    /// Button in charge of filtering the storage by ability.
    /// </summary>
    public class DexAbilityFilterButton : ToggableFilterButton
    {
        /// <summary>
        /// Reference to the species filter menu.
        /// </summary>
        [SerializeField]
        private AbilityFilterMenu Menu;

        /// <summary>
        /// Reference to the panel that contains the filter selectors.
        /// </summary>
        [SerializeField]
        private FilterSelectorPanel Panel;

        /// <summary>
        /// Selected ability.
        /// </summary>
        private Ability ability;

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

            Menu.OnButtonSelected += OnAbilitySelected;
        }

        /// <summary>
        /// Unsubscribe from the species menu.
        /// </summary>
        protected override void OnDisable()
        {
            Menu.OnButtonSelected -= OnAbilitySelected;

            base.OnDisable();
        }

        /// <summary>
        /// Apply the filter that is enabled.
        /// </summary>
        /// <param name="original">Original list.</param>
        /// <returns>Filtered list.</returns>
        protected override IEnumerable<(MonsterDexEntry, FormDexEntry, MonsterGender)> ApplyEnabledFilter(
            IEnumerable<(MonsterDexEntry, FormDexEntry, MonsterGender)> original) =>
            original.Where(tuple => tuple.Item1.HasMonsterBeenSeen
                                 && tuple.Item2.HasFormBeenSeen
                                 && tuple.Item1.Species[tuple.Item2.Form].CanHaveAbility(ability));

        /// <summary>
        /// Display the species selector.
        /// </summary>
        public override void OnButtonSelected()
        {
            StartCoroutine(Panel.SlideIn());
            Menu.Show();
        }

        /// <summary>
        /// Called when an ability is selected.
        /// </summary>
        /// <param name="index"></param>
        private void OnAbilitySelected(int index)
        {
            StartCoroutine(Panel.SlideOut());

            FilterEnabled = true;

            ability = Menu.Data[index];

            UpdateText();
        }

        /// <summary>
        /// Update the text in the button.
        /// </summary>
        protected override void UpdateText() =>
            Text.SetText(localizer["Menu/Pokemon/Cloud/SortAndFilter/Ability"]
                       + " "
                       + (FilterEnabled ? localizer[ability.LocalizableName] : "-"));
    }
}