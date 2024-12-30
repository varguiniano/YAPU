using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for Ember.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Fire/Ember", fileName = "Ember")]
    public class Ember : StatusChanceDamageMove
    {
        /// <summary>
        /// Sound effect.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Sound;

        /// <summary>
        /// Reference to the FX prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private BasicSpriteAnimation FXPrefab;

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
            battleManager.AudioManager.PlayAudio(Sound, pitch: speed);

            yield return new WaitForSeconds(.2f / speed);

            BasicSpriteAnimation spriteAnimation = Instantiate(FXPrefab, targetPositions[0]);

            yield return WaitAFrame;

            yield return spriteAnimation.PlayAnimation(speed);

            yield return spriteAnimation.GetCachedComponent<SpriteRenderer>()
                                        .DOFade(0, .2f / speed)
                                        .WaitForCompletion();

            Destroy(spriteAnimation.gameObject);
        }
    }
}