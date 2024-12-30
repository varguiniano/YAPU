using System;

namespace Varguiniano.YAPU.Runtime.UI.Sorting
{
    /// <summary>
    /// Helper class to deal with sorting options.
    /// </summary>
    public static class SortHelper
    {
        /// <summary>
        /// Translate a sort option to a localizable string.
        /// </summary>
        /// <param name="sortOption">Sort option.</param>
        /// <returns>A localizable key.</returns>
        public static string SortOptionToLocalizableString(SortOption sortOption) =>
            sortOption switch
            {
                SortOption.DateAdded => "Menu/Pokemon/Cloud/SortAndFilter/Sort/AddedDate",
                SortOption.DexNumber => "Menu/Pokemon/Cloud/SortAndFilter/Sort/DexNumber",
                SortOption.Alphabetically => "Menu/Pokemon/Cloud/SortAndFilter/Sort/Alphabetically",
                SortOption.Level => "Common/Level",
                SortOption.TotalIVs => "Stats/TotalIvs",
                SortOption.HP => "Stats/Hp",
                SortOption.Attack => "Stats/Attack",
                SortOption.Defense => "Stats/Defense",
                SortOption.SpAttack => "Stats/SpecialAttack",
                SortOption.SpDefense => "Stats/SpecialDefense",
                SortOption.Speed => "Stats/Speed",
                SortOption.TotalBaseStats => "Stats/Base",
                _ => throw new ArgumentOutOfRangeException(nameof(sortOption), sortOption, null)
            };

        /// <summary>
        /// Translate a sort mode to a localizable string.
        /// </summary>
        /// <param name="sortMode">Sort mode.</param>
        /// <returns>A localizable key.</returns>
        public static string SortModeToLocalizableString(SortMode sortMode) =>
            sortMode switch
            {
                SortMode.Ascending => "Menu/Pokemon/Cloud/SortAndFilter/Sort/Ascending",
                SortMode.Descending => "Menu/Pokemon/Cloud/SortAndFilter/Sort/Descending",
                _ => throw new ArgumentOutOfRangeException(nameof(sortMode), sortMode, null)
            };
    }
}