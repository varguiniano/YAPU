namespace Varguiniano.YAPU.Runtime.GameFlow
{
    /// <summary>
    /// Static class with time related utilities.
    /// </summary>
    public static class TimeUtils
    {
        /// <summary>
        /// Transform a day moment into a localizable key.
        /// </summary>
        public static string ToLocalizableKey(this DayMoment moment) =>
            moment switch
            {
                DayMoment.Night => "DayMoments/Night",
                DayMoment.Dawn => "DayMoments/Dawn",
                DayMoment.Day => "DayMoments/Day",
                DayMoment.Dusk => "DayMoments/Dusk",
                _ => "Error"
            };
        
        /// <summary>
        /// Transform a day moment into a localizable key.
        /// </summary>
        public static string AtTimeToLocalizableKey(this DayMoment moment) =>
            moment switch
            {
                DayMoment.Night => "DayMoments/AtNight",
                DayMoment.Dawn => "DayMoments/AtDawn",
                DayMoment.Day => "DayMoments/AtDay",
                DayMoment.Dusk => "DayMoments/AtDusk",
                _ => "Error"
            };
    }
}