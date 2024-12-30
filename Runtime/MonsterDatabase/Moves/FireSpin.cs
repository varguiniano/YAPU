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
    /// Class representing the move FireSpin.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Fire/FireSpin", fileName = "FireSpin")]
    public class FireSpin : DamageAndSetVolatileAffectedByBindingBandMove
    {
        /// <summary>
        /// Reference to the vortex audio.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

        /// <summary>
        /// Reference to the vortex prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private VisualEffect VortexPrefab;

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
                VisualEffect vortex = Instantiate(VortexPrefab,
                                                  battleManager
                                                     .GetMonsterSprite(battleManager.Battlers
                                                                          .GetBattlerFromBattleIndex(targetType,
                                                                               targetIndex))
                                                     .transform);

                AudioManager.Instance.PlayAudio(Audio, pitch: battleManager.BattleSpeed);

                vortex.enabled = true;
                vortex.Play();

                yield return new WaitForSeconds(1.8f / battleManager.BattleSpeed);

                vortex.Stop();

                DOVirtual.DelayedCall(3, () => Destroy(vortex.gameObject));
            }
        }
    }
}