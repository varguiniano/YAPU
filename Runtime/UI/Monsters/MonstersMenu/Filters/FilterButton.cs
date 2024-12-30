using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using WhateverDevs.Localization.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.MonstersMenu.Filters
{
    /// <summary>
    /// Class controlling a menu element that applies a filter to the storage.
    /// </summary>
    public abstract class FilterButton : MenuItem
    {
        /// <summary>
        /// Reference to the button text.
        /// </summary>
        [SerializeField]
        protected TMP_Text Text;

        /// <summary>
        /// Localizer.
        /// </summary>
        [Inject]
        protected ILocalizer Localizer;

        /// <summary>
        /// Update the text.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            UpdateText();

            Localizer.SubscribeToLanguageChange(UpdateText);
        }

        /// <summary>
        /// Unsubscribe from updating the text.
        /// </summary>
        protected override void OnDisable()
        {
            Localizer.UnsubscribeFromLanguageChange(UpdateText);

            base.OnDisable();
        }

        /// <summary>
        /// Apply the filter of this button to the list.
        /// </summary>
        /// <param name="original">Original list.</param>
        /// <returns>Filtered list.</returns>
        public abstract IEnumerable<MonsterInstance> ApplyFilter(IEnumerable<MonsterInstance> original);

        /// <summary>
        /// Called when the button is selected.
        /// </summary>
        public abstract void OnButtonSelected();

        /// <summary>
        /// Update the text in the button.
        /// </summary>
        protected abstract void UpdateText();
    }
}