using System.Collections.Generic;
using System.Linq;
using Varguiniano.YAPU.Runtime.Monster;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.MonstersMenu.Filters
{
    /// <summary>
    /// Controller for a filter button that filters by which monsters can learn a move.
    /// </summary>
    public class CanLearnMoveFilterButton : KnowsMoveFilterButton
    {
        /// <summary>
        /// First part of the button.
        /// </summary>
        protected override string IntroString => "Menu/Pokemon/Cloud/SortAndFilter/CanLearnMove";

        /// <summary>
        /// Filter by the species.
        /// </summary>
        /// <param name="original">Original list.</param>
        /// <returns>Filtered list.</returns>
        protected override IEnumerable<MonsterInstance> ApplyEnabledFilter(IEnumerable<MonsterInstance> original) =>
            original.Where(monster => monster.CanLearnMove(Move));
    }
}