using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Moves;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.MonstersMenu.Filters
{
    /// <summary>
    /// Controller for the menu that allows to filter by move.
    /// </summary>
    public class MoveFilterMenu : VirtualizedMenuSelector<Move, MoveButton, MoveButton.Factory>
    {
        /// <summary>
        /// Reference to the database.
        /// </summary>
        [Inject]
        private MonsterDatabaseInstance database;
        
        /// <summary>
        /// Set the data.
        /// </summary>
        public override void Show(bool show = true)
        {
            if (show) SetButtons(database.GetMoves());
            base.Show(show);
        }

        /// <summary>
        /// Set the button for a move.
        /// </summary>
        /// <param name="child">Button.</param>
        /// <param name="childData">Move.</param>
        protected override void PopulateChildData(MoveButton child, Move childData) => child.SetMove(childData);
    }
}