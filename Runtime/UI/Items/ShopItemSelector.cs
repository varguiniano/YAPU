using System.Collections.Generic;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;

namespace Varguiniano.YAPU.Runtime.UI.Items
{
    /// <summary>
    /// Controller for the item menu shown at the shop.
    /// </summary>
    public class ShopItemSelector : VirtualizedMenuSelector<Item, ItemButton, ItemButton.Factory>
    {
        /// <summary>
        /// List of prices for each button.
        /// </summary>
        private List<uint> prices;

        /// <summary>
        /// Set the item prices.
        /// </summary>
        /// <param name="newPrices"></param>
        public void SetPrices(List<uint> newPrices) => prices = newPrices;

        /// <summary>
        /// Get the price for the given index.
        /// </summary>
        /// <param name="index">Index to check.</param>
        public uint GetPrice(int index) => prices[index];

        /// <summary>
        /// Set the data for an item.
        /// </summary>
        /// <param name="child">Button.</param>
        /// <param name="childData">Data to display on that button.</param>
        protected override void PopulateChildData(ItemButton child, Item childData) =>
            child.SetItem(childData, (int)prices[Data.IndexOf(childData)]);
    }
}