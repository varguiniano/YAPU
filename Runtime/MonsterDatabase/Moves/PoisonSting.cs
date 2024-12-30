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
    /// Data class for the move PoisonSting.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Poison/PoisonSting", fileName = "PoisonSting")]
    public class PoisonSting : StatusChanceDamageMove
    {
        /// <summary>
        /// Reference to the move's audio
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

        /// <summary>
        /// Reference to the stinger prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Transform StingerPrefab;

        /// <summary>
        /// Hit effect prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private VisualEffect HitPrefab;

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
                battleManager.Audio.PlayAudio(Audio, pitch: speed);

                yield return new WaitForSeconds(.1f / speed);

                Transform stinger = Instantiate(StingerPrefab, userPosition);

                stinger.rotation = Quaternion.Euler(0, 0, targets[i].Item1 == BattlerType.Ally ? -135 : 45);

                Transform targetPosition = targetPositions[i];

                yield return stinger.DOMove(targetPosition.position, .3f / speed).WaitForCompletion();

                Destroy(stinger.gameObject);

                VisualEffect hit = Instantiate(HitPrefab, targetPosition);
                hit.EnableAndPlay();
                yield return new WaitForSeconds(1.2f / speed);
                hit.Stop();

                DOVirtual.DelayedCall(3,
                                      () =>
                                      {
                                          Destroy(hit.gameObject);
                                      });
            }
        }
    }
}