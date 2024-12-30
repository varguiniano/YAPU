using System.Collections;
using DG.Tweening;
using UnityEngine;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.Animation.Moves
{
    /// <summary>
    /// Class controlling an animation for two eyes.
    /// </summary>
    public class EyesAnimation : WhateverBehaviour<EyesAnimation>
    {
        /// <summary>
        /// Eye icons.
        /// </summary>
        [SerializeField]
        private SpriteRenderer EyeLeft;

        /// <summary>
        /// Eye icons.
        /// </summary>
        [SerializeField]
        private SpriteRenderer EyeRight;

        /// <summary>
        /// Play the animation.
        /// </summary>
        /// <param name="speed">Battle speed.</param>
        public IEnumerator PlayAnimation(float speed)
        {
            EyeLeft.DOFade(1, .4f / speed);
            yield return EyeRight.DOFade(1, .4f / speed).WaitForCompletion();

            yield return new WaitForSeconds(.7f / speed);

            EyeLeft.DOFade(0, .1f / speed);
            EyeRight.DOFade(0, .1f / speed);
        }
    }
}