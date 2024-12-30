using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for Flamethrower.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Fire/Flamethrower", fileName = "Flamethrower")]
    public class Flamethrower : StatusChanceDamageMove
    {
        /// <summary>
        /// Reference to the beam audio.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference BeamAudio;

        /// <summary>
        /// Animation curve for the beam size.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AnimationCurve BeamSize;

        /// <summary>
        /// Beam particle.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Texture2D BeamParticle;

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
                (BattlerType targetType, int targetIndex) = targets[i];

                BattleMonsterSprite sprite = battleManager.GetMonsterSprite(targetType, targetIndex);

                AudioManager.Instance.PlayAudio(BeamAudio, pitch: speed);

                yield return sprite.FXAnimator.PlayAbsorb(2.7f / speed,
                                                          sprite.transform.position,
                                                          userPosition,
                                                          spawnRadius: 0,
                                                          sizeOverLifetime: BeamSize,
                                                          spawnRate: 100,
                                                          particleTexture: BeamParticle);
            }
        }
    }
}