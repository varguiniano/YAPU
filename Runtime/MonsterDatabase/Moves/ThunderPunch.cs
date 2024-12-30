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
    /// Data class for the move ThunderPunch.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Electric/ThunderPunch", fileName = "ThunderPunch")]
    public class ThunderPunch : StatusChanceDamageMove
    {
        /// <summary>
        /// Reference to the move's sound.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Sound;

        /// <summary>
        /// Reference to the punch prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Punch PunchPrefab;

        /// <summary>
        /// Reference to the animation prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private BasicSpriteAnimation ParalysisPrefab;

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
            for (int i = 0; i < targetPositions.Count; i++)
            {
                Transform targetPosition = targetPositions[i];
                Punch punch = Instantiate(PunchPrefab, targetPosition);

                BattleMonsterSprite sprite = battleManager.GetMonsterSprite(targets[i]);

                BasicSpriteAnimation paralysis = Instantiate(ParalysisPrefab, sprite.Pivot);

                yield return WaitAFrame;

                battleManager.Audio.PlayAudio(Sound, pitch: speed);

                yield return new WaitForSeconds(.12f / speed);

                yield return punch.PlayAnimation(.3f / speed, speed);

                yield return paralysis.PlayAnimation(speed, true);

                DOVirtual.DelayedCall(3,
                                      () =>
                                      {
                                          Destroy(punch.gameObject);
                                          Destroy(paralysis.gameObject);
                                      });
            }
        }
    }
}