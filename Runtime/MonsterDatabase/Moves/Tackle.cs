using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move tackle.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/Tackle", fileName = "Tackle")]
    public class Tackle : DamageMove
    {
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
        /// Sound of this move.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Sound;

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
            Vector3 originalUserPosition = userPosition.localPosition;

            yield return userPosition.DOLocalMove(userType == BattlerType.Ally
                                                      ? originalUserPosition + UserMove
                                                      : originalUserPosition - UserMove,
                                                  .25f / speed)
                                     .WaitForCompletion();

            BasicSpriteAnimation hit = Instantiate(HitPrefab, targetPositions[0].position, Quaternion.identity);

            yield return WaitAFrame;

            AudioManager.Instance.PlayAudio(Sound, pitch: speed);

            userPosition.DOLocalMove(originalUserPosition, .25f / speed);

            yield return hit.PlayAnimation(speed);

            Destroy(hit.gameObject);
        }

        /// <summary>
        /// Play the move animation without sound.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// <param name="userType">Type of battler the user is.</param>
        /// <param name="userIndex">User index.</param>
        /// <param name="user">User of the attack.</param>
        /// <param name="userPosition">Position of the user.</param>
        /// <param name="targets">Targets types and indexes.</param>
        /// <param name="targetPositions">Position of the targets.</param>
        public IEnumerator PlayAnimationWithoutSound(BattleManager battleManager,
                                                     float speed,
                                                     BattlerType userType,
                                                     int userIndex,
                                                     Battler user,
                                                     Transform userPosition,
                                                     List<(BattlerType Type, int Index)> targets,
                                                     List<Transform> targetPositions)
        {
            Vector3 originalUserPosition = userPosition.localPosition;

            yield return userPosition.DOLocalMove(userType == BattlerType.Ally
                                                      ? originalUserPosition + UserMove
                                                      : originalUserPosition - UserMove,
                                                  .25f / speed)
                                     .WaitForCompletion();

            BasicSpriteAnimation hit = Instantiate(HitPrefab, targetPositions[0].position, Quaternion.identity);

            yield return WaitAFrame;

            userPosition.DOLocalMove(originalUserPosition, .25f / speed);

            yield return hit.PlayAnimation(speed);

            Destroy(hit.gameObject);
        }
    }
}