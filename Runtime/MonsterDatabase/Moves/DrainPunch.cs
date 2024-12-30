using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Animation.Moves;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move DrainPunch.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Fighting/DrainPunch", fileName = "DrainPunch")]
    public class DrainPunch : AbsorbMove
    {
        /// <summary>
        /// Reference to the punch prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Punch PunchPrefab;

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
            for (int i = 0; i < targets.Count; ++i)
            {
                (BattlerType targetType, int targetIndex) = targets[i];

                Transform targetPosition = targetPositions[i];

                BattleMonsterSprite sprite = battleManager.GetMonsterSprite(userType, userIndex);

                BattleMonsterSprite targetSprite = battleManager.GetMonsterSprite(targetType, targetIndex);

                Punch punch = Instantiate(PunchPrefab, targetPosition);

                yield return WaitAFrame;

                yield return punch.PlayAnimation(.3f / speed, speed);

                DOVirtual.DelayedCall(3, () => Destroy(punch.gameObject));

                BasicSpriteAnimation hit = Instantiate(HitPrefab, targetPosition.position, Quaternion.identity);

                CoroutineRunner.Instance.StartCoroutine(hit.PlayAnimation(speed,
                                                                          finished: () => Destroy(hit.gameObject)));

                AudioManager.Instance.PlayAudio(Audio, pitch: battleManager.BattleSpeed);

                yield return sprite.FXAnimator.PlayAbsorb(2f / battleManager.BattleSpeed,
                                                          sprite.transform.position,
                                                          targetSprite.transform,
                                                          spawnRadius: .2f,
                                                          sizeOverLifetime: ParticlesSizeOverTime);

                sprite.FXAnimator.PlayBoost(battleManager.BattleSpeed, false);
            }
        }
    }
}