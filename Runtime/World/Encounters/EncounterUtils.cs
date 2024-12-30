using System.Collections.Generic;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.UI.Dex;
using Varguiniano.YAPU.Runtime.World.OutOfBattleWeathers;
using WhateverDevs.Core.Runtime.Common;

namespace Varguiniano.YAPU.Runtime.World.Encounters
{
    /// <summary>
    /// Static class for encounter related utilities.
    /// </summary>
    public static class EncounterUtils
    {
        /// <summary>
        /// Translate an encounter type to a localizable string.
        /// </summary>
        public static string ToLocalizableString(this EncounterType encounterType) => "EncounterType/" + encounterType;

        /// <summary>
        /// Merge a list of encounter data into a single one with all options.
        /// </summary>
        public static EncounterSetDexData MergeIntoOne(this List<EncounterSetDexData> list)
        {
            EncounterSetDexData merge = new();

            foreach (EncounterSetDexData data in list)
            {
                merge.AvailableAtAnyTime |= data.AvailableAtAnyTime;
                merge.AvailableOnAnyWeather |= data.AvailableOnAnyWeather;

                if (!merge.AvailableAtAnyTime && data.AvailableAtSpecificTime)
                {
                    merge.AvailableAtSpecificTime = data.AvailableAtSpecificTime;

                    foreach (DayMoment moment in data.AvailableMoments) merge.AvailableMoments.AddIfNew(moment);
                }

                // ReSharper disable once InvertIf
                if (!merge.AvailableOnAnyWeather && data.AvailableUnderSpecificWeather)
                {
                    merge.AvailableUnderSpecificWeather = data.AvailableUnderSpecificWeather;

                    foreach (OutOfBattleWeather weather in data.AvailableWeathers)
                        merge.AvailableWeathers.AddIfNew(weather);
                }
            }

            return merge;
        }
    }
}