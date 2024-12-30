using System.Collections;
using DG.Tweening;
using UnityEngine;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.Animation.Moves
{
    /// <summary>
    /// Controller for a punch animation.
    /// </summary>
    public class Punch : WhateverBehaviour<Punch>
    {
        /// <summary>
        /// Reference to the fist.
        /// </summary>
        [SerializeField]
        private SpriteRenderer Fist;

        /// <summary>
        /// Reference to the hit.
        /// </summary>
        [SerializeField]
        private BasicSpriteAnimation Hit;

        /// <summary>
        /// Play the animation.
        /// </summary>
        /// <param name="duration">Duration of the animation.</param>
        /// <param name="speed">Battle speed.</param>
        public IEnumerator PlayAnimation(float duration, float speed)
        {
            Fist.DOFade(1, .1f / speed);

            yield return Fist.transform.DORotate(new Vector3(0, 0, 360), duration, RotateMode.FastBeyond360)
                             .SetRelative(true)
                             .WaitForCompletion();

            Fist.DOFade(0, .1f / speed);

            yield return Hit.PlayAnimation(speed, true);
        }
    }
}