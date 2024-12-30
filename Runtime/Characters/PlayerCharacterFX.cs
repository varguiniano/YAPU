using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Characters
{
    /// <summary>
    /// Controller for the player character FX.
    /// </summary>
    public class PlayerCharacterFX : WhateverBehaviour<PlayerCharacterFX>
    {
        /// <summary>
        /// Reference to the rock particles.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private VisualEffect RockParticles;

        /// <summary>
        /// Reference to the dust particles.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private VisualEffect DustParticles;

        /// <summary>
        /// Reference to the climb audio.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private AudioReference ClimbAudio;

        /// <summary>
        /// Reference to the fog sprite.
        /// </summary>
        [FoldoutGroup("Weather")]
        [SerializeField]
        private SpriteRenderer Fog;

        /// <summary>
        /// Reference to the sandstorm FX.
        /// </summary>
        [FoldoutGroup("Weather")]
        [SerializeField]
        private VisualEffect Sandstorm;
        
        /// <summary>
        /// Reference to the snowstorm FX.
        /// </summary>
        [FoldoutGroup("Weather")]
        [SerializeField]
        private VisualEffect Snowstorm;

        /// <summary>
        /// Reference to the rain FX prefab.
        /// </summary>
        [FoldoutGroup("Weather")]
        [SerializeField]
        private VisualEffect RainFX;

        /// <summary>
        /// Reference to the audio manager.
        /// </summary>
        [Inject]
        private IAudioManager audioManager;

        /// <summary>
        /// Show or hide the climbing particles.
        /// </summary>
        /// <param name="show">show or hide.</param>
        [FoldoutGroup("Debug")]
        [Button]
        [HideInEditorMode]
        public void ShowClimbingParticles(bool show = true)
        {
            if (show)
            {
                audioManager.PlayAudio(ClimbAudio);

                RockParticles.EnableAndPlay();
                DustParticles.EnableAndPlay();
            }
            else
            {
                RockParticles.Stop();
                DustParticles.Stop();

                audioManager.StopAudio(ClimbAudio, 1);
            }
        }

        /// <summary>
        /// Display a fog in the world?
        /// </summary>
        /// <param name="show">Display or hide?</param>
        [FoldoutGroup("Debug")]
        [Button]
        [HideInEditorMode]
        public IEnumerator ShowFog(bool show = true)
        {
            yield return Fog.DOFade(show ? 1 : 0, 2f).WaitForCompletion();
        }

        /// <summary>
        /// Play or stop the sandstorm FX.
        /// </summary>
        /// <param name="play">Play or stop?</param>
        public void PlaySandstormFX(bool play = true)
        {
            if (play)
                Sandstorm.EnableAndPlay();
            else
                Sandstorm.Stop();
        }
        
        /// <summary>
        /// Play or stop the snowstorm FX.
        /// </summary>
        /// <param name="play">Play or stop?</param>
        public void PlaySnowstormFX(bool play = true)
        {
            if (play)
                Snowstorm.EnableAndPlay();
            else
                Snowstorm.Stop();
        }
        
        /// <summary>
        /// Play or stop the rain FX.
        /// </summary>
        /// <param name="play">Play or stop?</param>
        public void PlayRainFX(bool play = true)
        {
            if (play)
                RainFX.EnableAndPlay();
            else
                RainFX.Stop();
        }
    }
}