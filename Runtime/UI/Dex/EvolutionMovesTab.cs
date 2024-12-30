using System.Collections.Generic;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.UI.Dex
{
    /// <summary>
    /// Dex tab that displays the moves the monster learns upon evolving.
    /// </summary>
    public class EvolutionMovesTab : MoveListTab
    {
        /// <summary>
        /// Retrieve the moves to display.
        /// </summary>
        protected override IEnumerable<Move> GetMoves(DataByFormEntry data) => data.OnEvolutionMoves;
    }
}