using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Core.Runtime.Configuration;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Options
{
    /// <summary>
    /// Selector for the battle style.
    /// </summary>
    public class BattleStyleSelector : OptionsMenuItem
    {
        /// <summary>
        /// Reference to the configuration manager.
        /// </summary>
        private IConfigurationManager configurationManager;

        /// <summary>
        /// Initialize.
        /// </summary>
        [Inject]
        public void Construct(IConfigurationManager configurationManagerReference)
        {
            configurationManager = configurationManagerReference;

            if (!configurationManager.GetConfiguration(out BattleConfiguration configuration))
            {
                Logger.Error("Couldn't retrieve configuration.");
                return;
            }

            SetOption((int)configuration.BattleStyle, true);
        }

        /// <summary>
        /// Save the battle style.
        /// </summary>
        /// <param name="isFirstSetup"></param>
        protected override void OnOptionSwitched(bool isFirstSetup)
        {
            if (!configurationManager.GetConfiguration(out BattleConfiguration configuration))
            {
                Logger.Error("Couldn't retrieve configuration.");
                return;
            }

            configuration.BattleStyle = (BattleStyle)CurrentIndex;

            if (!configurationManager.SetConfiguration(configuration)) Logger.Error("Couldn't save configuration.");
        }
    }
}