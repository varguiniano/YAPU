using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.UI.Dex;
using Varguiniano.YAPU.Runtime.World.OutOfBattleWeathers;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.World.Encounters
{
    /// <summary>
    /// Data structure that represents a set of encounters that can appear for a specific encounter type in a specific scene.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Battle/Encounters/WildEncountersSet", fileName = "WildEncountersSet")]
    public class WildEncountersSet : WhateverScriptable<WildEncountersSet>
    {
        /// <summary>
        /// Monsters that always show up.
        /// </summary>
        [SerializeField]
        private List<WildEncounter> BaseEncounters;

        /// <summary>
        /// Monsters that show up at specific times.
        /// </summary>
        [SerializeField]
        [Tooltip("Monsters that show up at specific times.")]
        private SerializableDictionary<DayMoment, List<WildEncounter>> EncountersPerMoment;

        /// <summary>
        /// Monsters that show up under specific weathers.
        /// </summary>
        [SerializeField]
        [Tooltip("Monsters that show up under specific weathers.")]
        private SerializableDictionary<OutOfBattleWeather, List<WildEncounter>> EncountersPerWeather;

        /// <summary>
        /// Get the encounters possible for a specific type, moment and weather.
        /// </summary>
        /// <param name="moment">Current day moment.</param>
        /// <param name="weather">Current weather.</param>
        /// <returns>A list of all the possible encounters.</returns>
        public List<WildEncounter> GetWildEncounters(DayMoment moment, OutOfBattleWeather weather)
        {
            List<WildEncounter> encounters = new(BaseEncounters);

            if (EncountersPerMoment.TryGetValue(moment, out List<WildEncounter> momentEncounter))
                encounters.AddRange(momentEncounter);

            if (EncountersPerWeather.TryGetValue(weather, out List<WildEncounter> weatherEncounter))
                encounters.AddRange(weatherEncounter);

            return encounters;
        }

        /// <summary>
        /// Get all monsters and forms possible for dex displaying.
        /// </summary>
        public Dictionary<(MonsterEntry, Form), EncounterSetDexData> GetPossibleDexEncounters()
        {
            Dictionary<(MonsterEntry, Form), EncounterSetDexData> encounters = new();

            AddPossibleDexBaseEncounters(ref encounters);

            AddPossibleDexTimeEncounters(ref encounters);

            AddPossibleDexWeatherEncounters(ref encounters);

            return encounters;
        }

        /// <summary>
        /// Add the possible base encounters to the dex data list.
        /// </summary>
        private void AddPossibleDexBaseEncounters(ref Dictionary<(MonsterEntry, Form), EncounterSetDexData> encounters)
        {
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (WildEncounter baseEncounter in BaseEncounters)
            {
                foreach ((MonsterEntry, Form) possibleEncounter in baseEncounter.GetPossibleDexEncounters())
                {
                    if (encounters.ContainsKey(possibleEncounter)) continue;

                    EncounterSetDexData data = new()
                                               {
                                                   AvailableAtAnyTime = true,
                                                   AvailableOnAnyWeather = true
                                               };

                    encounters[possibleEncounter] = data;
                }
            }
        }

        /// <summary>
        /// Add the possible time encounters to the dex data list.
        /// </summary>
        private void AddPossibleDexTimeEncounters(ref Dictionary<(MonsterEntry, Form), EncounterSetDexData> encounters)
        {
            foreach (KeyValuePair<DayMoment, List<WildEncounter>> momentSlot in EncountersPerMoment)
            {
                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (WildEncounter wildEncounterData in momentSlot.Value)
                {
                    foreach ((MonsterEntry, Form) possibleEncounter in wildEncounterData.GetPossibleDexEncounters())
                    {
                        EncounterSetDexData data =
                            encounters.TryGetValue(possibleEncounter, out EncounterSetDexData previousData)
                                ? previousData
                                : new EncounterSetDexData();

                        data.AvailableMoments ??= new List<DayMoment>();

                        data.AvailableAtSpecificTime = true;
                        data.AvailableMoments.AddIfNew(momentSlot.Key);

                        encounters[possibleEncounter] = data;
                    }
                }
            }
        }

        /// <summary>
        /// Add the possible time encounters to the dex data list.
        /// </summary>
        private void AddPossibleDexWeatherEncounters(
            ref Dictionary<(MonsterEntry, Form), EncounterSetDexData> encounters)
        {
            foreach (KeyValuePair<OutOfBattleWeather, List<WildEncounter>> weatherSlot in EncountersPerWeather)
            {
                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (WildEncounter wildEncounterData in weatherSlot.Value)
                {
                    foreach ((MonsterEntry, Form) possibleEncounter in wildEncounterData.GetPossibleDexEncounters())
                    {
                        EncounterSetDexData data =
                            encounters.TryGetValue(possibleEncounter, out EncounterSetDexData previousData)
                                ? previousData
                                : new EncounterSetDexData();

                        data.AvailableWeathers ??= new List<OutOfBattleWeather>();

                        data.AvailableUnderSpecificWeather = true;
                        data.AvailableWeathers.AddIfNew(weatherSlot.Key);

                        encounters[possibleEncounter] = data;
                    }
                }
            }
        }

        
    }
}