using System;
using System.Collections.Generic;
using log4net;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Experience;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Natures;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.DataStructures;
using Random = UnityEngine.Random;

namespace Varguiniano.YAPU.Runtime.Monster
{
    /// <summary>
    /// Class to store the data related to a monster's stats.
    /// </summary>
    [Serializable]
    public struct MonsterStatData
    {
        /// <summary>
        /// Level this monster has.
        /// </summary>
        public byte Level;

        /// <summary>
        /// Nature of this monster.
        /// </summary>
        public Nature Nature;

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
        /// Constructor for the monster data stats.
        /// </summary>
        /// <param name="database">Reference to the database to use to create this monster.</param>
        /// <param name="level">Level of this monster.</param>
        /// <param name="nature">Nature of this monster. Optional, a random one will be chosen.</param>
        /// <param name="ivOverride">IVs for this monster. Optional, random ones will be chosen.</param>
        /// <param name="evOverride">EVs for this monster. Optional, default will set them as 0s.</param>
        internal MonsterStatData(MonsterDatabaseInstance database,
                                 byte level,
                                 Nature nature,
                                 SerializableDictionary<Stat, byte> ivOverride,
                                 SerializableDictionary<Stat, byte> evOverride)
        {
            Level = level;
            Nature = nature == null ? database.GetRandomNature() : nature;
            CurrentLevelExperience = 0;

            IndividualValues = new SerializableDictionary<Stat, byte>();

            if (ivOverride == null)
                foreach (Stat stat in Utils.GetAllItems<Stat>())
                    IndividualValues[stat] = (byte)Random.Range(0, 31);
            else
                foreach (Stat stat in Utils.GetAllItems<Stat>())
                    if (ivOverride.TryGetValue(stat, out byte value))
                        IndividualValues[stat] = value;
                    else
                        IndividualValues[stat] = (byte)Random.Range(0, 31);

            EffortValues = new SerializableDictionary<Stat, byte>();

            if (evOverride == null)
                foreach (Stat stat in Utils.GetAllItems<Stat>())
                    EffortValues[stat] = 0;
            else
                foreach (Stat stat in Utils.GetAllItems<Stat>())
                    if (evOverride.TryGetValue(stat, out byte value))
                        EffortValues[stat] = value;
                    else
                        EffortValues[stat] = 0;
        }

        /// <summary>
        /// Get the experience needed for next level.
        /// </summary>
        /// <param name="species">Monster species.</param>
        /// <param name="lookupTable">Lookup table to use.</param>
        /// <returns>The experience needed for the next level.</returns>
        public int GetExperienceForNextLevel(DataByFormEntry species, ExperienceLookupTable lookupTable) =>
            lookupTable.GetExperienceNeededForNextLevel(species.GrowthRate, Level);

        /// <summary>
        /// Raise the value of an EV by the given amount.
        /// </summary>
        /// <param name="settings">Reference to the settings.</param>
        /// <param name="virusData">This monster's virus data.</param>
        /// <param name="stat">Stat to raise.</param>
        /// <param name="amount">Amount to raise.</param>
        /// <param name="name">Name of the monster, for logging.</param>
        /// <param name="logger">Reference to the logger.</param>
        public void ChangeEV(YAPUSettings settings,
                             VirusData virusData,
                             Stat stat,
                             int amount,
                             string name,
                             ILog logger)
        {
            if (amount > 0 && virusData.GetsVirusEffect) amount *= 2;

            int clampedAmount = Mathf.Min(amount, byte.MaxValue - EffortValues[stat]);

            clampedAmount = Mathf.Max(clampedAmount, -EffortValues[stat]);

            if (clampedAmount == 0) return;

            uint currentTotal = GetTotalEVs();

            clampedAmount = (int)Mathf.Min(clampedAmount, settings.MaxEVPointsPerMonster - currentTotal);

            if (clampedAmount == 0) return;

            logger.Info(name + "'s " + stat + " EV changed by " + amount + ".");

            EffortValues[stat] = (byte)(EffortValues[stat] + clampedAmount);
        }

        /// <summary>
        /// Get the total number of EV points.
        /// </summary>
        /// <returns>The total number of EV points, duh.</returns>
        public uint GetTotalEVs()
        {
            uint amount = 0;

            foreach (KeyValuePair<Stat, byte> value in EffortValues) amount += value.Value;

            return amount;
        }

        /// <summary>
        /// Get the total number of IVs of this monster.
        /// </summary>
        public int GetTotalNumberOfIvs() =>
            IndividualValues[Stat.Hp]
          + IndividualValues[Stat.Attack]
          + IndividualValues[Stat.Defense]
          + IndividualValues[Stat.SpecialAttack]
          + IndividualValues[Stat.SpecialDefense]
          + IndividualValues[Stat.Speed];

        /// <summary>
        /// Deep clone this struct.
        /// </summary>
        /// <returns>A new deep clone.</returns>
        public MonsterStatData DeepClone() =>
            new()
            {
                Level = Level,
                Nature = Nature,
                CurrentLevelExperience = CurrentLevelExperience,
                IndividualValues = IndividualValues.ShallowClone(),
                EffortValues = EffortValues.ShallowClone()
            };
    }
}