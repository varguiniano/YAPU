using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.Animation.Moves
{
    /// <summary>
    /// Controller for a big flames animation.
    /// </summary>
    public class BigFlames : WhateverBehaviour<BigFlames>
    {
        /// <summary>
        /// Total duration of the animation.
        /// </summary>
        [SerializeField]
        private float TotalDuration;

        /// <summary>
        /// List of flames in the animation.
        /// </summary>
        [SerializeField]
        private List<BasicSpriteAnimation> Flames;

        /// <summary>
        /// Play the animation.
        /// </summary>
        public IEnumerator PlayAnimation(float speed)
        {
            WaitForSeconds interval = new(TotalDuration / speed / Flames.Count);

            foreach (BasicSpriteAnimation flame in Flames)
            {
                StartCoroutine(flame.PlayAnimation(speed,
                                                   finished: () => flame.GetCachedComponent<SpriteRenderer>().enabled =
                                                                       false));

                yield return interval;
            }
        }
    }
}