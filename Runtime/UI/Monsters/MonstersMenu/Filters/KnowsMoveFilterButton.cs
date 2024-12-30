using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using WhateverDevs.Localization.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.MonstersMenu.Filters
{
    /// <summary>
    /// Controller for the button that allows to filter by known moves.
    /// </summary>
    public class KnowsMoveFilterButton : ToggableFilterButton
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
        protected Move Move;

        /// <summary>
        /// First part of the button.
        /// </summary>
        protected virtual string IntroString => "Menu/Pokemon/Cloud/SortAndFilter/KnowsMove";

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
        /// Filter by the species.
        /// </summary>
        /// <param name="original">Original list.</param>
        /// <returns>Filtered list.</returns>
        protected override IEnumerable<MonsterInstance> ApplyEnabledFilter(IEnumerable<MonsterInstance> original) =>
            original.Where(monster => monster.KnowsMove(Move));

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

            Move = database.GetMoves()[index];

            UpdateText();
        }

        /// <summary>
        /// Update the text in the button.
        /// </summary>
        protected override void UpdateText() =>
            Text.SetText(localizer[IntroString]
                       + " "
                       + (FilterEnabled ? localizer[Move.LocalizableName] : "-"));
    }
}