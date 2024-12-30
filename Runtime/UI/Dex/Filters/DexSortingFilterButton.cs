using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.UI.Sorting;

namespace Varguiniano.YAPU.Runtime.UI.Dex.Filters
{
    /// <summary>
    /// Controller for the button that applies sorting.
    /// </summary>
    public class DexSortingFilterButton : FilterButton
    {
        /// <summary>
        /// Option selected.
        /// </summary>
        [NonSerialized]
        public SortOption Option = SortOption.DateAdded;

        /// <summary>
        /// Mode to sort by.
        /// </summary>
        [NonSerialized]
        public SortMode Mode = SortMode.Descending;

        /// <summary>
        /// Available options for sorting.
        /// </summary>
        private readonly SortOption[] availableOptions =
        {
            SortOption.DexNumber,
            SortOption.Alphabetically,
            SortOption.TotalBaseStats,
            SortOption.HP,
            SortOption.Attack,
            SortOption.Defense,
            SortOption.SpAttack,
            SortOption.SpDefense,
            SortOption.Speed,
        };

        /// <summary>
        /// Set an external option and mode.
        /// </summary>
        /// <param name="newOption">New option.</param>
        /// <param name="newMode">New mode.</param>
        public void SetOptionAndMode(SortOption newOption, SortMode newMode)
        {
            Option = newOption;
            Mode = newMode;
            UpdateText();
        }

        /// <summary>
        /// Sort by the selected filter.
        /// Anything other than date added will only show seen monsters.
        /// </summary>
        /// <param name="originalEntries">Original dex entries.</param>
        /// <returns>Sorted list.</returns>
        // ReSharper disable once CyclomaticComplexity
        public override IEnumerable<(MonsterDexEntry, FormDexEntry, MonsterGender)>
            ApplyFilter(IEnumerable<(MonsterDexEntry, FormDexEntry, MonsterGender)> originalEntries) =>
            Option switch
            {
                SortOption.DexNumber when Mode == SortMode.Ascending =>
                    originalEntries.OrderBy(tuple => tuple.Item1.Species.DexNumber),
                SortOption.DexNumber => originalEntries.OrderByDescending(tuple => tuple.Item1.Species.DexNumber),
                SortOption.Alphabetically when Mode == SortMode.Ascending =>
                    originalEntries.Where(HasTupleBeenSeen)
                                   .OrderBy(tuple => Localizer[tuple.Item1.Species.LocalizableName]),
                SortOption.Alphabetically =>
                    originalEntries.Where(HasTupleBeenSeen)
                                   .OrderByDescending(tuple => Localizer[tuple.Item1.Species.LocalizableName]),
                SortOption.TotalBaseStats when Mode == SortMode.Ascending =>
                    originalEntries.Where(HasTupleBeenSeen)
                                   .OrderBy(tuple => tuple.Item1.Species[tuple.Item2.Form].TotalBaseStats),
                SortOption.TotalBaseStats => originalEntries.Where(HasTupleBeenSeen)
                                                            .OrderByDescending(tuple => tuple.Item1
                                                                                  .Species[tuple.Item2.Form]
                                                                                  .TotalBaseStats),
                SortOption.HP when Mode == SortMode.Ascending => originalEntries
                                                                .Where(HasTupleBeenSeen)
                                                                .OrderBy(tuple => tuple.Item1
                                                                            .Species[tuple.Item2.Form]
                                                                            .BaseStats[Stat.Hp]),
                SortOption.HP => originalEntries.Where(HasTupleBeenSeen)
                                                .OrderByDescending(tuple => tuple.Item1.Species[tuple.Item2.Form]
                                                                      .BaseStats[Stat.Hp]),
                SortOption.Attack when Mode == SortMode.Ascending => originalEntries
                                                                    .Where(HasTupleBeenSeen)
                                                                    .OrderBy(tuple => tuple.Item1
                                                                                .Species[tuple.Item2.Form]
                                                                                .BaseStats[Stat.Attack]),
                SortOption.Attack => originalEntries.Where(HasTupleBeenSeen)
                                                    .OrderByDescending(tuple => tuple.Item1.Species[tuple.Item2.Form]
                                                                          .BaseStats[Stat.Attack]),
                SortOption.Defense when Mode == SortMode.Ascending => originalEntries
                                                                     .Where(HasTupleBeenSeen)
                                                                     .OrderBy(tuple => tuple.Item1
                                                                                 .Species[tuple.Item2.Form]
                                                                                 .BaseStats[Stat.Defense]),
                SortOption.Defense => originalEntries.Where(HasTupleBeenSeen)
                                                     .OrderByDescending(tuple => tuple.Item1.Species[tuple.Item2.Form]
                                                                           .BaseStats[Stat.Defense]),
                SortOption.SpAttack when Mode == SortMode.Ascending => originalEntries
                                                                      .Where(HasTupleBeenSeen)
                                                                      .OrderBy(tuple => tuple.Item1
                                                                                  .Species[tuple.Item2.Form]
                                                                                  .BaseStats[Stat.SpecialAttack]),
                SortOption.SpAttack => originalEntries.Where(HasTupleBeenSeen)
                                                      .OrderByDescending(tuple => tuple.Item1.Species[tuple.Item2.Form]
                                                                            .BaseStats[Stat.SpecialAttack]),
                SortOption.SpDefense when Mode == SortMode.Ascending => originalEntries
                                                                       .Where(HasTupleBeenSeen)
                                                                       .OrderBy(tuple => tuple.Item1
                                                                                   .Species[tuple.Item2.Form]
                                                                                   .BaseStats
                                                                                        [Stat.SpecialDefense]),
                SortOption.SpDefense => originalEntries.Where(HasTupleBeenSeen)
                                                       .OrderByDescending(tuple => tuple.Item1.Species[tuple.Item2.Form]
                                                                             .BaseStats[Stat.SpecialDefense]),
                SortOption.Speed when Mode == SortMode.Ascending => originalEntries
                                                                   .Where(HasTupleBeenSeen)
                                                                   .OrderBy(tuple => tuple.Item1
                                                                               .Species[tuple.Item2.Form]
                                                                               .BaseStats[Stat.Speed]),
                SortOption.Speed => originalEntries.Where(HasTupleBeenSeen)
                                                   .OrderByDescending(tuple => tuple.Item1.Species[tuple.Item2.Form]
                                                                         .BaseStats[Stat.Speed]),
                _ => throw new ArgumentOutOfRangeException()
            };

        /// <summary>
        /// Select the new sorting via choice menu.
        /// </summary>
        public override void OnButtonSelected() => StartCoroutine(ShowSortOptionsAfterAFrame());

        /// <summary>
        /// Select the new sorting via choice menu.
        /// </summary>
        private IEnumerator ShowSortOptionsAfterAFrame()
        {
            yield return WaitAFrame;
            yield return WaitAFrame;

            DialogManager.ShowChoiceMenu(new List<string>
                                         {
                                             SortHelper.SortOptionToLocalizableString(SortOption.DexNumber),
                                             SortHelper.SortOptionToLocalizableString(SortOption.Alphabetically),
                                             SortHelper.SortOptionToLocalizableString(SortOption.TotalBaseStats),
                                             SortHelper.SortOptionToLocalizableString(SortOption.HP),
                                             SortHelper.SortOptionToLocalizableString(SortOption.Attack),
                                             SortHelper.SortOptionToLocalizableString(SortOption.Defense),
                                             SortHelper.SortOptionToLocalizableString(SortOption.SpAttack),
                                             SortHelper.SortOptionToLocalizableString(SortOption.SpDefense),
                                             SortHelper.SortOptionToLocalizableString(SortOption.Speed)
                                         },
                                         OnOptionSelected);
        }

        /// <summary>
        /// Check if a specific tuple of monster, form and gender has been seen.
        /// </summary>
        private static bool
            HasTupleBeenSeen((MonsterDexEntry Monster, FormDexEntry Form, MonsterGender Gender) tuple) =>
            tuple.Monster.HasMonsterBeenSeen
         && tuple.Form.HasFormBeenSeen
         && (!tuple.Monster.Species[tuple.Form.Form].HasGenderVariations || tuple.Form.GendersSeen[tuple.Gender]);

        /// <summary>
        /// Update the button text.
        /// </summary>
        protected override void UpdateText() =>
            Text.SetText(Localizer["Menu/Pokemon/Cloud/SortAndFilter/Sort"]
                       + " "
                       + Localizer[SortHelper.SortOptionToLocalizableString(Option)]
                       + " ("
                       + Localizer[SortHelper.SortModeToLocalizableString(Mode)]
                       + ")");

        /// <summary>
        /// Called when an option has been selected.
        /// </summary>
        /// <param name="index">Index selected.</param>
        private void OnOptionSelected(int index) => StartCoroutine(OnOptionSelectedRoutine(index));

        /// <summary>
        /// Called when an option has been selected.
        /// </summary>
        /// <param name="index">Index selected.</param>
        private IEnumerator OnOptionSelectedRoutine(int index)
        {
            Option = availableOptions[index];

            yield return WaitAFrame;

            DialogManager.ShowChoiceMenu(new List<string>
                                         {
                                             SortHelper.SortModeToLocalizableString(SortMode.Ascending),
                                             SortHelper.SortModeToLocalizableString(SortMode.Descending)
                                         },
                                         OnModeSelected);
        }

        /// <summary>
        /// Called when a mode has been selected.
        /// </summary>
        /// <param name="index">Index selected.</param>
        private void OnModeSelected(int index)
        {
            Mode = (SortMode)index;
            UpdateText();
        }
    }
}