using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.UI.Monsters.MonstersMenu.Filters;
using Varguiniano.YAPU.Runtime.UI.Types;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Dex.Filters
{
    /// <summary>
    /// Button in charge of filtering the dex by types.
    /// </summary>
    public class DexTypeFilterButton : ToggableFilterButton
    {
        /// <summary>
        /// Reference to the menu.
        /// </summary>
        [SerializeField]
        private MenuSelector TypeFilterMenu;

        /// <summary>
        /// Reference to the panel that contains the filter selectors.
        /// </summary>
        [SerializeField]
        private FilterSelectorPanel Panel;

        /// <summary>
        /// Reference to the type badge.
        /// </summary>
        [SerializeField]
        private TypeBadge Badge;

        /// <summary>
        /// Reference to the text that is offcentered.
        /// </summary>
        [SerializeField]
        private GameObject OffcenteredText;

        /// <summary>
        /// Selected monster type.
        /// </summary>
        private MonsterType typeSelected;

        /// <summary>
        /// Reference to the monster database.
        /// </summary>
        [Inject]
        private MonsterDatabaseInstance database;

        /// <summary>
        /// Apply the filter that is enabled.
        /// </summary>
        /// <param name="original">Original list.</param>
        /// <returns>Filtered list.</returns>
        protected override IEnumerable<(MonsterDexEntry, FormDexEntry, MonsterGender)> ApplyEnabledFilter(
            IEnumerable<(MonsterDexEntry, FormDexEntry, MonsterGender)> original) =>
            original.Where(tuple => tuple.Item1.HasMonsterBeenSeen
                                 && tuple.Item2.HasFormBeenSeen
                                 && tuple.Item1.Species[tuple.Item2.Form].IsOfType(typeSelected));

        /// <summary>
        /// Display the species selector.
        /// </summary>
        public override void OnButtonSelected()
        {
            TypeFilterMenu.OnButtonSelected += OnTypeSelected;

            StartCoroutine(Panel.SlideIn());
            TypeFilterMenu.Show();
        }

        /// <summary>
        /// Called when a type is selected.
        /// </summary>
        /// <param name="index">Index of the selected type.</param>
        private void OnTypeSelected(int index)
        {
            TypeFilterMenu.OnButtonSelected -= OnTypeSelected;

            StartCoroutine(Panel.SlideOut());

            FilterEnabled = true;

            typeSelected = database.GetMonsterTypes()[index];

            UpdateText();
        }

        /// <summary>
        /// Update the text in the button.
        /// </summary>
        protected override void UpdateText()
        {
            if (!FilterEnabled) typeSelected = null;
            Badge.SetType(typeSelected);

            OffcenteredText.SetActive(FilterEnabled);

            Text.SetText(FilterEnabled ? "" : Localizer["Menu/Pokemon/Cloud/SortAndFilter/Type"] + " -");
        }
    }
}