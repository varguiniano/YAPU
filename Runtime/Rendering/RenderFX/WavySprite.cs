using System.Collections;
using DG.Tweening;
using UnityEngine;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.Rendering.RenderFX
{
    /// <summary>
    /// Behaviour for sprites that have a wavy effect.
    /// </summary>
    public class WavySprite : WhateverBehaviour<WavySprite>
    {
        /// <summary>
        /// Duration of the wave.
        /// </summary>
        [SerializeField]
        private float WaveDuration = 1;

        /// <summary>
        /// Min max waving.
        /// </summary>
        [SerializeField]
        private Vector2 MinMax = new(.85f, 1);

        /// <summary>
        /// Reference to this object's transform.
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
        /// Start waving.
        /// </summary>
        private void OnEnable() => StartCoroutine(Wave());

        /// <summary>
        /// Kill the tweens.
        /// </summary>
        private void OnDisable() => Transform.DOKill();

        /// <summary>
        /// Wave.
        /// </summary>
        private IEnumerator Wave()
        {
            bool finished = false;
            // ReSharper disable once AccessToModifiedClosure
            WaitUntil waitForFinished = new(() => finished);

            while (Transform != null)
            {
                finished = false;

                Transform.DOScaleX(MinMax.x, WaveDuration).SetEase(Ease.InOutSine).OnComplete(() => finished = true);

                yield return waitForFinished;

                if (Transform == null) yield break;

                finished = false;

                Transform.DOScaleX(MinMax.y, WaveDuration).SetEase(Ease.InOutSine).OnComplete(() => finished = true);

                yield return waitForFinished;
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}