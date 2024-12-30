using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move AquaJet.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Water/AquaJet", fileName = "AquaJet")]
    public class AquaJet : DamageMove
    {
        /// <summary>
        /// Sound of this move.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Sound;

        /// <summary>
        /// Reference to the hit prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private BasicSpriteAnimation HitPrefab;

        /// <summary>
        /// Amount to move the user forth and back.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Vector3 UserMove;

        /// <summary>
        /// Prefab for water vapor.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private VisualEffect VaporPrefab;

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
                Vector3 originalUserPosition = userPosition.localPosition;

                yield return userPosition.DOLocalMove(userType == BattlerType.Ally
                                                          ? originalUserPosition + UserMove
                                                          : originalUserPosition - UserMove,
                                                      .25f / speed)
                                         .WaitForCompletion();

                BasicSpriteAnimation hit = Instantiate(HitPrefab, targetPosition);
                VisualEffect vapor = Instantiate(VaporPrefab, targetPosition);

                yield return WaitAFrame;

                AudioManager.Instance.PlayAudio(Sound, pitch: speed);

                userPosition.DOLocalMove(originalUserPosition, .25f / speed);

                vapor.EnableAndPlay();

                yield return hit.PlayAnimation(speed, true);

                vapor.Stop();

                DOVirtual.DelayedCall(3,
                                      () =>
                                      {
                                          Destroy(vapor.gameObject);
                                          Destroy(hit.gameObject);
                                      });
            }
        }
    }
}