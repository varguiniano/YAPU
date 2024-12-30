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
    /// Data class for the move Razor Leaf.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Grass/RazorLeaf", fileName = "RazorLeaf")]
    public class RazorLeaf : DamageMove
    {
        /// <summary>
        /// Animation prefab for the move.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        protected LateralParticlesMoveAnimation AnimationPrefab;

        /// <summary>
        /// Reference to this moves audio.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        protected AudioReference Audio;

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
            
            yield return WaitAFrame;

            AudioManager.Instance.PlayAudio(Audio, pitch: speed);
            animation.PlayFX(targets.First().Item1);

            yield return new WaitForSeconds(1f / speed);

            foreach (Transform targetPosition in targetPositions) targetPosition.DOShakePosition(3 / speed, .05f, 100);

            yield return new WaitForSeconds(1f / speed);

            animation.StopVFX();

            DOVirtual.DelayedCall(3, () => Destroy(animation.gameObject));
        }
    }
}