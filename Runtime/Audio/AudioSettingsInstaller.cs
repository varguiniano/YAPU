using UnityEngine;
using UnityEngine.Audio;
using WhateverDevs.Core.Runtime.DependencyInjection;

namespace Varguiniano.YAPU.Runtime.Audio
{
    /// <summary>
    /// Installer for the audio settings manager.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Dependency Injection/AudioSettingsInstaller",
                     fileName = "AudioSettingsInstaller")]
    public class AudioSettingsInstaller : LazySingletonScriptableInstaller<AudioSettingsManager>
    {
        /// <summary>
        /// Reference to the audio mixer.
        /// </summary>
        [SerializeField]
        private AudioMixer Mixer;

        /// <summary>
        /// Also inject the audio mixer.
        /// </summary>
        public override void InstallBindings()
        {
            base.InstallBindings();

            Container.Bind<AudioMixer>().FromInstance(Mixer).WhenInjectedInto<AudioSettingsManager>().Lazy();
        }
    }
}