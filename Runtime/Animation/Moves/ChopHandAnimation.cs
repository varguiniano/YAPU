using System.Collections;
using DG.Tweening;
using UnityEngine;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.Animation.Moves
{
    /// <summary>
    /// Controller for the animation of a chop hand.
    /// </summary>
    public class ChopHandAnimation : WhateverBehaviour<ChopHandAnimation>
    {
        /// <summary>
        /// Reference to the hand.
        /// </summary>
        [SerializeField]
        private SpriteRenderer Hand;

        /// <summary>
        /// Play the chop animation.
        /// </summary>
        public IEnumerator PlayAnimation(float speed)
        {
            yield return Hand.DOFade(1, .1f / speed).WaitForCompletion();
            yield return Hand.transform.DOLocalMove(new Vector3(0, -.5f, 0), .5f / speed).WaitForCompletion();
            yield return Hand.DOFade(0, .1f / speed).WaitForCompletion();
        }
    }
}