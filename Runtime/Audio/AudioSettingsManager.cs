using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Configuration;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Audio
{
    /// <summary>
    /// Manager in charge of applying audio settings.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Audio/SettingsManager", fileName = "AudioSettingsManager")]
    public class AudioSettingsManager : WhateverScriptable<AudioSettingsManager>
    {
        /// <summary>
        /// Mixer group parameter name for master volume.
        /// </summary>
        [SerializeField]
        private string MasterVolumeParameterName;

        /// <summary>
        /// Mixer group parameter name for music volume.
        /// </summary>
        [SerializeField]
        private string MusicVolumeParameterName;

        /// <summary>
        /// Mixer group parameter name for FX volume.
        /// </summary>
        [SerializeField]
        private string FXVolumeParameterName;

        /// <summary>
        /// Event raised then the configuration gets updated.
        /// </summary>
        public Action<AudioConfiguration> ConfigurationUpdated;

        /// <summary>
        /// Audio mixer reference.
        /// </summary>
        private AudioMixer mainMixer;

        /// <summary>
        /// Reference to the configuration manager.
        /// </summary>
        private IConfigurationManager configurationManager;

        /// <summary>
        /// Apply the settings on the next frame.
        /// </summary>
        private void ApplySettingsOnNextFrame() =>
            CoroutineRunner.RunRoutine(ApplySettingsOnNextFrameRoutine());

        /// <summary>
        /// Apply the settings on the next frame.
        /// </summary>
        private IEnumerator ApplySettingsOnNextFrameRoutine()
        {
            yield return WaitAFrame;
            ApplySettings();
        }

        /// <summary>
        /// Inject the dependencies and initialize.
        /// Get the configuration and set the current app volume to that.
        /// </summary>
        /// <param name="configurationManagerReference">Reference to the configuration manager.</param>
        /// <param name="audioMixerReference">Reference to the audio mixer.</param>
        [Inject]
        public void Construct(IConfigurationManager configurationManagerReference, AudioMixer audioMixerReference)
        {
            configurationManager = configurationManagerReference;
            mainMixer = audioMixerReference;

            ApplySettingsOnNextFrame();
        }

        /// <summary>
        /// Retrieve the current audio configuration.
        /// </summary>
        /// <returns></returns>
        public AudioConfiguration GetAudioConfiguration()
        {
            if (configurationManager.GetConfiguration(out AudioConfiguration configuration)) return configuration;

            Logger.Error("Error retrieving audio configuration!");

            return null;
        }

        /// <summary>
        /// Set the new audio configuration.
        /// </summary>
        /// <param name="newConfiguration"></param>
        [FoldoutGroup("Debug")]
        [HideInEditorMode]
        [Button]
        public void SetAudioConfiguration(AudioConfiguration newConfiguration)
        {
            configurationManager.SetConfiguration(newConfiguration);

            ApplySettings();

            ConfigurationUpdated?.Invoke(newConfiguration);
        }

        /// <summary>
        /// Apply the audio settings to the mixer group.
        /// </summary>
        [FoldoutGroup("Debug")]
        [HideInEditorMode]
        [Button]
        private void ApplySettings()
        {
            if (configurationManager.GetConfiguration(out AudioConfiguration configuration))
                ApplySettings(configuration);
            else
                Logger.Error("Error retrieving audio configuration!");
        }

        /// <summary>
        /// Apply the audio settings to the mixer group.
        /// </summary>
        /// <param name="configuration"></param>
        private void ApplySettings(AudioConfiguration configuration)
        {
            if (mainMixer == null)
            {
                Logger.Error("Reference to the audio mixer is null!");
                return;
            }

            mainMixer.SetFloat(MasterVolumeParameterName,
                               IAudioManager.LinearVolumeToLogarithmicVolume(configuration.MasterVolume));

            mainMixer.SetFloat(MusicVolumeParameterName,
                               IAudioManager.LinearVolumeToLogarithmicVolume(configuration.MusicVolume));

            mainMixer.SetFloat(FXVolumeParameterName,
                               IAudioManager.LinearVolumeToLogarithmicVolume(configuration.FXVolume));
        }
    }
}