using Varguiniano.YAPU.Runtime.GameFlow;
using WhateverDevs.Core.Runtime.Configuration;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Options
{
    /// <summary>
    /// Configuration selector for the prompting the player to send new monsters to storage.
    /// </summary>
    public class AutoStorageSelector : OptionsMenuItem
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

            SetOption(gameplayConfiguration.AutoSendToStorage ? 1 : 0, true);
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

            gameplayConfiguration.AutoSendToStorage = CurrentIndex == 1;

            if (!configurationManager.SetConfiguration(gameplayConfiguration))
                Logger.Error("Couldn't save configuration.");
        }
    }
}