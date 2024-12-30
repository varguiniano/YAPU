using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move Struggle.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/General/Struggle", fileName = "Struggle")]
    public class Struggle : DamageWithRecoilBasedOnMaxHPMove
    {
        /// <summary>
        /// Reference to the hit prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private BasicSpriteAnimation HitPrefab;

        /// <summary>
        /// Amount to make the use scale.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private float ScaleShift;

        /// <summary>
        /// Sound of this move.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Sound;

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
            bool moved = false;

            AudioManager.Instance.PlayAudio(Sound, pitch: speed);

            userPosition.DOShakeScale(1f / speed, ScaleShift).OnComplete(() => moved = true);

            yield return new WaitForSeconds(.5f / speed);

            BasicSpriteAnimation hit = Instantiate(HitPrefab, targetPositions[0].position, Quaternion.identity);

            yield return WaitAFrame;

            yield return hit.PlayAnimation(speed);

            yield return new WaitUntil(() => moved);

            Destroy(hit.gameObject);
        }

        /// <summary>
        /// Calculate the damage of a move.
        /// Based on: https://bulbapedia.bulbagarden.net/wiki/Stat#Generations_III_onward
        /// But modified for the struggle specifics: https://bulbapedia.bulbagarden.net/wiki/Struggle_(move)
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="hitNumber"></param>
        /// <param name="totalHits"></param>
        /// <param name="isCritical">Is it a critical move?</param>
        /// <param name="typeEffectiveness">Type effectiveness.</param>
        /// <param name="externalPowerMultiplier">External multiplier applied to the power.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="allTargets">All of the move's targets.</param>
        /// <param name="finished">Callback with the amount of damage it deals.</param>
        protected override IEnumerator CalculateMoveDamage(BattleManager battleManager,
                                                           ILocalizer localizer,
                                                           Battler user,
                                                           Battler target,
                                                           int hitNumber,
                                                           int totalHits,
                                                           bool isCritical,
                                                           float typeEffectiveness,
                                                           float externalPowerMultiplier,
                                                           bool ignoresAbilities,
                                                           List<(BattlerType Type, int Index)> allTargets,
                                                           Action<float> finished)
        {
            Stat attackStat = GetMoveCategory(user, target, ignoresAbilities, battleManager) == Category.Physical
                                  ? Stat.Attack
                                  : Stat.SpecialAttack;

            Stat defenseStat = GetMoveCategory(user, target, ignoresAbilities, battleManager) == Category.Physical
                                   ? Stat.Defense
                                   : Stat.SpecialDefense;

            float attack = MonsterMathHelper.CalculateStat(user, attackStat, battleManager)
                         * MonsterMathHelper.GetStageMultiplier(user,
                                                                attackStat,
                                                                isCritical);

            float defense = MonsterMathHelper.CalculateStat(target, defenseStat, battleManager)
                          * MonsterMathHelper.GetStageMultiplier(target,
                                                                 defenseStat,
                                                                 isCritical);

            float attackDefense = attack / defense;

            float numerator = (2 * user.StatData.Level / 5f + 2)
                            * GetMovePowerInBattle(battleManager, user, target, ignoresAbilities)
                            * attackDefense;

            float baseMultiplier = numerator / 50 + 2;

            float targets = allTargets.Count > 1 ? .75f : 1;

            float weather = battleManager.Scenario.GetWeather(out Weather currentWeather)
                                ? currentWeather.CalculateMovesDamageMultiplier(this,
                                                                                    user,
                                                                                    battleManager)
                                : 1f;

            float terrain = battleManager.Scenario.Terrain != null
                                ? battleManager.Scenario.Terrain.CalculateMovesDamageMultiplier(this,
                                         user,
                                         target,
                                         battleManager)
                                : 1f;

            float critical = isCritical ? 1.5f : 1;
            float random = battleManager.RandomProvider.Range(.85f, 1);

            float status = GetMoveCategory(user, target, ignoresAbilities, battleManager) == Category.Physical
                        && BurnEffectApplies
                               ? user.GetStatus() == null
                                     ? 1f
                                     : user.GetStatus().OnCalculateMoveDamage(this, user, battleManager)
                               : 1f;

            float additional = 1f;

            if (target.CanUseHeldItemInBattle(battleManager))
            {
                bool consumeItem = false;

                yield return target.HeldItem.OnCalculateMoveDamageWhenUsing(this,
                                                                            additional,
                                                                            user,
                                                                            target,
                                                                            battleManager,
                                                                            localizer,
                                                                            (shouldConsume, newMultiplier) =>
                                                                            {
                                                                                if (shouldConsume) consumeItem = true;
                                                                                additional = newMultiplier;
                                                                            });

                if (consumeItem) yield return user.ConsumeItemInBattle(battleManager);
            }

            finished.Invoke(baseMultiplier
                          * targets
                          * weather
                          * terrain
                          * critical
                          * random
                          * typeEffectiveness
                          * status
                          * additional);
        }
    }
}