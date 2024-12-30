using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    /// Data class for the move Dive.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Water/Dive", fileName = "Dive")]
    public class Dive : TwoTurnDamageMove
    {
        /// <summary>
        /// Audio for the first part of the move.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference PartOneAudio;

        /// <summary>
        /// Audio for the second part of the move.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference PartTwoAudio;

        /// <summary>
        /// Reference to the hit prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private BasicSpriteAnimation HitPrefab;

        /// <summary>
        /// Reference to the particles prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private VisualEffect VaporParticlesPrefab;

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
            battleManager.Audio.PlayAudio(PartOneAudio, pitch: speed);

            yield return new WaitForSeconds(.3f / speed);

            yield return battleManager.GetMonsterSprite(userType, userIndex).DiveDown(speed, VaporParticlesPrefab);
        }

        /// <summary>
        /// Play the animation for the second turn of this move.
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
        public override IEnumerator PlaySecondTurnAnimation(BattleManager battleManager,
                                                            float speed,
                                                            BattlerType userType,
                                                            int userIndex,
                                                            Battler user,
                                                            Transform userPosition,
                                                            List<(BattlerType Type, int Index)> targets,
                                                            List<Transform> targetPositions,
                                                            bool ignoresAbilities)
        {
            BattleMonsterSprite userSprite = battleManager.GetMonsterSprite(userType, userIndex);

            VisualEffect particles = Instantiate(VaporParticlesPrefab,
                                                 userSprite.transform.position + new Vector3(0, -.5f, 0),
                                                 Quaternion.identity);

            yield return WaitAFrame;

            battleManager.Audio.PlayAudio(PartTwoAudio, pitch: speed);

            yield return new WaitForSeconds(.12f / speed);

            particles.EnableAndPlay();

            yield return userSprite.DiveUp(speed);

            foreach (BasicSpriteAnimation hit in targetPositions.Select(targetPosition =>
                                                                            Instantiate(HitPrefab, targetPosition)))
            {
                yield return hit.PlayAnimation(speed);

                Destroy(hit.gameObject);
            }

            particles.Stop();

            DOVirtual.DelayedCall(3, () => Destroy(particles.gameObject));
        }
    }
}