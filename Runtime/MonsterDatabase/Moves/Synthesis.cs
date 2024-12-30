using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move synthesis.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Grass/Synthesis", fileName = "Synthesis")]
    public class Synthesis : HPPercentageRegenMove
    {
        /// <summary>
        /// Percentage of HP to regen.
        /// </summary>
        [FoldoutGroup("Effect")]
        [Range(0, 1)]
        public float DefaultRegenPercentage;

        /// <summary>
        /// Percentage of HP to regen.
        /// </summary>
        [FoldoutGroup("Effect")]
        [Range(0, 1)]
        public float NoWeatherRegenPercentage;

        /// <summary>
        /// Percentage of HP to regen on specific weather conditions.
        /// </summary>
        [FoldoutGroup("Effect")]
        public SerializableDictionary<Weather, float> PercentagePerWeather;

        /// <summary>
        /// Get the percentage of HP to regen.
        /// </summary>
        /// <param name="battleManager"></param>
        /// <param name="userType"></param>
        /// <param name="userIndex"></param>
        /// <param name="targetType"></param>
        /// <param name="targetIndex"></param>
        /// <returns>A number between 0 and 1.</returns>
        protected override float GetRegenPercentage(BattleManager battleManager,
                                                    BattlerType userType,
                                                    int userIndex,
                                                    BattlerType targetType,
                                                    int targetIndex)
        {
            if (!battleManager.Scenario.GetWeather(out Weather weather)) return NoWeatherRegenPercentage;

            foreach (KeyValuePair<Weather, float> pair in PercentagePerWeather)
                if (pair.Key == weather)
                    return pair.Value;

            return DefaultRegenPercentage;
        }
    }
}