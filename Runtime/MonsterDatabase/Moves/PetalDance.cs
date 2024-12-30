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
    /// Data class for the move PetalDance.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Grass/PetalDance", fileName = "PetalDance")]
    public class PetalDance : FixationMove
    {
        /// <summary>
        /// Petal texture.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Texture2D Petal;

        /// <summary>
        /// Move's audio.
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

            sprite.FXAnimator.PlayBoost(speed, false, Petal);

            bool finished = false;

            sprite.Pivot.DOShakePosition(4f / speed, .3f, 1).OnComplete(() => finished = true);

            yield return new WaitUntil(() => finished);
        }
    }
}