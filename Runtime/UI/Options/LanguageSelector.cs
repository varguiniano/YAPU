using System.Collections.Generic;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Options
{
    /// <summary>
    /// Options selector for the language.
    /// </summary>
    public class LanguageSelector : OptionsMenuItem
    {
        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        private ILocalizer localizer;

        /// <summary>
        /// List of available languages.
        /// </summary>
        private List<string> languages;

        /// <summary>
        /// Get the current option.
        /// </summary>
        /// <param name="localizerReference">Reference to the localizer.</param>
        [Inject]
        public void Construct(ILocalizer localizerReference)
        {
            localizer = localizerReference;

            UpdateOptions();
        }

        /// <summary>
        /// Update the displayed options.
        /// </summary>
        private void UpdateOptions()
        {
            languages = localizer.GetAllLanguageIds();

            Options = new List<ObjectPair<string, string>>();

            foreach (string language in languages)
                Options.Add(new ObjectPair<string, string>
                            {
                                Key = language,
                                Value = "Options/Language/Description"
                            });

            SetOption(languages.IndexOf(localizer.GetCurrentLanguage()), true);

            localizer.SubscribeToLanguageChange(() => SetOption(languages.IndexOf(localizer.GetCurrentLanguage()),
                                                                true));
        }

        /// <summary>
        /// Called when the user selects a new language.
        /// </summary>
        /// <param name="isFirstSetup"></param>
        protected override void OnOptionSwitched(bool isFirstSetup)
        {
            if (localizer.GetCurrentLanguageIndex() == CurrentIndex) return;
            localizer.SetLanguage(languages[CurrentIndex]);
        }
    }
}