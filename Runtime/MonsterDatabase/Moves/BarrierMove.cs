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
    /// Data class for moves that set a barrier on their side.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/General/BarrierMove", fileName = "BarrierMove")]
    public class BarrierMove : SetSideStatusMove
    {
        /// <summary>
        /// Prefab for the animation.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private SpriteRenderer AnimationPrefab;

        /// <summary>
        /// Were to place the barrier when its on the enemy side.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Vector3 EnemyPosition;

        /// <summary>
        /// Were to place the barrier when its on the enemy side.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private int EnemyPriority;

        /// <summary>
        /// Were to place the barrier when its on the allied side.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Vector3 AllyPosition;

        /// <summary>
        /// Were to place the barrier when its on the allied side.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private int AllyPriority;

        /// <summary>
        /// Animation audio.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

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
            foreach ((BattlerType targetType, int targetIndex) in targets)
            {
                BattleMonsterSprite sprite = battleManager.GetMonsterSprite(targetType, targetIndex);

                SpriteRenderer animation = Instantiate(AnimationPrefab, sprite.Pivot);

                animation.transform.localPosition =
                    targetType == BattlerType.Ally ? AllyPosition : EnemyPosition;

                animation.sortingOrder =
                    targetType == BattlerType.Ally ? AllyPriority : EnemyPriority;

                AudioManager.Instance.PlayAudio(Audio, pitch: speed);

                bool finished = false;

                // 0.7843137255 = 200/255
                animation.DOFade(0.7843137255f, .75f / speed).OnComplete(() => finished = true);

                yield return new WaitUntil(() => finished);

                finished = false;

                animation.DOFade(0, .75f / speed).OnComplete(() => finished = true);

                yield return new WaitUntil(() => finished);

                DOVirtual.DelayedCall(2f, () => Destroy(animation.gameObject));
            }
        }
    }
}