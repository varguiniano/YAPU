using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.Animation.Moves
{
    /// <summary>
    /// Controller for the leech seed animation.
    /// It has to be instantiated as a child of the pivot of the target.
    /// </summary>
    public class SeedBombMoveAnimation : WhateverBehaviour<SeedBombMoveAnimation>
    {
        /// <summary>
        /// Reference to the seed transforms.
        /// </summary>
        [SerializeField]
        private List<Transform> SeedTransforms;

        /// <summary>
        /// Reference to the seed sprites.
        /// </summary>
        [SerializeField]
        private List<SpriteRenderer> SeedSprites;

        /// <summary>
        /// Reference to the seed animations.
        /// </summary>
        [SerializeField]
        private List<BasicSpriteAnimation> Animations;

        /// <summary>
        /// Reference to the seed targets.
        /// </summary>
        [SerializeField]
        private List<Transform> SeedTargets;

        /// <summary>
        /// Path for the seeds to follow.
        /// </summary>
        [SerializeField]
        private List<Transform> EnemyPath;

        /// <summary>
        /// Path for the seeds to follow.
        /// </summary>
        [SerializeField]
        private List<Transform> AllyPath;

        /// <summary>
        /// Play the animation.
        /// </summary>
        /// <param name="source">Source of the animation.</param>
        /// <param name="targetType">Type of the target getting hit.</param>
        /// <param name="speed">Battle speed.</param>
        public IEnumerator Play(Transform source, BattlerType targetType, float speed)
        {
            yield return new WaitForSeconds(.4f / speed);

            foreach (Transform seedTransform in SeedTransforms) seedTransform.position = source.position;

            for (int i = 0; i < SeedTransforms.Count; i++)
            {
                List<Transform> pathToUse = targetType == BattlerType.Enemy ? EnemyPath : AllyPath;

                List<Vector3> path = pathToUse.Select(t => t.position).ToList();

                path.Add(SeedTargets[i].position);

                SeedSprites[i].enabled = true;

                SeedTransforms[i]
                   .DOPath(path.ToArray(), .8f / speed, PathType.CatmullRom)
                   .SetEase(Ease.Linear);

                yield return new WaitForSeconds(.1f / speed);
            }

            yield return new WaitForSeconds(.6f / speed);

            for (int i = 0; i < Animations.Count; i++)
            {
                BasicSpriteAnimation seedAnimation = Animations[i];
                int iCopy = i;
                StartCoroutine(seedAnimation.PlayAnimation(speed, finished: () => SeedSprites[iCopy].enabled = false));

                yield return new WaitForSeconds(.1f / speed);
            }
        }
    }
}