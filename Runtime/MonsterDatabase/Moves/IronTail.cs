using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move IronTail.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Steel/IronTail", fileName = "IronTail")]
    public class IronTail : StageChanceDamageMove
    {
        /// <summary>
        /// Reference to the move's audio.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

        /// <summary>
        /// Reference to the Tail prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private SpriteRenderer TailPrefab;

        /// <summary>
        /// Tail starting position.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Vector3 TailStartingPosition;

        /// <summary>
        /// Tail starting rotation.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Vector3 TailStartingRotation;

        /// <summary>
        /// Tail target position.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Vector3 TailTargetPosition;

        /// <summary>
        /// Play the move animation.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// <param name="userType">Type of battler the user is.</param>
        /// <param name="userIndex">User index.</param>
        /// <param name="user">User of the attack.</param>
        /// <param name="userPosition">Position of the user.</param>
        /// <param name="targets">Targets types and indexes.</param>
        /// <param name="targetPositions">Position of the targets.</param>
        /// <param name="ignoresAbilities"></param>
        public override IEnumerator PlayAnimation(BattleManager battleManager,
                                                  float speed,
                                                  BattlerType userType,
                                                  int userIndex,
                                                  Battler user,
                                                  Transform userPosition,
                                                  List<(BattlerType Type, int Index)> targets,
                                                  List<Transform> targetPositions,
                                                  bool ignoresAbilities)
        {
            foreach (Transform targetPosition in targetPositions)
            {
                AudioManager.Instance.PlayAudio(Audio, pitch: speed);

                userPosition.DOShakePosition(.2f / speed, .1f);

                SpriteRenderer whip = Instantiate(TailPrefab, targetPosition);
                Transform whipTransform = whip.GetComponent<Transform>();

                whipTransform.localPosition = TailStartingPosition;
                whipTransform.localRotation = Quaternion.Euler(TailStartingRotation);

                whip.DOFade(1, .1f / speed);

                yield return whipTransform.DOLocalMove(TailTargetPosition, .5f / speed)
                                          .SetEase(Ease.InBack)
                                          .WaitForCompletion();

                whip.DOFade(0, .1f / speed).OnComplete(() => Destroy(whip.gameObject));
            }
        }
    }
}