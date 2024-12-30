using Varguiniano.YAPU.Runtime.Badges;

namespace Varguiniano.YAPU.Runtime.UI.Profile
{
    /// <summary>
    /// Controller for the list of badges in the profile screen.
    /// </summary>
    public class BadgeList : VirtualizedMenuSelector<Badge, BadgeButton, BadgeButton.Factory>
    {
        /// <summary>
        /// Set the data on each button.
        /// </summary>
        /// <param name="child">Button.</param>
        /// <param name="childData">Data to set.</param>
        protected override void PopulateChildData(BadgeButton child, Badge childData) => child.SetBadge(childData);
    }
}