using System.Collections;
using DG.Tweening;
using UnityEngine;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.UI.Map
{
    /// <summary>
    /// Controller for the animation of the map cursor.
    /// </summary>
    public class MapCursorAnimation : WhateverBehaviour<MapCursorAnimation>
    {
        /// <summary>
        /// Translation.
        /// </summary>
        [SerializeField]
        private float Translation;

        /// <summary>
        /// Translation duration.
        /// </summary>
        [SerializeField]
        private float TranslationDuration;

        /// <summary>
        /// Flip interval.
        /// </summary>
        [SerializeField]
        private float FlipInterval;

        /// <summary>
        /// Cached reference to the attached transform.
        /// </summary>
        private Transform Transform
        {
            get
            {
                if (transformReference == null) transformReference = transform;
                return transformReference;
            }
        }

        /// <summary>
        /// Backfield for Transform.
        /// </summary>
        private Transform transformReference;

        /// <summary>
        /// Start the routines.
        /// </summary>
        private void OnEnable()
        {
            StartCoroutine(TranslateAnimation());
            StartCoroutine(FlipAnimation());
        }

        /// <summary>
        /// Kill the tweens.
        /// </summary>
        private void OnDisable() => Transform.DOKill();

        /// <summary>
        /// Cursor translate animation.
        /// </summary>
        /// <returns></returns>
        private IEnumerator TranslateAnimation()
        {
            Vector3 originalPosition = Transform.localPosition;
            Vector3 up = Transform.up;

            while (true)
            {
                yield return Transform.DOLocalMove(originalPosition + up * Translation, TranslationDuration)
                                      .SetEase(Ease.InOutQuad)
                                      .WaitForCompletion();

                yield return Transform.DOLocalMove(originalPosition + up * -Translation, TranslationDuration)
                                      .SetEase(Ease.InOutQuad)
                                      .WaitForCompletion();
            }
            // ReSharper disable once IteratorNeverReturns
        }

        /// <summary>
        /// Cursor flip animation.
        /// </summary>
        private IEnumerator FlipAnimation()
        {
            while (true)
            {
                yield return new WaitForSeconds(FlipInterval);

                yield return Transform.DOLocalRotate(new Vector3(0, 360, 0), .3f, RotateMode.LocalAxisAdd)
                                      .SetRelative(true)
                                      .WaitForCompletion();
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}