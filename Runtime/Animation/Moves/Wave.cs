using System.Collections;
using DG.Tweening;
using UnityEngine;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.Animation.Moves
{
    /// <summary>
    /// Controller for an animation of a wave that crosses the screen.
    /// </summary>
    public class Wave : WhateverBehaviour<Wave>
    {
        /// <summary>
        /// Reference to the Wave transform.
        /// </summary>
        [SerializeField]
        private Transform WaveTransform;

        /// <summary>
        /// Reference to the wave target.
        /// </summary>
        [SerializeField]
        private Transform WaveTarget;

        /// <summary>
        /// Play the animation.
        /// </summary>
        /// <param name="duration">Duration of the animation.</param>
        public IEnumerator PlayAnimation(float duration)
        {
            yield return WaveTransform.DOMove(WaveTarget.position, duration).WaitForCompletion();
        }
    }
}