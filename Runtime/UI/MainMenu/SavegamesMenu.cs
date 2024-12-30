namespace Varguiniano.YAPU.Runtime.UI.MainMenu
{
    /// <summary>
    /// Controller for the behaviour in charge of the menu for loading a game.
    /// </summary>
    public class SavegamesMenu : VirtualizedMenuSelector<string, SavegameButton, SavegameButton.Factory>
    {
        /// <summary>
        /// Set the data on a button.
        /// </summary>
        /// <param name="child">Button to set.</param>
        /// <param name="childData">Data to set.</param>
        protected override void PopulateChildData(SavegameButton child, string childData) =>
            child.SetSaveName(childData);
    }
}