using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Moves;

namespace Varguiniano.YAPU.Runtime.UI.Dialogs.MoveTutor
{
    /// <summary>
    /// Controller for the menu that allows to select a move.
    /// </summary>
    public class MoveSelectionMenu : VirtualizedMenuSelector<Move, MoveButton, MoveButton.Factory>
    {
        /// <summary>
        /// Set the button for a move.
        /// </summary>
        /// <param name="child">Button.</param>
        /// <param name="childData">Move.</param>
        protected override void PopulateChildData(MoveButton child, Move childData) => child.SetMove(childData);
    }
}