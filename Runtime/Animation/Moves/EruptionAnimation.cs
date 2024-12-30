using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.Animation.Moves
{
    /// <summary>
    /// controller for an eruption animation.
    /// </summary>
    public class EruptionAnimation : WhateverBehaviour<EruptionAnimation>
    {
        /// <summary>
        /// Items that make the base eruption.
        /// </summary>
        [SerializeField]
        private List<SpriteRenderer> EruptionItems;

        /// <summary>
        /// Eruption particles.
        /// </summary>
        [SerializeField]
        private VisualEffect EruptionParticles;

        /// <summary>
        /// Play the animation.
        /// </summary>
        public void PlayAnimation()
        {
            foreach (SpriteRenderer eruptionItem in EruptionItems)
            {
                eruptionItem.DOFade(1, .25f);
                eruptionItem.transform.DOShakeScale(1, .05f, 20, fadeOut: false).SetLoops(-1);
            }

            EruptionParticles.enabled = true;
            EruptionParticles.Play();
        }

        /// <summary>
        /// Stop the animation.
        /// </summary>
        public void StopAnimation()
        {
            foreach (SpriteRenderer eruptionItem in EruptionItems)
            {
                eruptionItem.DOFade(0, .25f);
                eruptionItem.transform.DOKill();
            }

            EruptionParticles.Stop();
        }
    }
}