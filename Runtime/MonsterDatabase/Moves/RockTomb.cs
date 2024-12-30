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
    /// Data class for the move RockTomb.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Rock/RockTomb", fileName = "RockTomb")]
    public class RockTomb : StageChanceDamageMove
    {
        /// <summary>
        /// Move sound.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Sound;

        /// <summary>
        /// Move animation prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private RockTombAnimation AnimationPrefab;

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
            foreach (RockTombAnimation moveAnimation in targetPositions.Select(targetPosition =>
                                                                                   Instantiate(AnimationPrefab,
                                                                                       targetPosition)))
            {
                yield return WaitAFrame;

                battleManager.AudioManager.PlayAudio(Sound, pitch: speed);

                yield return moveAnimation.PlayAnimation(speed);

                DOVirtual.DelayedCall(3, () => Destroy(moveAnimation.gameObject));
            }
        }
    }
}