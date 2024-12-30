using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move Vine Whip.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Grass/VineWhip", fileName = "VineWhip")]
    public class VineWhip : DamageMove
    {
        /// <summary>
        /// Reference to the move's audio.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

        /// <summary>
        /// Reference to the vine prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private SpriteRenderer VinePrefab;

        /// <summary>
        /// Vine starting position.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Vector3 VineStartingPosition;

        /// <summary>
        /// Vine starting rotation.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Vector3 VineStartingRotation;

        /// <summary>
        /// Vine target position.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Vector3 VineTargetPosition;

        /// <summary>
        /// Grass hit effect prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private VisualEffect GrassHitPrefab;

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

                yield return new WaitForSeconds(.3f / speed);

                SpriteRenderer whip = Instantiate(VinePrefab, targetPosition);
                Transform whipTransform = whip.GetComponent<Transform>();

                whipTransform.localPosition = VineStartingPosition;
                whipTransform.localRotation = Quaternion.Euler(VineStartingRotation);

                whip.DOFade(1, .1f / speed);

                bool finished = false;

                Tween tween = whipTransform.DOLocalMove(VineTargetPosition, .3f / speed)
                                           .SetEase(Ease.InBack)
                                           .OnComplete(() => finished = true);

                bool shownHit = false;
                VisualEffect hit = null;

                tween.OnUpdate(() =>
                               {
                                   if (shownHit || !(tween.ElapsedPercentage() > .6f)) return;
                                   hit = Instantiate(GrassHitPrefab, targetPosition);
                                   shownHit = true;
                               });

                yield return new WaitUntil(() => finished);

                whip.DOFade(0, .1f / speed).OnComplete(() => Destroy(whip.gameObject));

                DOVirtual.DelayedCall(3, () => Destroy(hit.gameObject));
            }
        }
    }
}