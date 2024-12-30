using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move SwordsDance.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/SwordsDance", fileName = "SwordsDance")]
    public class SwordsDance : StageChangeMove
    {
        /// <summary>
        /// Particle texture.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Texture2D ParticleTexture;

        /// <summary>
        /// Reference to the audio.
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
            BattleMonsterSprite sprite = battleManager.GetMonsterSprite(userType, userIndex);

            AudioManager.Instance.PlayAudio(Audio, pitch: speed);

            sprite.Pivot.DOShakePosition(1.7f / speed, .1f, 3);

            sprite.FXAnimator.PlayRiseStat(speed,
                                           null,
                                           ParticleTexture,
                                           playAudio: false,
                                           scaleOverride: new Vector3(.5f, 1.5f, .5f));

            yield return new WaitForSeconds(1.7f / speed);
        }
    }
}