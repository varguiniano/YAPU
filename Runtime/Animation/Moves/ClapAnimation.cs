using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.Animation.Moves
{
    /// <summary>
    /// Controller of a clapping animation.
    /// It is synced with the helping hand sound.
    /// </summary>
    public class ClapAnimation : WhateverBehaviour<ClapAnimation>
    {
        /// <summary>
        /// Reference to the left hand.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform LeftHand;

        /// <summary>
        /// Reference to the right hand.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform RightHand;

        /// <summary>
        /// Reference to the clap audio.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private AudioReference ClapAudio;

        /// <summary>
        /// Test the animation.
        /// </summary>
        [Button]
        [HideInEditorMode]
        private void Test() => StartCoroutine(Play(1));

        /// <summary>
        /// Play the animation.
        /// </summary>
        /// <param name="speed">Battle speed.</param>
        public IEnumerator Play(float speed)
        {
            float leftHandOriginalPosition = LeftHand.position.x;
            float rightHandOriginalPosition = RightHand.position.x;

            AudioManager.Instance.PlayAudio(ClapAudio, pitch: speed);

            LeftHand.DOMoveX(0, .15f / speed);
            yield return RightHand.DOMoveX(0, .15f / speed).WaitForCompletion();

            LeftHand.DOMoveX(leftHandOriginalPosition, .15f / speed);
            yield return RightHand.DOMoveX(rightHandOriginalPosition, .15f / speed).WaitForCompletion();

            LeftHand.DOMoveX(0, .05f / speed);
            yield return RightHand.DOMoveX(0, .05f / speed).WaitForCompletion();

            LeftHand.DOMoveX(leftHandOriginalPosition, .15f / speed);
            yield return RightHand.DOMoveX(rightHandOriginalPosition, .15f / speed).WaitForCompletion();

            LeftHand.DOMoveX(0, .2f / speed);
            yield return RightHand.DOMoveX(0, .2f / speed).WaitForCompletion();

            LeftHand.DOMoveX(leftHandOriginalPosition, .2f / speed);
            yield return RightHand.DOMoveX(rightHandOriginalPosition, .2f / speed).WaitForCompletion();
        }
    }
}