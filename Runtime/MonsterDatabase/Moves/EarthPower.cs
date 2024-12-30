﻿using System.Collections;
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
    /// Data class for the move EarthPower.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Ground/EarthPower", fileName = "EarthPower")]
    public class EarthPower : StageChanceDamageMove
    {
        /// <summary>
        /// Reference to the sound.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Sound;

        /// <summary>
        /// Eruption animation prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private EruptionAnimation EruptionAnimation;

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
            foreach (EruptionAnimation eruptionAnimation in targetPositions.Select(targetPosition =>
                         Instantiate(EruptionAnimation, targetPosition)))
            {
                battleManager.AudioManager.PlayAudio(Sound, pitch: speed);

                eruptionAnimation.PlayAnimation();

                yield return new WaitForSeconds(3f / speed);

                eruptionAnimation.StopAnimation();

                DOVirtual.DelayedCall(3, () => Destroy(eruptionAnimation.gameObject));
            }
        }
    }
}