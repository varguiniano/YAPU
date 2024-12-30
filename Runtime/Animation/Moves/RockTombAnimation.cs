using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.Animation.Moves
{
    /// <summary>
    /// Animation controller for the move Rock Tomb.
    /// </summary>
    public class RockTombAnimation : WhateverBehaviour<RockTombAnimation>
    {
        /// <summary>
        /// Reference to the rocks.
        /// </summary>
        [SerializeField]
        private List<SpriteRenderer> Rocks;

        /// <summary>
        /// Play the animation.
        /// </summary>
        /// <param name="speed">Battle speed.</param>
        public IEnumerator PlayAnimation(float speed)
        {
            float fallDuration = .3f / speed;

            foreach (SpriteRenderer spriteRenderer in Rocks)
                yield return spriteRenderer.transform.DOLocalMoveY(-.3f, fallDuration).WaitForCompletion();

            yield return new WaitForSeconds(.8f / speed);

            foreach (SpriteRenderer spriteRenderer in Rocks) spriteRenderer.DOFade(0, .1f / speed);
        }
    }
}