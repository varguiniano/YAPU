using System.Collections;
using DG.Tweening;
using UnityEngine;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.Animation.Moves
{
    /// <summary>
    /// Class controlling an animation for an angry monster.
    /// </summary>
    public class AngryAnimation : WhateverBehaviour<AngryAnimation>
    {
        /// <summary>
        /// Angry icons.
        /// </summary>
        [SerializeField]
        private SpriteRenderer AngryLeft;

        /// <summary>
        /// Angry icons.
        /// </summary>
        [SerializeField]
        private SpriteRenderer AngryRight;

        /// <summary>
        /// Play the animation.
        /// </summary>
        /// <param name="speed">Battle speed.</param>
        public IEnumerator PlayAnimation(float speed)
        {
            yield return AngryLeft.DOFade(1, .4f / speed).WaitForCompletion();

            yield return new WaitForSeconds(.2f / speed);

            yield return AngryRight.DOFade(1, .4f / speed).WaitForCompletion();

            yield return new WaitForSeconds(.2f / speed);

            AngryLeft.DOFade(0, .1f / speed);
            AngryRight.DOFade(0, .1f / speed);
        }
    }
}