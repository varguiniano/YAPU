namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Stats
{
    /// <summary>
    /// Class with utility functions for stats.
    /// </summary>
    public static class StatUtils
    {
        /// <summary>
        /// Prefix for the stat localization keys.
        /// </summary>
        private const string StatsLocalizationPrefix = "Stats/";

        /// <summary>
        /// Prefix for the Condition localization keys.
        /// </summary>
        private const string ConditionsLocalizationPrefix = "Conditions/";

        /// <summary>
        /// Get the localization string of a stat.
        /// </summary>
        /// <param name="stat">Stat to get the string from.</param>
        /// <returns>The localization string.</returns>
        public static string GetLocalizationString(this Stat stat) => StatsLocalizationPrefix + stat;

        /// <summary>
        /// Get the localization string of a stat.
        /// </summary>
        /// <param name="stat">Stat to get the string from.</param>
        /// <returns>The localization string.</returns>
        public static string GetLocalizationString(this BattleStat stat) => StatsLocalizationPrefix + stat;

        /// <summary>
        /// Get the localization string of a stat.
        /// </summary>
        /// <param name="condition">Condition to get the string from.</param>
        /// <returns>The localization string.</returns>
        public static string GetLocalizationString(this Condition condition) =>
            ConditionsLocalizationPrefix + condition;
    }
}