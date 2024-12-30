using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;

namespace Varguiniano.YAPU.Runtime.UI.Moves
{
    /// <summary>
    /// Controller for a menu that displays the moves that a monster can learn by level.
    /// </summary>
    public class MovesByLevelMenu : VirtualizedMenuSelector<MoveLevelPair, MoveByLevelButton,
        MoveByLevelButton.MoveByLevelFactory>
    {
        /// <summary>
        /// Set the data of a button.
        /// </summary>
        protected override void PopulateChildData(MoveByLevelButton child, MoveLevelPair childData) =>
            child.SetMoveAndLevel(childData.Move, childData.Level);
    }
}