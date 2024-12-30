using System;
using System.Collections.Generic;
using System.Linq;
using Varguiniano.YAPU.Runtime.Monster.Evolution;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Experience;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase
{
    /// <summary>
    /// Class with helper functions for the monster database.
    /// </summary>
    public static class MonsterDatabaseHelper
    {
        /// <summary>
        /// Deep clone an evolution data list.
        /// </summary>
        /// <param name="original">Original list.</param>
        /// <returns>Deep cloned list.</returns>
        public static List<EvolutionData> DeepClone(this IEnumerable<EvolutionData> original) =>
            original.Select(data => data.Clone()).ToList();

        /// <summary>
        /// Get the localizable key for a growth rate.
        /// </summary>
        /// <param name="growthRate">Growth rate to the get the key from.</param>
        /// <returns>The localizable key.</returns>
        public static string GetLocalizableKey(this GrowthRate growthRate) => "Stats/GrowthRate/" + growthRate;
    }
}