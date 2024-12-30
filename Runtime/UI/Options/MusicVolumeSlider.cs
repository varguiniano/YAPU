using Varguiniano.YAPU.Runtime.Audio;

namespace Varguiniano.YAPU.Runtime.UI.Options
{
    /// <summary>
    /// Controller for the music volume slider.
    /// </summary>
    public class MusicVolumeSlider : VolumeSlider
    {
        /// <summary>
        /// Get the config value for the slider.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        protected override float GetValueForSlider(AudioConfiguration configuration) => configuration.MusicVolume;

        /// <summary>
        /// Set the config value from the slider.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override AudioConfiguration SetValueFromSlider(AudioConfiguration configuration, float value)
        {
            configuration.MusicVolume = value;
            return configuration;
        }
    }
}