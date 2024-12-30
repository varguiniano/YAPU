using Varguiniano.YAPU.Runtime.GameFlow;
using WhateverDevs.Core.Runtime.Configuration;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Options
{
    /// <summary>
    /// Configuration selector for the prompting the player for a nickname.
    /// </summary>
    public class ShowNicknameSelector : OptionsMenuItem
    {
        /// <summary>
        /// Reference to the configuration manager.
        /// </summary>
        private IConfigurationManager configurationManager;

        /// <summary>
        /// Get the current option.
        /// </summary>
        /// <param name="configurationManagerReference">Reference to the configuration manager.</param>
        [Inject]
        public void Construct(IConfigurationManager configurationManagerReference)
        {
            configurationManager = configurationManagerReference;

            if (!configurationManager.GetConfiguration(out GameplayConfiguration gameplayConfiguration))
            {
                Logger.Error("Couldn't retrieve gameplay configuration.");
                return;
            }

            SetOption(gameplayConfiguration.ShowNicknameDialog ? 0 : 1, true);
        }

        /// <summary>
        /// Update the option.
        /// </summary>
        /// <param name="isFirstSetup"></param>
        protected override void OnOptionSwitched(bool isFirstSetup)
        {
            if (!configurationManager.GetConfiguration(out GameplayConfiguration gameplayConfiguration))
            {
                Logger.Error("Couldn't retrieve gameplay configuration.");
                return;
            }

            gameplayConfiguration.ShowNicknameDialog = CurrentIndex == 0;

            if (!configurationManager.SetConfiguration(gameplayConfiguration))
                Logger.Error("Couldn't save configuration.");
        }
    }
}