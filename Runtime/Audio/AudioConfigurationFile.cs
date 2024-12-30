using System;
using UnityEngine;
using WhateverDevs.Core.Runtime.Configuration;

namespace Varguiniano.YAPU.Runtime.Audio
{
    /// <summary>
    /// Configuration holder for the audio configuration.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Audio/Config", fileName = "AudioConfiguration")]
    public class AudioConfigurationFile : ConfigurationScriptableHolderUsingFirstValidPersister<AudioConfiguration>
    {
    }

    /// <summary>
    /// Configuration for the audio.
    /// </summary>
    [Serializable]
    public class AudioConfiguration : ConfigurationData
    {
        /// <summary>
        /// Master volume for all the app.
        /// </summary>
        public float MasterVolume;

        /// <summary>
        /// Volume for the music sounds.
        /// </summary>
        public float MusicVolume;

        /// <summary>
        /// Volume for the FX sounds.
        /// </summary>
        public float FXVolume;

        /// <summary>
        /// Clone this data into a new instance of the same type.
        /// </summary>
        /// <typeparam name="TConfigurationData">Type of the configuration.</typeparam>
        /// <returns>The cloned config.</returns>
        protected override TConfigurationData Clone<TConfigurationData>() =>
            new AudioConfiguration
            {
                MasterVolume = MasterVolume,
                MusicVolume = MusicVolume,
                FXVolume = FXVolume
            } as TConfigurationData;
    }
}