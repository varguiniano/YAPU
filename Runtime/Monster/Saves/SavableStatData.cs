using System;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.Monster.Saves
{
    /// <summary>
    /// Version of the stat data class that can be serialized as a string.
    /// </summary>
    [Serializable]
    public class SavableStatData
    {
        /// <summary>
        /// Level this monster has.
        /// </summary>
        public byte Level;

        /// <summary>
        /// Nature of this monster.
        /// </summary>
        public string Nature;

        /// <summary>
        /// Current experience this monster has.
        /// </summary>
        public uint CurrentLevelExperience;

        /// <summary>
        /// Individual values of this monster.
        /// </summary>
        public SerializableDictionary<Stat, byte> IndividualValues;

        /// <summary>
        /// Effort values of this monster.
        /// </summary>
        public SerializableDictionary<Stat, byte> EffortValues;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="statData">Original stat data class.</param>
        public SavableStatData(MonsterStatData statData)
        {
            Level = statData.Level;
            Nature = statData.Nature.name;
            CurrentLevelExperience = statData.CurrentLevelExperience;
            IndividualValues = statData.IndividualValues;
            EffortValues = statData.EffortValues;
        }
    }
}