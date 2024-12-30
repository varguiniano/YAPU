using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.UI.Monsters.MonstersMenu.Filters;
using WhateverDevs.Localization.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Dex.Filters
{
    /// <summary>
    /// Controller for the button that allows to filter by known moves.
    /// </summary>
    public class DexCanLearnMoveFilterButton : ToggableFilterButton
    {
        /// <summary>
        /// Reference to the menu.
        /// </summary>
        [SerializeField]
        private MoveFilterMenu Menu;

        /// <summary>
        /// Reference to the panel that contains the filter selectors.
        /// </summary>
        [SerializeField]
        private FilterSelectorPanel Panel;

        /// <summary>
        /// Selected move.
        /// </summary>
        private Move move;

        /// <summary>
        /// First part of the button.
        /// </summary>
        private static string IntroString => "Menu/Pokemon/Cloud/SortAndFilter/CanLearnMove";

        /// <summary>
        /// Reference to the monster database.
        /// </summary>
        [Inject]
        private MonsterDatabaseInstance database;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Apply the filter that is enabled.
        /// </summary>
        /// <param name="original">Original list.</param>
        /// <returns>Filtered list.</returns>
        protected override IEnumerable<(MonsterDexEntry, FormDexEntry, MonsterGender)> ApplyEnabledFilter(
            IEnumerable<(MonsterDexEntry, FormDexEntry, MonsterGender)> original) =>
            original.Where(tuple => tuple.Item1.HasMonsterBeenSeen
                                 && tuple.Item2.HasFormBeenSeen
                                 && tuple.Item1.Species[tuple.Item2.Form].CanLearnMove(move));

        /// <summary>
        /// Display the species selector.
        /// </summary>
        public override void OnButtonSelected()
        {
            Menu.OnButtonSelected += OnMoveSelected;

            StartCoroutine(Panel.SlideIn());
            Menu.Show();
        }

        /// <summary>
        /// Called when a move has been selected.
        /// </summary>
        /// <param name="index">Index of the selected move.</param>
        private void OnMoveSelected(int index)
        {
            Menu.OnButtonSelected -= OnMoveSelected;

            StartCoroutine(Panel.SlideOut());

            FilterEnabled = true;

            move = database.GetMoves()[index];

            UpdateText();
        }

        /// <summary>
        /// Update the text in the button.
        /// </summary>
        protected override void UpdateText() =>
            Text.SetText(localizer[IntroString]
                       + " "
                       + (FilterEnabled ? localizer[move.LocalizableName] : "-"));
    }
}