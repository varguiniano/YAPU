using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Animation
{
    /// <summary>
    /// Controller for a basic sprite animation that loops.
    /// </summary>
    public class LoopSpriteAnimation : BasicSpriteAnimation
    {
        /// <summary>
        /// Loop routine reference.
        /// </summary>
        private Coroutine loop;

        /// <summary>
        /// Start the animation loop.
        /// </summary>
        /// <param name="speed">Speed at which to play the animation.</param>
        public void StartLoop(float speed)
        {
            if (loop != null) return;

            loop = StartCoroutine(AnimationLoop(speed));
        }

        /// <summary>
        /// Stop the animation loop.
        /// </summary>
        /// <param name="speed">Speed of the animation.</param>
        /// <param name="hideSprite">Hide the sprite?</param>
        public IEnumerator StopLoop(float speed, bool hideSprite = true)
        {
            StopCoroutine(loop);

            yield return WaitAFrame;

            loop = null;

            if (hideSprite)
                yield return GetCachedComponent<SpriteRenderer>().DOFade(0, .1f / speed).WaitForCompletion();
        }

        /// <summary>
        /// Routine that handles the loop.
        /// </summary>
        /// <param name="speed">Speed at which to play the animation.</param>
        private IEnumerator AnimationLoop(float speed)
        {
            while (true) yield return PlayAnimation(speed);
            // ReSharper disable once IteratorNeverReturns
        }
    }
}