using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Sounds
{
    /// <summary>
    /// Play the given sound.
    /// </summary>
    [Serializable]
    public class PlaySound : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Sound to play.
        /// </summary>
        [SerializeField]
        private AudioReference Sound;

        /// <summary>
        /// Loop the sound?
        /// </summary>
        [SerializeField]
        private bool Loop;

        /// <summary>
        /// Sound pitch.
        /// </summary>
        [SerializeField]
        [PropertyRange(-3, 3)]
        private float Pitch = 1;

        /// <summary>
        /// Sound volume.
        /// </summary>
        [SerializeField]
        [PropertyRange(0, 1)]
        private float Volume = 1;

        /// <summary>
        /// Fade in time.
        /// </summary>
        [SerializeField]
        private float FadeInTime;

        /// <summary>
        /// Prevent the sound from play is it is already playing.
        /// </summary>
        [SerializeField]
        private bool PreventDuplicates;

        /// <summary>
        /// Play the given sound.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            bool audioPlaying = false;

            yield return AudioManager.Instance.IsAudioPlaying(Sound,
                                                              isPlaying =>
                                                              {
                                                                  audioPlaying = isPlaying;
                                                              });

            if (PreventDuplicates && audioPlaying) yield break;

            AudioManager.Instance.PlayAudio(Sound, Loop, Pitch, Volume, FadeInTime);
        }
    }
}