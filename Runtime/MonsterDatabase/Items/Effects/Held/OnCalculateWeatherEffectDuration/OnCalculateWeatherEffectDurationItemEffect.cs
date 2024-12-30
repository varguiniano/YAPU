using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCalculateWeatherEffectDuration
{
    /// <summary>
    /// Data class for an item effect that modifies the duration of a weather effect. 
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/OnCalculateWeatherEffectDuration/BasicModifier",
                     fileName = "OnCalculateWeatherEffectDuration")]
    public class
        OnCalculateWeatherEffectDurationItemEffect : MonsterDatabaseScriptable<
            OnCalculateWeatherEffectDurationItemEffect>
    {
        /// <summary>
        /// Durations for specific weathers.
        /// </summary>
        [SerializeField]
        private SerializableDictionary<Weather, int> CustomDurations;

        /// <summary>
        /// Calculate the weather duration of the given weather.
        /// -2 if not modified.
        /// </summary>
        /// <param name="item">Item that has the effect.</param>
        /// <param name="user">Owner of the item.</param>
        /// <param name="weather">Weather.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The duration to have.</returns>
        public int CalculateWeatherDuration(Item item, Battler user, Weather weather, BattleManager battleManager)
        {
            if (!CustomDurations.TryGetValue(weather, out int duration)) return -2;
            item.ShowItemNotification(user, battleManager.Localizer);
            return duration;
        }
    }
}