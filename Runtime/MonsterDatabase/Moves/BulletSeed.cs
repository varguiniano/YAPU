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
    /// Data class for the move Bullet Seed.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Grass/BulletSeed", fileName = "BulletSeed")]
    public class BulletSeed : DamageMove
    {
        /// <summary>
        /// Reference to the seed prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Transform SeedPrefab;

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
            foreach (Transform targetPosition in targetPositions)
            {
                List<Transform> seeds = new();

                AudioManager.Instance.PlayAudio(Audio, pitch: battleManager.BattleSpeed);

                for (int i = 0; i < 9; ++i)
                {
                    Transform seed = Instantiate(SeedPrefab, userPosition.position, Quaternion.identity, null);

                    yield return WaitAFrame;

                    seed.DOMove(targetPosition.position, .5f / battleManager.BattleSpeed)
                        .OnComplete(() => seed.GetComponent<SpriteRenderer>().enabled = false);

                    seeds.Add(seed);

                    yield return new WaitForSeconds(.2f / battleManager.BattleSpeed);
                }

                DOVirtual.DelayedCall(3f,
                                      () =>
                                      {
                                          foreach (Transform seed in seeds) Destroy(seed.gameObject);
                                      });
            }
        }
    }
}