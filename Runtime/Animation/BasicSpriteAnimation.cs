using System;
using System.Collections;
using UnityEngine;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.Animation
{
    /// <summary>
    /// Behaviour for a very simple sprite animation that changes sprites along time.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class BasicSpriteAnimation : WhateverBehaviour<BasicSpriteAnimation>
    {
        /// <summary>
        /// Sprites to use in the animation.
        /// </summary>
        [SerializeField]
        private Sprite[] Sprites;

        /// <summary>
        /// Duration of the animation.
        /// </summary>
        [SerializeField]
        private float Duration;

        /// <summary>
        /// Plays the animation once.
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// <param name="hideOnFinish">Hide the sprite when finished?</param>
        /// </summary>
        public IEnumerator PlayAnimation(float speed, bool hideOnFinish = false, Action finished = null)
        {
            GetCachedComponent<SpriteRenderer>().enabled = true;

            float interval = Duration / speed / Sprites.Length;

            foreach (Sprite sprite in Sprites)
            {
                GetCachedComponent<SpriteRenderer>().sprite = sprite;

                yield return new WaitForSeconds(interval);
            }

            if (hideOnFinish) GetCachedComponent<SpriteRenderer>().enabled = false;

            finished?.Invoke();
        }
    }
}