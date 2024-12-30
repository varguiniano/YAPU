using System.Collections.Generic;
using System.Linq;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.UI.Items;

namespace Varguiniano.YAPU.Runtime.UI.Bags
{
    /// <summary>
    /// Controller for a specific tab in the bag.
    /// </summary>
    public class BagTab : VirtualizedMenuSelector<KeyValuePair<Item, int>, ItemButton, ItemButton.Factory>
    {
        /// <summary>
        /// Set the data for the buttons from a dictionary.
        /// </summary>
        /// <param name="dictionary">Dictionary with the items and the amounts.</param>
        /// <param name="clearPrevious">Clear the previous buttons?</param>
        public void SetButtons(Dictionary<Item, int> dictionary, bool clearPrevious = true) =>
            SetButtons(dictionary.ToList(), clearPrevious);

        /// <summary>
        /// Populate the data of the button.
        /// </summary>
        /// <param name="child">Button to populate.</param>
        /// <param name="childData"></param>
        protected override void PopulateChildData(ItemButton child, KeyValuePair<Item, int> childData) =>
            child.SetItem(childData.Key, childData.Value);
    }
}