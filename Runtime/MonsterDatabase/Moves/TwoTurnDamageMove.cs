using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Base class for a two turn move that deals damage.
    /// </summary>
    public abstract class TwoTurnDamageMove : TwoTurnMove
    {
        /// <summary>
        /// Weathers that change the damage multiplier.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializableDictionary<Weather, float> WeatherDamageMultipliers;

        /// <summary>
        /// Execute the effect for the second turn of this move.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="user">Direct reference to the user of the move.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="hitNumber">This is the number of hits already made in this move. It will be always 0 unless it's a multihit move.</param>
        /// <param name="expectedHits">Expected hits for this move.</param>
        /// <param name="externalPowerMultiplier">External multiplier applied to the power.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="finishedCallback">Callback stating if the move successfully executed.</param>
        public override IEnumerator ExecuteSecondEffect(BattleManager battleManager,
                                                        ILocalizer localizer,
                                                        BattlerType userType,
                                                        int userIndex,
                                                        Battler user,
                                                        List<(BattlerType Type, int Index)> targets,
                                                        int hitNumber,
                                                        int expectedHits,
                                                        float externalPowerMultiplier,
                                                        bool ignoresAbilities,
                                                        Action<bool> finishedCallback)
        {
            yield return ExecuteDamageEffect(battleManager,
                                             localizer,
                                             userType,
                                             userIndex,
                                             user,
                                             targets,
                                             hitNumber,
                                             expectedHits,
                                             externalPowerMultiplier,
                                             ignoresAbilities,
                                             finishedCallback);

            battleManager.Statuses.ScheduleRemoveStatus(BuildUpStatus, user);
        }

        /// <summary>
        /// Calculate the damage of a move.
        /// Based on: https://bulbapedia.bulbagarden.net/wiki/Stat#Generations_III_onward
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
            float damage = 0;

            yield return base.CalculateMoveDamage(battleManager,
                                                  localizer,
                                                  user,
                                                  target,
                                                  hitNumber,
                                                  totalHits,
                                                  isCritical,
                                                  typeEffectiveness,
                                                  externalPowerMultiplier,
                                                  ignoresAbilities,
                                                  allTargets,
                                                  damageToDeal => damage = damageToDeal);

            if (battleManager.Scenario.GetWeather(out Weather weather)
             && WeatherDamageMultipliers.TryGetValue(weather, out float multiplier))
                damage *= multiplier;

            finished.Invoke(damage);
        }
    }
}