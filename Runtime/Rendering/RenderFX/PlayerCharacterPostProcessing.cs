using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.World;
using WhateverDevs.Core.Behaviours;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Rendering.RenderFX
{
    /// <summary>
    /// Controller for the post processing FX that can affect the player character.
    /// </summary>
    public class PlayerCharacterPostProcessing : WhateverBehaviour<PlayerCharacterPostProcessing>
    {
        /// <summary>
        /// Post processing volume for the day-night cycle.
        /// </summary>
        [FoldoutGroup("Day-Night cycle")]
        [SerializeField]
        private Volume DayNightVolume;

        /// <summary>
        /// Gradient to use for the color adjustment of the day-night cycle.
        /// </summary>
        [FoldoutGroup("Day-Night cycle")]
        [SerializeField]
        private Gradient DayNightGradient;
        
        /// <summary>
        /// Reference to the weather volume.
        /// </summary>
        [FoldoutGroup("Weather")]
        public Volume WeatherVolume;

        /// <summary>
        /// Post processing volume for dark caves.
        /// </summary>
        [FoldoutGroup("Dark caves")]
        [SerializeField]
        private Volume DarkCavesVolume;

        /// <summary>
        /// Vignette value when the cave is dark.
        /// </summary>
        [FoldoutGroup("Dark caves")]
        [SerializeField]
        private float DarkVignetteValue = 1;

        /// <summary>
        /// Vignette value when the player lit the cave.
        /// </summary>
        [FoldoutGroup("Dark caves")]
        [SerializeField]
        private float LitVignetteValue = .2f;

        /// <summary>
        /// Reference to the time manager.
        /// </summary>
        [Inject]
        private TimeManager timeManager;

        /// <summary>
        /// Reference to the player character.
        /// </summary>
        private PlayerCharacter playerCharacter;

        /// <summary>
        /// Cached reference to the color adjustments effect that the day-night cycle uses.
        /// </summary>
        private ColorAdjustments dayNightColor;

        /// <summary>
        /// Cached reference to the vignette effect that the dark caves use.
        /// </summary>
        private Vignette darkCaveVignette;

        /// <summary>
        /// Get the FX.
        /// </summary>
        public void Init(PlayerCharacter playerCharacterReference)
        {
            playerCharacter = playerCharacterReference;

            if (!DayNightVolume.sharedProfile.TryGet(out dayNightColor))
                Logger.Error("Couldn't retrieve day-night color effect.");

            if (!DarkCavesVolume.sharedProfile.TryGet(out darkCaveVignette))
                Logger.Error("Couldn't retrieve dark caves vignette effect.");
        }

        /// <summary>
        /// Leave the FX as they were.
        /// </summary>
        private void OnDisable()
        {
            ResetDayNightColor();
            DisableDarkCave();
        }

        /// <summary>
        /// Update the day night color adjustments.
        /// </summary>
        private void Update()
        {
            if (playerCharacter == null || playerCharacter.CharacterController.CurrentGrid == null) return;

            SetDayNightColor(timeManager.DayProgress, playerCharacter.Scene);
        }

        /// <summary>
        /// Set the color adjustment according to the time of day and the current scene.
        /// </summary>
        /// <param name="dayProgress">Moment of the day, between 0 and 1.</param>
        /// <param name="currentScene">Current scene the character is in.</param>
        [FoldoutGroup("Day-Night cycle")]
        [Button]
        [HideInEditorMode]
        public void SetDayNightColor(float dayProgress, SceneInfo currentScene) =>
            dayNightColor.colorFilter.SetValue(new ColorParameter(DayNightGradient.Evaluate(currentScene
                                                                     .IsAffectedBySky
                                                                      ? dayProgress
                                                                      : .5f)));

        /// <summary>
        /// Reset the day night color to its normal value.
        /// </summary>
        private void ResetDayNightColor() =>
            dayNightColor.colorFilter.SetValue(new ColorParameter(DayNightGradient.Evaluate(.5f)));

        /// <summary>
        /// Set the post processing to show a dark cave.
        /// </summary>
        /// <param name="lit">Is the player lighting the cave?</param>
        [FoldoutGroup("Dark caves")]
        [Button]
        [HideInEditorMode]
        public void EnableDarkCave(bool lit) =>
            darkCaveVignette.intensity.SetValue(new FloatParameter(lit ? LitVignetteValue : DarkVignetteValue));

        /// <summary>
        /// Disable the dark cave post processing.
        /// </summary>
        [FoldoutGroup("Dark caves")]
        [Button]
        [HideInEditorMode]
        public void DisableDarkCave() => darkCaveVignette.intensity.SetValue(new FloatParameter(0));

        /// <summary>
        /// Reduce the dark cave FX intensity.
        /// </summary>
        [FoldoutGroup("Dark caves")]
        [Button("Lit Dark Cave")]
        [HideInEditorMode]
        private void TestLitDarkCave() => StartCoroutine(LitDarkCave());

        /// <summary>
        /// Reduce the dark cave FX intensity.
        /// </summary>
        public IEnumerator LitDarkCave()
        {
            float currentValue = DarkVignetteValue;

            yield return DOTween.To(() => currentValue,
                                    x => currentValue = x,
                                    LitVignetteValue,
                                    .5f)
                                .OnUpdate(() =>
                                          {
                                              darkCaveVignette.intensity.SetValue(new FloatParameter(currentValue));
                                          })
                                .WaitForCompletion();
        }
    }
}