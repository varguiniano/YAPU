using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for an ability that boosts a stat of the monster on certain weathers.
    /// </summary>
    public abstract class BoostStatOnWeatherAbility : Ability
    {
        /// <summary>
        /// Stat to affect.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private Stat Stat;

        /// <summary>
        /// Weathers and the multipliers to apply to them.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        protected SerializableDictionary<Weather, float> MultiplierPerWeather;

        /// <summary>
        /// Called when calculating a stat of the monster that has this ability.
        /// </summary>
        /// <param name="monster">Monster that has the ability.</param>
        /// <param name="stat">Stat to calculate.</param>
        /// <param name="battleManager">Reference to the battle manager if we are in battle.</param>
        /// <returns>A multiplier to apply to that stat.</returns>
        public override float OnCalculateStat(MonsterInstance monster, Stat stat, BattleManager battleManager)
        {
            if (stat != Stat || battleManager == null || !battleManager.Scenario.GetWeather(out Weather weather))
                return 1;

            foreach (KeyValuePair<Weather, float> pair in MultiplierPerWeather)
                if (pair.Key == weather)
                    return pair.Value;

            return 1;
        }
    }
}