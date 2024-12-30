using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move Growth.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/Growth", fileName = "Growth")]
    public class Growth : StageChangeMove
    {
        /// <summary>
        /// Weathers that double the effectiveness.
        /// </summary>
        [SerializeField]
        private List<Weather> DoublingWeathers;

        /// <summary>
        /// Reference to the Growth audio.
        /// </summary>
        [SerializeField]
        private AudioReference Audio;

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
            AudioManager.Instance.PlayAudio(Audio, pitch: speed);

            float originalScale = userPosition.localScale.x;
            float endScale = originalScale * 1.3f;

            bool finished = false;

            userPosition.DOScale(endScale, .7f / speed).OnComplete(() => finished = true);

            yield return new WaitUntil(() => finished);

            finished = false;

            userPosition.DOScale(originalScale, .7f / speed).OnComplete(() => finished = true);

            yield return new WaitUntil(() => finished);
        }

        /// <summary>
        /// Double on sunny weather.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">Type of the move user.</param>
        /// <param name="userIndex">In battle index of the user.</param>
        /// <param name="targetType">Current target.</param>
        /// <param name="targetIndex">Current target,</param>
        /// <param name="targets">Move targets.</param>
        /// <returns>The multiplier to apply.</returns>
        protected override float GetStageChangeMultiplier(BattleManager battleManager,
                                                          BattlerType userType,
                                                          int userIndex,
                                                          BattlerType targetType,
                                                          int targetIndex,
                                                          List<(BattlerType Type, int Index)> targets)
        {
            if (battleManager.Scenario.GetWeather(out Weather weather) && DoublingWeathers.Contains(weather)) return 2;

            return base.GetStageChangeMultiplier(battleManager, userType, userIndex, targetType, targetIndex, targets);
        }
    }
}