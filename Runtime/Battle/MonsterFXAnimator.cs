using System;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Animator that controls FX that are directly played on the monster.
    /// </summary>
    public class MonsterFXAnimator : WhateverBehaviour<MonsterFXAnimator>
    {
        /// <summary>
        /// Reference to the lower stat FX.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private VisualEffect LowerStat;

        /// <summary>
        /// Reference to the rise stat FX.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private VisualEffect RiseStat;

        /// <summary>
        /// Reference to a generic boost.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private VisualEffect Boost;

        /// <summary>
        /// Reference to the absorb fx.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private VisualEffect Absorb;

        /// <summary>
        /// Reference to the Form switch fx.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private VisualEffect FormSwitch;

        /// <summary>
        /// Duration of the lower stat effect.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private float ChangeStatDuration = 1.5f;

        /// <summary>
        /// Duration of the boost effect.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private float BoostDuration;

        /// <summary>
        /// Reference to the lower stat audio.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private AudioReference LowerStatAudio;

        /// <summary>
        /// Reference to the rise stat audio.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private AudioReference RiseStatAudio;

        /// <summary>
        /// Default rise stat animation texture.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private Texture2D DefaultRiseStatTexture;

        /// <summary>
        /// Audio for the boost effect.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private AudioReference BoostAudio;

        /// <summary>
        /// Default boost animation texture.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private Texture2D DefaultBoostTexture;

        /// <summary>
        /// Default absorb animation texture.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private Texture2D DefaultAbsorbTexture;

        /// <summary>
        /// Default absorb animation curve.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private AnimationCurve DefaultAbsorbSizeOverLifetime;

        /// <summary>
        /// Default particle color.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private Gradient DefaultParticleColor;

        /// <summary>
        /// Audio for the form change animation.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private AudioReference FormChangeAudio;

        /// <summary>
        /// Reference to the audio manager.
        /// </summary>
        [Inject]
        private IAudioManager audioManager;

        /// <summary>
        /// Plays the lower stat FX.
        /// </summary>
        /// <param name="speed">Speed of the animation.</param>
        /// <param name="finished">Callback when finished.</param>
        [FoldoutGroup("Debug")]
        [Button]
        [HideInEditorMode]
        public void PlayLowerStat(float speed, Action finished)
        {
            LowerStat.enabled = true;

            audioManager.PlayAudio(LowerStatAudio, pitch: speed);

            DOVirtual.DelayedCall(ChangeStatDuration / speed,
                                  () =>
                                  {
                                      LowerStat.enabled = false;
                                      finished?.Invoke();
                                  });
        }

        /// <summary>
        /// Plays the rise stat FX.
        /// </summary>
        /// <param name="speed">Speed of the animation.</param>
        /// <param name="finished">Callback when finished.</param>
        /// <param name="particleOverride">Override for the particle texture.</param>
        /// <param name="scaleOverride">Override for the scale of the particles.</param>
        /// <param name="playAudio">Play the audio?</param>
        [FoldoutGroup("Debug")]
        [Button]
        [HideInEditorMode]
        public void PlayRiseStat(float speed,
                                 Action finished,
                                 Texture2D particleOverride = null,
                                 Vector3 scaleOverride = default,
                                 bool playAudio = true)
        {
            RiseStat.SetTexture("ParticleTexture",
                                particleOverride != null ? particleOverride : DefaultRiseStatTexture);

            RiseStat.SetVector3("Scale", scaleOverride != default ? scaleOverride : Vector3.one);

            RiseStat.enabled = true;

            if (playAudio) audioManager.PlayAudio(RiseStatAudio, pitch: speed);

            DOVirtual.DelayedCall(ChangeStatDuration / speed,
                                  () =>
                                  {
                                      RiseStat.enabled = false;
                                      finished?.Invoke();
                                  });
        }

        /// <summary>
        /// Plays a burst of FX useful for boosts.
        /// </summary>
        /// <param name="speed">Speed to play the audio of the burst.</param>
        /// <param name="playAudio">Should we play audio.</param>
        /// <param name="particleOverride">Override for the particle texture.</param>
        /// <param name="finished">Callback when finished.</param>
        public IEnumerator PlayBoostRoutine(float speed,
                                            bool playAudio = true,
                                            Texture2D particleOverride = null,
                                            Action finished = null)
        {
            bool animationEnded = false;

            PlayBoost(speed,
                      playAudio,
                      particleOverride,
                      () =>
                      {
                          animationEnded = true;
                          finished?.Invoke();
                      });

            yield return new WaitUntil(() => animationEnded);
        }

        /// <summary>
        /// Plays a burst of FX useful for boosts.
        /// </summary>
        /// <param name="speed">Speed to play the audio of the burst.</param>
        /// <param name="playAudio">Should we play audio.</param>
        /// <param name="particleOverride">Override for the particle texture.</param>
        /// <param name="finished">Callback when finished.</param>
        [FoldoutGroup("Debug")]
        [Button]
        [HideInEditorMode]
        public void PlayBoost(float speed,
                              bool playAudio = true,
                              Texture2D particleOverride = null,
                              Action finished = null)
        {
            Boost.SetTexture("MainTexture", particleOverride != null ? particleOverride : DefaultBoostTexture);

            Boost.enabled = true;

            if (playAudio) audioManager.PlayAudio(BoostAudio, pitch: speed);

            DOVirtual.DelayedCall(BoostDuration / speed,
                                  () =>
                                  {
                                      Boost.Stop();
                                      finished?.Invoke();

                                      DOVirtual.DelayedCall(1f, () => Boost.enabled = false);
                                  });
        }

        /// <summary>
        /// Play the Absorb FX.
        /// </summary>
        /// <param name="duration">Duration of the FX. -1: infinite.</param>
        /// <param name="targetPosition">Target position for the particles.</param>
        /// <param name="spawnCenter">Center of the spawn.</param>
        /// <param name="spawnAngle">Angles at which to start and stop spawning (rads).</param>
        /// <param name="spawnRadius">Spawn radius of the particle.</param>
        /// <param name="sizeOverLifetime">Size of the particles over lifetime.</param>
        /// <param name="spawnRate">Rate at which to spawn the particles.</param>
        /// <param name="particleTexture">Override the particle's texture.</param>
        /// <param name="particleColor">Override the particle's color.</param>
        /// <param name="finished">Callback when the FX is finished.</param>
        public IEnumerator PlayAbsorb(float duration,
                                      Vector3 targetPosition,
                                      Transform spawnCenter = null,
                                      Vector2 spawnAngle = default,
                                      float spawnRadius = 1,
                                      AnimationCurve sizeOverLifetime = null,
                                      float spawnRate = 50,
                                      Texture2D particleTexture = null,
                                      Gradient particleColor = null,
                                      Action finished = null)
        {
            if (spawnAngle == default) spawnAngle = new Vector2(0, 6.28f);

            if (spawnCenter != null)
                Absorb.transform.position = spawnCenter.position;
            else
                Absorb.transform.localPosition = Vector3.zero;

            Absorb.SetTexture("MainTexture", particleTexture != null ? particleTexture : DefaultAbsorbTexture);

            Absorb.SetFloat("SpawnRate", spawnRate);
            Absorb.SetVector3("Target", Absorb.transform.InverseTransformPoint(targetPosition));
            Absorb.SetVector2("SpawnAngle", spawnAngle);
            Absorb.SetFloat("SpawnRadius", spawnRadius);
            Absorb.SetAnimationCurve("LifeOverTime", sizeOverLifetime ?? DefaultAbsorbSizeOverLifetime);
            Absorb.SetGradient("ParticleColor", particleColor ?? DefaultParticleColor);

            Absorb.enabled = true;

            if (Math.Abs(duration - -1) < 0.05f) yield break;

            yield return new WaitForSeconds(duration);

            StartCoroutine(StopAbsorb());

            finished?.Invoke();
        }

        /// <summary>
        /// Stop the absorb FX.
        /// </summary>
        public IEnumerator StopAbsorb()
        {
            Absorb.Stop();

            yield return new WaitForSeconds(3);

            Absorb.enabled = false;
        }

        /// <summary>
        /// Play the form change animation.
        /// </summary>
        public IEnumerator PlayFormChange(float speed)
        {
            audioManager.PlayAudio(FormChangeAudio, pitch: speed);

            FormSwitch.EnableAndPlay();

            yield return new WaitForSeconds(3.5f / speed);

            FormSwitch.Stop();
        }
    }
}