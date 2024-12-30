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
    /// Class representing the move Growl.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/Growl", fileName = "Growl")]
    public class Growl : StageChangeMove
    {
        /// <summary>
        /// Reference to the audio to play when growling.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference GrowlSound;

        /// <summary>
        /// Duration of the animation.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private float Duration = 1.6f;

        /// <summary>
        /// Play the growl animation.
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
            AudioManager.Instance.PlayAudio(GrowlSound, pitch: speed);

            float third = Duration / 3f;

            float twoThirds = third * 2;

            bool finished = false;

            Vector3 originalScale = userPosition.localScale;
            Vector3 biggerScale = originalScale * 1.1f;

            userPosition.DOScale(biggerScale, twoThirds / speed)
                        .OnComplete(() => userPosition.DOScale(originalScale, third / speed)
                                                      .OnComplete(() => finished = true));

            foreach (Transform targetPosition in targetPositions)
                targetPosition.DOShakePosition(Duration / speed, .05f);

            yield return new WaitUntil(() => finished);
        }
    }
}