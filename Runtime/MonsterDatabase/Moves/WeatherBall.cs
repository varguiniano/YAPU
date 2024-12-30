using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move WeatherBall.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/WeatherBall", fileName = "WeatherBall")]
    public class WeatherBall : DamageMove
    {
        /// <summary>
        /// Types of this move in specific weathers.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializableDictionary<Weather, MonsterType> WeatherTypeOverrides;

        /// <summary>
        /// Reference to the ball prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Transform BallPrefab;

        /// <summary>
        /// Animation audio.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

        /// <summary>
        /// Get the type of this move in battle.
        /// </summary>
        /// <param name="battler">Owner of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The type of the move.</returns>
        public override MonsterType GetMoveTypeInBattle(Battler battler, BattleManager battleManager) =>
            battleManager.Scenario.GetWeather(out Weather weather) && WeatherTypeOverrides.ContainsKey(weather)
                ? WeatherTypeOverrides[weather]
                : base.GetMoveTypeInBattle(battler, battleManager);

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
            foreach (Transform targetPosition in targetPositions)
            {
                Transform ball = Instantiate(BallPrefab, userPosition.position, Quaternion.identity, null);

                yield return WaitAFrame;

                SpriteRenderer ballSprite = ball.GetComponent<SpriteRenderer>();
                VisualEffect ballFX = ball.GetComponentInChildren<VisualEffect>();

                AudioManager.Instance.PlayAudio(Audio, pitch: speed);

                bool finished = false;

                ball.DOMove(targetPosition.position, 1.1f / battleManager.BattleSpeed)
                    .OnComplete(() => finished = true);

                yield return new WaitUntil(() => finished);

                ballSprite.DOFade(0, .1f / battleManager.BattleSpeed);
                ballFX.Stop();

                DOVirtual.DelayedCall(3f, () => Destroy(ball.gameObject));
            }
        }

        /// <summary>
        /// Get the move's power.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="hitNumber"></param>
        /// <returns>The move's power.</returns>
        public override int GetMovePowerInBattle(BattleManager battleManager,
                                                 Battler user,
                                                 Battler target,
                                                 bool ignoresAbilities,
                                                 int hitNumber = 0) =>
            (battleManager.Scenario.GetWeather(out Weather weather) && WeatherTypeOverrides.ContainsKey(weather)
                 ? 2
                 : 1)
          * base.GetMovePowerInBattle(battleManager, user, target, ignoresAbilities, hitNumber);
    }
}