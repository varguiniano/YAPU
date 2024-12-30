using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Animation.Moves;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move FlameCharge.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Fire/FlameCharge", fileName = "FlameCharge")]
    public class FlameCharge : StageChanceDamageMove
    {
        /// <summary>
        /// Sound effect.
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
        /// Reference to the big flames prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private BigFlames BigFlamesPrefab;

        /// <summary>
        /// Play the move animation.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// <param name="userType">Type of battler the user is.</param>
        /// <param name="userIndex">User index.</param>
        /// <param name="user">User of the move.</param>
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
            BigFlames flames = Instantiate(BigFlamesPrefab, userPosition);

            yield return WaitAFrame;

            battleManager.AudioManager.PlayAudio(Sound, pitch: speed);

            yield return flames.PlayAnimation(speed);

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

            DOVirtual.DelayedCall(3, () => Destroy(flames.gameObject));
        }
    }
}