using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Options
{
    /// <summary>
    /// Base class for menu items that are part of the options menu.
    /// </summary>
    public abstract class OptionsMenuItem : MenuItem
    {
        /// <summary>
        /// Can the selection loop?
        /// </summary>
        [SerializeField]
        private bool CanLoop = true;

        /// <summary>
        /// Reference to the name text.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro Name;

        /// <summary>
        /// Does this option have a slider?
        /// </summary>
        [SerializeField]
        private bool HasSlider;

        /// <summary>
        /// Reference to the slider to use.
        /// </summary>
        [ShowIf(nameof(HasSlider))]
        [SerializeField]
        private Slider Slider;

        /// <summary>
        /// Offset to add to the slider.
        /// </summary>
        [ShowIf(nameof(HasSlider))]
        [SerializeField]
        private int SliderOffset;

        /// <summary>
        /// Localization keys for each of the options in this menu item.
        /// </summary>
        [SerializeField]
        protected List<ObjectPair<string, string>> Options;

        /// <summary>
        /// Index of the currently selected option.
        /// </summary>
        protected int CurrentIndex;

        /// <summary>
        /// Switch the option.
        /// <param name="direction">Less than 0 indicates left, more than 0 indicates right.</param>
        /// <returns>Localizable key for the description of the new option.</returns>
        /// </summary>
        public string SwitchOption(float direction)
        {
            switch (direction)
            {
                case < 0:
                    CurrentIndex--;
                    break;
                case > 0:
                    CurrentIndex++;
                    break;
            }

            return SetOption(CurrentIndex, true);
        }

        /// <summary>
        /// Set a new option.
        /// </summary>
        /// <param name="index">Index of the option to set.</param>
        /// <param name="isFirstSetup"></param>
        /// <returns>Localizable key for the description of the new option.</returns>
        protected string SetOption(int index, bool isFirstSetup)
        {
            CurrentIndex = index;

            if (CurrentIndex < 0) CurrentIndex = CanLoop ? Options.Count - 1 : 0;

            if (CurrentIndex >= Options.Count) CurrentIndex = CanLoop ? 0 : Options.Count - 1;

            ObjectPair<string, string> keys = Options[CurrentIndex];

            Name.SetValue(keys.Key);

            if (HasSlider)
            {
                Slider.maxValue = Options.Count - 1 + SliderOffset;
                Slider.value = CurrentIndex + SliderOffset;
            }

            OnOptionSwitched(isFirstSetup);

            return keys.Value;
        }
        
        /// <summary>
        /// Get the description to display.
        /// </summary>
        public string GetDescription() => Options[CurrentIndex].Value;

        /// <summary>
        /// Called when a new option is chosen.
        /// </summary>
        /// <param name="isFirstSetup"></param>
        protected abstract void OnOptionSwitched(bool isFirstSetup);
    }
}