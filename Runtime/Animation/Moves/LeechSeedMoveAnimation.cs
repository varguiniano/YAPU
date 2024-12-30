using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.Animation.Moves
{
    /// <summary>
    /// Controller for the leech seed animation.
    /// It has to be instantiated as a child of the pivot of the target.
    /// </summary>
    public class LeechSeedMoveAnimation : WhateverBehaviour<LeechSeedMoveAnimation>
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
        /// Reference to the seed animators.
        /// </summary>
        [SerializeField]
        private List<BasicSpriteAnimation> SeedAnimators;

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
        /// Test play method.
        /// </summary>
        /// <param name="source">Source of the seeds.</param>
        /// <param name="targetType">Type of the target getting hit.</param>
        [Button("Play")]
        [HideInEditorMode]
        private void PlayTest(Transform source, BattlerType targetType) => StartCoroutine(Play(source, targetType, 1));

        /// <summary>
        /// Play the animation.
        /// </summary>
        /// <param name="source">Source of the animation.</param>
        /// <param name="targetType">Type of the target getting hit.</param>
        /// <param name="speed">Battle speed.</param>
        public IEnumerator Play(Transform source, BattlerType targetType, float speed)
        {
            foreach (Transform seedTransform in SeedTransforms) seedTransform.position = source.position;

            foreach (SpriteRenderer spriteRenderer in SeedSprites) spriteRenderer.enabled = true;

            bool finished = false;

            for (int i = 0; i < SeedTransforms.Count; i++)
            {
                List<Transform> pathToUse = targetType == BattlerType.Enemy ? EnemyPath : AllyPath;

                List<Vector3> path = pathToUse.Select(t => t.position).ToList();

                path.Add(SeedTargets[i].position);

                SeedTransforms[i]
                   .DOPath(path.ToArray(), .5f / speed, PathType.CatmullRom)
                   .OnComplete(() => finished = true);
            }

            // ReSharper disable once AccessToModifiedClosure
            yield return new WaitUntil(() => finished);

            finished = false;

            foreach (BasicSpriteAnimation seedAnimator in SeedAnimators)
                StartCoroutine(seedAnimator.PlayAnimation(speed, finished: () => finished = true));

            yield return new WaitUntil(() => finished);

            foreach (SpriteRenderer spriteRenderer in SeedSprites) spriteRenderer.enabled = false;
        }
    }
}