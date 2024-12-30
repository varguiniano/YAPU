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
    /// Data class for the move Venoshock.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Poison/Venoshock", fileName = "Venoshock")]
    public class Venoshock : TargetStatusModifiableDamageMove
    {
        /// <summary>
        /// Reference to the animation prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private BasicSpriteAnimation AnimationPrefab;

        /// <summary>
        /// Hit effect prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private VisualEffect HitPrefab;

        /// <summary>
        /// Reference to the move's audio.
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
            for (int i = 0; i < targets.Count; i++)
            {
                (BattlerType targetType, int targetIndex) = targets[i];
                Transform pivot = battleManager.GetMonsterSprite(targetType, targetIndex).Pivot;

                AudioManager.Instance.PlayAudio(Audio, pitch: battleManager.BattleSpeed);

                BasicSpriteAnimation animation = Instantiate(AnimationPrefab, pivot);

                yield return animation.PlayAnimation(battleManager.BattleSpeed);

                VisualEffect hit = Instantiate(HitPrefab, pivot);

                animation.GetCachedComponent<SpriteRenderer>().enabled = false;

                DOVirtual.DelayedCall(3f,
                                      () =>
                                      {
                                          Destroy(animation.gameObject);
                                          Destroy(hit.gameObject);
                                      });
            }
        }
    }
}