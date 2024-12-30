using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.UI.Sorting;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.MonstersMenu.Filters
{
    /// <summary>
    /// Controller for the button that applies sorting.
    /// </summary>
    public class SortingFilterButton : FilterButton
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
            SortOption.DateAdded,
            SortOption.DexNumber,
            SortOption.Alphabetically,
            SortOption.Level,
            SortOption.TotalIVs,
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
        /// </summary>
        /// <param name="original">Original list.</param>
        /// <returns>Sorted list.</returns>
        // ReSharper disable once CyclomaticComplexity
        public override IEnumerable<MonsterInstance> ApplyFilter(IEnumerable<MonsterInstance> original)
        {
            switch (Option)
            {
                case SortOption.DateAdded when Mode == SortMode.Ascending: return original;
                case SortOption.DateAdded:
                    List<MonsterInstance> newList = new(original);
                    newList.Reverse();
                    return newList;
                case SortOption.DexNumber when Mode == SortMode.Ascending:
                    return original.OrderBy(monster => monster.Species.DexNumber);
                case SortOption.DexNumber: return original.OrderByDescending(monster => monster.Species.DexNumber);
                case SortOption.Alphabetically when Mode == SortMode.Ascending:
                    return original.OrderBy(monster => monster.GetNameOrNickName(Localizer));
                case SortOption.Alphabetically:
                    return original.OrderByDescending(monster => monster.GetNameOrNickName(Localizer));
                case SortOption.Level when Mode == SortMode.Ascending:
                    return original.OrderBy(monster => monster.StatData.Level);
                case SortOption.Level: return original.OrderByDescending(monster => monster.StatData.Level);
                case SortOption.TotalIVs when Mode == SortMode.Ascending:
                    return original.OrderBy(monster => monster.StatData.GetTotalNumberOfIvs());
                case SortOption.TotalIVs:
                    return original.OrderByDescending(monster => monster.StatData.GetTotalNumberOfIvs());
                case SortOption.HP when Mode == SortMode.Ascending:
                    return original.OrderBy(monster => monster.GetStats(null)[Stat.Hp]);
                case SortOption.HP: return original.OrderByDescending(monster => monster.GetStats(null)[Stat.Hp]);
                case SortOption.Attack when Mode == SortMode.Ascending:
                    return original.OrderBy(monster => monster.GetStats(null)[Stat.Attack]);
                case SortOption.Attack:
                    return original.OrderByDescending(monster => monster.GetStats(null)[Stat.Attack]);
                case SortOption.Defense when Mode == SortMode.Ascending:
                    return original.OrderBy(monster => monster.GetStats(null)[Stat.Defense]);
                case SortOption.Defense:
                    return original.OrderByDescending(monster => monster.GetStats(null)[Stat.Defense]);
                case SortOption.SpAttack when Mode == SortMode.Ascending:
                    return original.OrderBy(monster => monster.GetStats(null)[Stat.SpecialAttack]);
                case SortOption.SpAttack:
                    return original.OrderByDescending(monster => monster.GetStats(null)[Stat.SpecialAttack]);
                case SortOption.SpDefense when Mode == SortMode.Ascending:
                    return original.OrderBy(monster => monster.GetStats(null)[Stat.SpecialDefense]);
                case SortOption.SpDefense:
                    return original.OrderByDescending(monster => monster.GetStats(null)[Stat.SpecialDefense]);
                case SortOption.Speed when Mode == SortMode.Ascending:
                    return original.OrderBy(monster => monster.GetStats(null)[Stat.Hp]);
                case SortOption.Speed: return original.OrderByDescending(monster => monster.GetStats(null)[Stat.Speed]);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Select the new sorting via choice menu.
        /// </summary>
        public override void OnButtonSelected() =>
            DialogManager.ShowChoiceMenu(new List<string>
                                         {
                                             SortHelper.SortOptionToLocalizableString(SortOption.DateAdded),
                                             SortHelper.SortOptionToLocalizableString(SortOption.DexNumber),
                                             SortHelper.SortOptionToLocalizableString(SortOption.Alphabetically),
                                             SortHelper.SortOptionToLocalizableString(SortOption.Level),
                                             SortHelper.SortOptionToLocalizableString(SortOption.TotalIVs),
                                             SortHelper.SortOptionToLocalizableString(SortOption.HP),
                                             SortHelper.SortOptionToLocalizableString(SortOption.Attack),
                                             SortHelper.SortOptionToLocalizableString(SortOption.Defense),
                                             SortHelper.SortOptionToLocalizableString(SortOption.SpAttack),
                                             SortHelper.SortOptionToLocalizableString(SortOption.SpDefense),
                                             SortHelper.SortOptionToLocalizableString(SortOption.Speed)
                                         },
                                         OnOptionSelected);

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