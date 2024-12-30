using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.UI.Monsters
{
    /// <summary>
    /// Controller for the slider that shows the health of a monster.
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public class HealthSlider : TweenableSlider<HealthSlider>
    {
        /// <summary>
        /// Colors the slider should have by percentage.
        /// </summary>
        [SerializeField]
        private SerializableDictionary<float, Color> ColorsByPercentage;

        /// <summary>
        /// Reference to the slider's fill area.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Image FillArea;

        /// <summary>
        /// Reference to the monster panel.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private MonsterPanelBase MonsterPanel;

        /// <summary>
        /// Play a warning audio when too low?
        /// </summary>
        [FoldoutGroup("Audio")]
        public bool PlayWarningAudio;

        /// <summary>
        /// Level to play an audio warning at.
        /// </summary>
        [FoldoutGroup("Audio")]
        [SerializeField]
        [ShowIf(nameof(PlayWarningAudio))]
        private float WarningLevel;

        /// <summary>
        /// Sound to play on the warning level.
        /// </summary>
        [FoldoutGroup("Audio")]
        [SerializeField]
        [ShowIf(nameof(PlayWarningAudio))]
        private AudioReference WarningSound;

        /// <summary>
        /// Value of the lowest threshold.
        /// </summary>
        private float lowestThreshold;

        /// <summary>
        /// Get the lowest threshold.
        /// </summary>
        private void OnEnable()
        {
            lowestThreshold = 1000;

            foreach ((float threshold, Color _) in ColorsByPercentage)
                lowestThreshold = Mathf.Min(lowestThreshold, threshold);
        }

        /// <summary>
        /// Update the color of the health bar.
        /// </summary>
        protected override void SliderUpdate()
        {
            float percentage = Slider.value / Slider.maxValue;

            MonsterPanel.UpdateHealthText((uint)Slider.value, (uint)Slider.maxValue);

            foreach ((float threshold, Color color) in ColorsByPercentage)
                if (percentage > threshold)
                {
                    FillArea.color = color;
                    return;
                }
        }

        /// <summary>
        /// Called when the slider has finished updating.
        /// </summary>
        protected override void SliderUpdated()
        {
            float percentage = Slider.value / Slider.maxValue;

            if (PlayWarningAudio && percentage > 0 && percentage <= WarningLevel)
                AudioManager.Instance.PlayAudio(WarningSound);
        }
    }
}