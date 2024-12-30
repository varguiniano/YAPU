using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.Animation
{
    /// <summary>
    /// Behaviour to control the animation of a ball bursting.
    /// </summary>
    public class BallBurst : WhateverBehaviour<BallBurst>
    {
        /// <summary>
        /// Reference to the shine visual effect.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private VisualEffect Shine;

        /// <summary>
        /// Reference to the sparks visual effect.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private VisualEffect Sparks;

        /// <summary>
        /// Reference to the shine graphic transform.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform[] Rays;

        /// <summary>
        /// Reference to the shine sprite.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private SpriteRenderer[] RaySprites;

        /// <summary>
        /// Duration of the animation.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private float AnimationDuration;

        /// <summary>
        /// Set the scales to 0.
        /// </summary>
        private void OnEnable()
        {
            Shine.enabled = false;
            Sparks.enabled = false;

            for (int i = 0; i < Rays.Length; i++) Rays[i].localScale = Vector3.zero;
        }

        /// <summary>
        /// Play the animation.
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// TODO: Support different sprites based on the ball.
        /// </summary>
        [Button]
        [HideInEditorMode]
        public void PlayAnimation(float speed)
        {
            float actualDuration = AnimationDuration / speed;
            float halfDuration = actualDuration * .5f;

            Shine.enabled = true;
            Sparks.enabled = true;

            for (int i = 0; i < Rays.Length; i++)
            {
                int index = i;

                Rays[i]
                   .DOScale(Vector3.one, actualDuration)
                   .SetEase(Ease.OutBack)
                   .OnComplete(() =>
                               {
                                   Shine.Stop();
                                   Sparks.Stop();

                                   RaySprites[index].DOFade(0, halfDuration);
                               });
            }
        }
    }
}