using System.Collections.Generic;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.World;
using Varguiniano.YAPU.Runtime.World.Encounters;
using Varguiniano.YAPU.Runtime.World.OutOfBattleWeathers;

namespace Varguiniano.YAPU.Runtime.UI.Dex
{
    /// <summary>
    /// Data class that represents all the encounters a monster can have.
    /// </summary>
    public class SceneDexEncounterData
    {
        /// <summary>
        /// Encounters per type, per scene.
        /// </summary>
        public Dictionary<SceneInfoAsset, Dictionary<EncounterType, EncounterSetDexData>> Encounters = new();
    }

    /// <summary>
    /// Data class representing all the encounters that can be found on a set.
    /// </summary>
    public class EncounterSetDexData
    {
        /// <summary>
        /// Is it available at any time?
        /// </summary>
        public bool AvailableAtAnyTime;

        /// <summary>
        /// Is it available on any weather?
        /// </summary>
        public bool AvailableOnAnyWeather;

        /// <summary>
        /// Is it available at a specific time?
        /// </summary>
        public bool AvailableAtSpecificTime;

        /// <summary>
        /// Is it available under a specific weather?
        /// </summary>
        public bool AvailableUnderSpecificWeather;

        /// <summary>
        /// Moments in which this encounter is available.
        /// </summary>
        public List<DayMoment> AvailableMoments = new();

        /// <summary>
        /// Weathers in which this encounter is available.
        /// </summary>
        public List<OutOfBattleWeather> AvailableWeathers = new();
    }
}