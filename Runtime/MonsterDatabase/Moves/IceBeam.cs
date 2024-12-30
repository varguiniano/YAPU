using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation.Moves;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move IceBeam.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Ice/IceBeam", fileName = "IceBeam")]
    public class IceBeam : StatusChanceDamageMove
    {
        /// <summary>
        /// Reference to the move's sound.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Sound;

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
        /// Prefab for the ice FX.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private IceFX IcePrefab;

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
                IceFX ice = Instantiate(IcePrefab, targetPositions[i]);
                yield return WaitAFrame;

                (BattlerType targetType, int targetIndex) = targets[i];

                BattleMonsterSprite sprite = battleManager.GetMonsterSprite(targetType, targetIndex);

                battleManager.Audio.PlayAudio(Sound, pitch: speed);

                yield return new WaitForSeconds(.1f / speed);

                yield return sprite.FXAnimator.PlayAbsorb(.8f / speed,
                                                          sprite.transform.position,
                                                          userPosition,
                                                          spawnRadius: 0,
                                                          sizeOverLifetime: BeamSize,
                                                          spawnRate: 25,
                                                          particleTexture: BeamParticle);

                yield return new WaitForSeconds(.3f / speed);

                yield return ice.PlayAnimation(1 / speed, speed);

                DOVirtual.DelayedCall(3, () => Destroy(ice.gameObject));
            }
        }
    }
}