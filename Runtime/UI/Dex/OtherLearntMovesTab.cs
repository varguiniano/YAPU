using System.Collections.Generic;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Dex
{
    /// <summary>
    /// Dex tab that displays the moves the monster learns through other means.
    /// </summary>
    public class OtherLearntMovesTab : MoveListTab
    {
        /// <summary>
        /// Reference to the monster database.
        /// </summary>
        [Inject]
        private MonsterDatabaseInstance database;

        /// <summary>
        /// Retrieve the moves to display.
        /// </summary>
        protected override IEnumerable<Move> GetMoves(DataByFormEntry data) => data.GetOtherLearnMoves(database);
    }
}