using UnityEngine;
using Varguiniano.YAPU.Runtime.Audio;
using Zenject;
using AudioConfiguration = Varguiniano.YAPU.Runtime.Audio.AudioConfiguration;

namespace Varguiniano.YAPU.Runtime.UI.Options
{
    /// <summary>
    /// Base class for a volume slider UI element.
    /// </summary>
    public abstract class VolumeSlider : OptionsMenuItem
    {
        /// <summary>
        /// Reference to the audio settings manager.
        /// </summary>
        private AudioSettingsManager audioSettingsManager;

        /// <summary>
        /// Cached reference to the configuration.
        /// </summary>
        private AudioConfiguration configuration;

        /// <summary>
        /// Initialize.
        /// </summary>
        [Inject]
        public void Construct(AudioSettingsManager audioSettingsManagerReference)
        {
            audioSettingsManager = audioSettingsManagerReference;

            configuration = audioSettingsManager.GetAudioConfiguration();

            UpdateValue(configuration);

            audioSettingsManager.ConfigurationUpdated += UpdateValue;
        }

        /// <summary>
        /// Update the value on the slider.
        /// </summary>
        /// <param name="newConfiguration"></param>
        private void UpdateValue(AudioConfiguration newConfiguration)
        {
            configuration = newConfiguration;
            SetOption(Mathf.RoundToInt(GetValueForSlider(configuration) * 10), true);
        }

        /// <summary>
        /// Called when the option is switched.
        /// </summary>
        /// <param name="isFirstSetup"></param>
        protected override void OnOptionSwitched(bool isFirstSetup)
        {
            if (CurrentIndex != Mathf.RoundToInt(GetValueForSlider(configuration) * 10))
                audioSettingsManager.SetAudioConfiguration(SetValueFromSlider(configuration, CurrentIndex / 10f));
        }

        /// <summary>
        /// Retrieve the value for the slider from the configuration.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        protected abstract float GetValueForSlider(AudioConfiguration configuration);

        /// <summary>
        /// Set the value from the slider into the configuration.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected abstract AudioConfiguration SetValueFromSlider(AudioConfiguration configuration, float value);
    }
}