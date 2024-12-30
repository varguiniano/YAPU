using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation.Moves;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move Petal Blizzard.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Grass/PetalBlizzard", fileName = "PetalBlizzard")]
    public class PetalBlizzard : DamageMove
    {
        /// <summary>
        /// Animation prefab for the move.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private LateralParticlesMoveAnimation AnimationPrefab;

        /// <summary>
        /// Color of the petals.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        [ColorUsage(true, true)]
        private Color PetalsColor;

        /// <summary>
        /// Reference to this moves audio.
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
            LateralParticlesMoveAnimation animation = Instantiate(AnimationPrefab);

            AudioManager.Instance.PlayAudio(Audio, pitch: speed);
            animation.PlayFX(targets.First().Item1, colorOverride: PetalsColor);

            yield return new WaitForSeconds(1f / speed);

            foreach (Transform targetPosition in targetPositions)
                targetPosition.DOShakePosition(2.7f / speed, .05f, 100);

            yield return new WaitForSeconds(1f / speed);

            animation.StopVFX();

            DOVirtual.DelayedCall(3, () => Destroy(animation));
        }
    }
}