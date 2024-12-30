using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.UI.Moves
{
    /// <summary>
    /// Controller for a virtualized menu that displays moves.
    /// </summary>
    public class MovesMenu : VirtualizedMenuSelector<Move, MoveButton, MoveButton.Factory>
    {
        /// <summary>
        /// Set the move into the button.
        /// </summary>
        protected override void PopulateChildData(MoveButton child, Move childData) => child.SetMove(childData);
    }
}