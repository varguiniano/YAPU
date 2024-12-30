using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.GameFlow;
using WhateverDevs.Core.Runtime.Ui;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Time
{
    /// <summary>
    /// Class that keeps a text updated with the current time.
    /// </summary>
    public class Clock : EasyUpdateText<Clock>
    {
        /// <summary>
        /// Reference to the day moment icon.
        /// </summary>
        [SerializeField]
        private Image DayMomentIcon;

        /// <summary>
        /// Start updating the clock on enable?
        /// </summary>
        [SerializeField]
        private bool StartUpdatingOnEnable;

        /// <summary>
        /// Seconds between each clock update.
        /// </summary>
        [SerializeField]
        private float SecondsBetweenUpdates;

        /// <summary>
        /// Reference to the time manager.
        /// </summary>
        [Inject]
        private TimeManager timeManager;

        /// <summary>
        /// Flag to keep updating the clock.
        /// </summary>
        private bool updateClock;

        /// <summary>
        /// Reference to the player character.
        /// </summary>
        private PlayerCharacter playerCharacter;

        /// <summary>
        /// Start updating if configured.
        /// </summary>
        private void OnEnable()
        {
            if (StartUpdatingOnEnable) StartUpdating(null);
        }

        /// <summary>
        /// Start updating the clock.
        /// </summary>
        public void StartUpdating(PlayerCharacter playerCharacterReference)
        {
            playerCharacter = playerCharacterReference;

            if (updateClock) return;

            updateClock = true;
            StartCoroutine(UpdateClock());
        }

        /// <summary>
        /// Stop updating the lock.
        /// </summary>
        public void StopUpdating() => updateClock = false;

        /// <summary>
        /// Coroutine that updates the clock.
        /// </summary>
        private IEnumerator UpdateClock()
        {
            WaitForSeconds interval = new(SecondsBetweenUpdates);

            while (updateClock)
            {
                UpdateText(timeManager.Hour);
                DayMomentIcon.sprite = timeManager.DayMomentIcon;

                if (playerCharacter != null && playerCharacter.CurrentWeather != null)
                    DayMomentIcon.sprite = playerCharacter.CurrentWeather.Icon;

                yield return interval;
            }
        }
    }
}