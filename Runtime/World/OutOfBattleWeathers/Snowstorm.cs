using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.World.OutOfBattleWeathers
{
    /// <summary>
    /// Data class for Snowstorm weather outside battles.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Maps/Weathers/Snowstorm", fileName = "Snowstorm")]
    public class Snowstorm : OutOfBattleWeather
    {
        /// <summary>
        /// Audio for the animation.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

        /// <summary>
        /// Start this weather.
        /// </summary>
        public override IEnumerator StartWeather(PlayerCharacter playerCharacter)
        {
            AudioManager.Instance.PlayAudio(Audio, loop: true, fadeTime: .25f, volume: .25f);
            playerCharacter.FX.PlaySnowstormFX();

            yield break;
        }

        /// <summary>
        /// End this weather.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player's character.</param>
        /// <param name="isDestroyingCharacter">Is the character being destroyed?</param>
        public override IEnumerator EndWeather(PlayerCharacter playerCharacter, bool isDestroyingCharacter)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.StopAudio(Audio, fadeTime: .25f);
            playerCharacter.FX.PlaySnowstormFX(false);

            yield break;
        }
    }
}