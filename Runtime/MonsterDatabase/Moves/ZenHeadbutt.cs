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
    /// Data class for the move ZenHeadbutt.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Psychic/ZenHeadbutt", fileName = "ZenHeadbutt")]
    public class ZenHeadbutt : StatusChanceDamageMove
    {
        /// <summary>
        /// Reference to the move's sound.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Sound;

        /// <summary>
        /// Reference to the particles prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private VisualEffect ParticlesPrefab;

        /// <summary>
        /// Reference to a move with a tackle animation.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Tackle TackleMove;

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
            VisualEffect particles = Instantiate(ParticlesPrefab, userPosition);

            battleManager.Audio.PlayAudio(Sound, pitch: speed);

            yield return new WaitForSeconds(.1f / speed);

            particles.EnableAndPlay();

            yield return new WaitForSeconds(1.5f / speed);

            particles.Stop();

            yield return TackleMove.PlayAnimationWithoutSound(battleManager,
                                                              speed,
                                                              userType,
                                                              userIndex,
                                                              user,
                                                              userPosition,
                                                              targets,
                                                              targetPositions);

            DOVirtual.DelayedCall(3, () => Destroy(particles.gameObject));
        }
    }
}