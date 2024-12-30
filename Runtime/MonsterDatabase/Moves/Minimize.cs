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
    /// Class representing the move Minimize.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/Minimize", fileName = "Minimize")]
    public class Minimize : StageChangeAndSetVolatileMove
    {
        /// <summary>
        /// Reference to the move's audio.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

        /// <summary>
        /// Play the minimize animation.
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
            AudioManager.Instance.PlayAudio(Audio, pitch: speed);

            float originalScale = userPosition.localScale.x;
            float endScale = originalScale * 0.2f;
            Transform pivot = battleManager.GetMonsterSprite(userType, userIndex).Pivot;

            bool finished = false;

            pivot.DOScale(endScale, .7f / speed).OnComplete(() => finished = true);

            yield return new WaitUntil(() => finished);

            finished = false;

            pivot.DOScale(originalScale, .7f / speed).OnComplete(() => finished = true);

            yield return new WaitUntil(() => finished);
        }
    }
}