namespace Varguiniano.YAPU.Runtime.UI
{
    /// <summary>
    /// Base class for menu items that are part of a virtualized menu.
    /// </summary>
    public class VirtualizedMenuItem : MenuItem
    {
        /// <summary>
        /// Current row in the menu.
        /// </summary>
        public int CurrentRow { get; private set; }

        /// <summary>
        /// Notify this button its current assignment in the list.
        /// </summary>
        /// <param name="row">Row it's assigned to.</param>
        public void NotifyCurrentAssignment(int row) => CurrentRow = row;
    }
}