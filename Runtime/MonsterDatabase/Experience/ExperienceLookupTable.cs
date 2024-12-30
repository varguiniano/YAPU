using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Experience
{
    /// <summary>
    /// Look up table to know how much experience a monster has based on its level and how much does it need for the next.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/ExperienceLookUpTable", fileName = "ExperienceLookUpTable")]
    public class ExperienceLookupTable : WhateverScriptable<ExperienceLookupTable>
    {
        /// <summary>
        /// Lookup table for experience for each level.
        /// </summary>
        public SerializableDictionary<GrowthRate, SerializableDictionary<int, int>> CurrentExperienceTable;

        /// <summary>
        /// Retrieve the base experience for the current level.
        /// </summary>
        /// <param name="growthRate">The growth rate of the monster.</param>
        /// <param name="level">Its level.</param>
        /// <returns>The base experience of that level.</returns>
        public int GetBaseExperienceForLevel(GrowthRate growthRate, int level) =>
            CurrentExperienceTable[growthRate][level];

        /// <summary>
        /// Get the experience needed for the next level.
        /// </summary>
        /// <param name="growthRate">The growth rate of the monster.</param>
        /// <param name="level">Its level.</param>
        /// <returns>The experience needed for the next level.</returns>
        public int GetExperienceNeededForNextLevel(GrowthRate growthRate, int level)
        {
            if (level > 99) return -1;
            return GetBaseExperienceForLevel(growthRate, level + 1) - GetBaseExperienceForLevel(growthRate, level);
        }

        /// <summary>
        /// Load the lookup table values from a tsv file.
        /// </summary>
        /// <param name="path">Path to that file.</param>
        [Button]
        private void LoadFromTsvFile(string path)
        {
            CurrentExperienceTable = new SerializableDictionary<GrowthRate, SerializableDictionary<int, int>>();

            foreach (GrowthRate growthRate in Utils.GetAllItems<GrowthRate>())
                CurrentExperienceTable[growthRate] = new SerializableDictionary<int, int>();

            foreach (string line in File.ReadAllLines(path))
            {
                string[] values = line.Replace(",", string.Empty).Split('\t');

                Logger.Info(values);

                CurrentExperienceTable[GrowthRate.Erratic][int.Parse(values[6])] = int.Parse(values[0]);
                CurrentExperienceTable[GrowthRate.Fast][int.Parse(values[6])] = int.Parse(values[1]);
                CurrentExperienceTable[GrowthRate.MediumFast][int.Parse(values[6])] = int.Parse(values[2]);
                CurrentExperienceTable[GrowthRate.MediumSlow][int.Parse(values[6])] = int.Parse(values[3]);
                CurrentExperienceTable[GrowthRate.Slow][int.Parse(values[6])] = int.Parse(values[4]);
                CurrentExperienceTable[GrowthRate.Fluctuating][int.Parse(values[6])] = int.Parse(values[5]);
            }
        }
    }
}