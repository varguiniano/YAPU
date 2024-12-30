using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.Animation.Moves
{
    /// <summary>
    /// Animation controller for moves that launch stuff at the opponent.
    /// </summary>
    public class LaunchThingsAnimation : WhateverBehaviour<LaunchThingsAnimation>
    {
        /// <summary>
        /// things to throw.
        /// </summary>
        [SerializeField]
        private List<SpriteRenderer> Launchables;

        /// <summary>
        /// Fade in time.
        /// </summary>
        [SerializeField]
        private float FadeInTime = 1.3f;

        /// <summary>
        /// Throw duration.
        /// </summary>
        [SerializeField]
        private float ThrowTime = .8f;

        /// <summary>
        /// Play the animation.
        /// </summary>
        /// <param name="targetPosition">Position of the launchables to throw at.</param>
        /// <param name="speed">Battle speed.</param>
        public IEnumerator PlayAnimation(Vector3 targetPosition, float speed)
        {
            float fadeDuration = FadeInTime / speed;

            foreach (SpriteRenderer spriteRenderer in Launchables) spriteRenderer.DOFade(1, fadeDuration);

            yield return new WaitForSeconds(fadeDuration);

            float throwDuration = ThrowTime / speed;

            foreach (Transform launchable in Launchables.Select(rock => rock.transform))
                launchable.DOMove(targetPosition + launchable.localPosition * .25f, throwDuration);

            yield return new WaitForSeconds(throwDuration);

            foreach (SpriteRenderer spriteRenderer in Launchables) spriteRenderer.DOFade(0, .1f / speed);
        }
    }
}