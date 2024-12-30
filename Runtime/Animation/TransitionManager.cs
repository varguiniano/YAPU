using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using WhateverDevs.Core.Runtime.Common;

namespace Varguiniano.YAPU.Runtime.Animation
{
    /// <summary>
    /// Manager to animate transitions between screens.
    /// </summary>
    public class TransitionManager : Singleton<TransitionManager>
    {
        /// <summary>
        /// Reference to the black screen image.
        /// </summary>
        [FoldoutGroup("Basic Black Screen")]
        public Image BlackScreen;

        /// <summary>
        /// Duration of the basic black screen fade.
        /// </summary>
        [FoldoutGroup("Basic Black Screen")]
        [SerializeField]
        private float BlackScreenFadeDuration;

        /// <summary>
        /// Fade in the black screen.
        /// </summary>
        public static void BlackScreenFadeIn(float overrideDuration = -1) =>
            Instance.BlackScreenFadeInInternal(overrideDuration);

        /// <summary>
        /// Fade in the black screen.
        /// </summary>
        private void BlackScreenFadeInInternal(float overrideDuration) =>
            StartCoroutine(BlackScreenFadeInRoutineInternal(overrideDuration));

        /// <summary>
        /// Fade in the black screen.
        /// </summary>
        public static IEnumerator BlackScreenFadeInRoutine(float overrideDuration = -1)
        {
            yield return Instance.BlackScreenFadeInRoutineInternal(overrideDuration);
        }

        /// <summary>
        /// Fade in the black screen.
        /// </summary>
        private IEnumerator BlackScreenFadeInRoutineInternal(float overrideDuration)
        {
            if (BlackScreen.color.a < 1)
                yield return BlackScreen.DOFade(1, overrideDuration <= 0 ? BlackScreenFadeDuration : overrideDuration)
                                        .WaitForCompletion();
        }

        /// <summary>
        /// Fade out the black screen.
        /// </summary>
        public static void BlackScreenFadeOut(float overrideDuration = -1) =>
            Instance.BlackScreenFadeOutInternal(overrideDuration);

        /// <summary>
        /// Fade out the black screen.
        /// </summary>
        private void BlackScreenFadeOutInternal(float overrideDuration) =>
            StartCoroutine(BlackScreenFadeOutRoutineInternal(overrideDuration));

        /// <summary>
        /// Fade out the black screen.
        /// </summary>
        public static IEnumerator BlackScreenFadeOutRoutine(float overrideDuration = -1)
        {
            yield return Instance.BlackScreenFadeOutRoutineInternal(overrideDuration);
        }

        /// <summary>
        /// Fade out the black screen.
        /// </summary>
        private IEnumerator BlackScreenFadeOutRoutineInternal(float overrideDuration)
        {
            if (BlackScreen.color.a > 0)
                yield return BlackScreen.DOFade(0, overrideDuration <= 0 ? BlackScreenFadeDuration : overrideDuration)
                                        .WaitForCompletion();
        }
    }
}