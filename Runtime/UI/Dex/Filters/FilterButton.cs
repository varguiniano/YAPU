using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDex;
using WhateverDevs.Localization.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Dex.Filters
{
    /// <summary>
    /// Class controlling a menu element that applies a filter to the dex.
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
        /// <param name="originalEntries">Original dex entries.</param>
        /// <returns>Filtered list.</returns>
        public abstract IEnumerable<(MonsterDexEntry, FormDexEntry, MonsterGender)>
            ApplyFilter(IEnumerable<(MonsterDexEntry, FormDexEntry, MonsterGender)> originalEntries);

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