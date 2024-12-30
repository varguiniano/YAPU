using System.Collections.Generic;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDex;

namespace Varguiniano.YAPU.Runtime.UI.Dex.Filters
{
    /// <summary>
    /// Base class for filter buttons which filter can be toggled.
    /// </summary>
    public abstract class ToggableFilterButton : FilterButton
    {
        /// <summary>
        /// Flag to know if the filter is enabled.
        /// </summary>
        protected bool FilterEnabled;

        /// <summary>
        /// Disable the filter.
        /// </summary>
        public void DisableFilter()
        {
            FilterEnabled = false;
            UpdateText();
        }

        /// <summary>
        /// Filter only if filtering is enabled.
        /// </summary>
        /// <param name="originalEntries">Original dex entries.</param>
        /// <returns>Filtered list.</returns>
        public override IEnumerable<(MonsterDexEntry, FormDexEntry, MonsterGender)>
            ApplyFilter(IEnumerable<(MonsterDexEntry, FormDexEntry, MonsterGender)> originalEntries) =>
            FilterEnabled
                ? ApplyEnabledFilter(originalEntries)
                : new List<(MonsterDexEntry, FormDexEntry, MonsterGender)>(originalEntries);

        /// <summary>
        /// Apply the filter that is enabled.
        /// </summary>
        /// <param name="original">Original list.</param>
        /// <returns>Filtered list.</returns>
        protected abstract IEnumerable<(MonsterDexEntry, FormDexEntry, MonsterGender)> ApplyEnabledFilter(
            IEnumerable<(MonsterDexEntry, FormDexEntry, MonsterGender)> original);
    }
}