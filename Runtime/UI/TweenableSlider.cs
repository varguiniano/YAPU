using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.UI
{
    /// <summary>
    /// Behaviour to control a tweenable slider.
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public class TweenableSlider : TweenableSlider<TweenableSlider>
    {
    }

    /// <summary>
    /// Behaviour to control a tweenable slider.
    /// </summary>
    /// <typeparam name="T">Loggable type.</typeparam>
    [RequireComponent(typeof(Slider))]
    public class TweenableSlider<T> : WhateverBehaviour<T> where T : TweenableSlider<T>
    {
        /// <summary>
        /// Duration of the animation.
        /// </summary>
        [FoldoutGroup("Config")]
        [SerializeField]
        private float Duration = 2f;

        /// <summary>
        /// Play a sound when decreasing.
        /// </summary>
        [FoldoutGroup("Audio")]
        [SerializeField]
        private bool PlaySoundOnDecrease;

        /// <summary>
        /// Sound to play when decreasing.
        /// </summary>
        [FoldoutGroup("Audio")]
        [SerializeField]
        [ShowIf(nameof(PlaySoundOnDecrease))]
        private AudioReference DecreaseSound;

        /// <summary>
        /// Play a sound when increasing.
        /// </summary>
        [FoldoutGroup("Audio")]
        [SerializeField]
        private bool PlaySoundOnIncrease;

        /// <summary>
        /// Sound to play when increasing.
        /// </summary>
        [FoldoutGroup("Audio")]
        [SerializeField]
        [ShowIf(nameof(PlaySoundOnIncrease))]
        private AudioReference IncreaseSound;

        /// <summary>
        /// Duration of the increasing sound.
        /// </summary>
        [FoldoutGroup("Audio")]
        [SerializeField]
        [ShowIf(nameof(PlaySoundOnIncrease))]
        private float IncreaseSoundDuration = 2f;

        /// <summary>
        /// Reference to this gameobject's slider.
        /// </summary>
        protected Slider Slider => GetCachedComponent<Slider>();

        /// <summary>
        /// Set the slider's value.
        /// </summary>
        /// <param name="speed">Speed at which to play the animation.</param>
        /// <param name="newValue">New value of the slider.</param>
        /// <param name="maxValue">Max value of the slider.</param>
        /// <param name="tween">Should tween from previous?</param>
        /// <param name="finished">Callback when finished.</param>
        [Button]
        [HideInEditorMode]
        public void SetValue(float speed, float newValue, float maxValue, bool tween = false, Action finished = null)
        {
            Slider.maxValue = maxValue;

            if (tween)
            {
                if (PlaySoundOnDecrease && Slider.value > newValue)
                    AudioManager.Instance.PlayAudio(DecreaseSound, pitch: speed);

                if (PlaySoundOnIncrease && Slider.value < newValue)
                    AudioManager.Instance.PlayAudio(IncreaseSound, pitch: IncreaseSoundDuration / Duration * speed);

                Slider.DOValue(newValue, Duration / speed)
                      .OnUpdate(SliderUpdate)
                      .OnComplete(() =>
                                  {
                                      SliderUpdated();
                                      finished?.Invoke();
                                  });
            }
            else
            {
                Slider.value = newValue;
                SliderUpdate();
                SliderUpdated();
                finished?.Invoke();
            }
        }

        /// <summary>
        /// Called when the slider updates.
        /// </summary>
        protected virtual void SliderUpdate()
        {
        }

        /// <summary>
        /// Called when the slider has finished updating.
        /// </summary>
        protected virtual void SliderUpdated()
        {
        }
    }
}