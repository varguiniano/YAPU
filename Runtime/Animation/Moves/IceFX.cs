using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.Animation.Moves
{
    /// <summary>
    /// Controller for an animation that displays ice FX.
    /// </summary>
    public class IceFX : WhateverBehaviour<IceFX>
    {
        /// <summary>
        /// Crystals of the animation.
        /// </summary>
        [SerializeField]
        private List<SpriteRenderer> Crystals;

        /// <summary>
        /// Play the animation.
        /// </summary>
        /// <param name="duration">Duration of the animation.</param>
        /// <param name="speed">Battle speed.</param>
        public IEnumerator PlayAnimation(float duration, float speed)
        {
            float interval = duration / Crystals.Count;

            foreach (SpriteRenderer crystal in Crystals) yield return crystal.DOFade(1, interval).WaitForCompletion();

            foreach (SpriteRenderer crystal in Crystals) crystal.DOFade(0, .2f / speed);
        }
    }
}