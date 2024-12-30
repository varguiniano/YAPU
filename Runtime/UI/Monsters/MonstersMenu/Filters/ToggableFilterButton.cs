using System.Collections.Generic;
using Varguiniano.YAPU.Runtime.Monster;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.MonstersMenu.Filters
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
        /// <param name="original">Original list.</param>
        /// <returns>Filtered list.</returns>
        public override IEnumerable<MonsterInstance> ApplyFilter(IEnumerable<MonsterInstance> original) =>
            FilterEnabled ? ApplyEnabledFilter(original) : new List<MonsterInstance>(original);

        /// <summary>
        /// Apply the filter that is enabled.
        /// </summary>
        /// <param name="original">Original list.</param>
        /// <returns>Filtered list.</returns>
        protected abstract IEnumerable<MonsterInstance> ApplyEnabledFilter(IEnumerable<MonsterInstance> original);
    }
}