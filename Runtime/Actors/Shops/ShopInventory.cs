using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.Actors.Shops
{
    /// <summary>
    /// Data asset storing the inventory a shop can have, classified by the number of badges in the current region.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Shops/Inventory", fileName = "ShopInventory")]
    public class ShopInventory : WhateverScriptable<ShopInventory>
    {
        /// <summary>
        /// Inventory of the store, classified by the number of badges the player has.
        /// </summary>
        [InfoBox("Inventory of the store, classified by the number of badges the player has.")]
        [SerializeField]
        private SerializableDictionary<byte, List<Item>> Inventory = new();

        /// <summary>
        /// Promotions the store has.
        /// 1 B item will be gifted for each X A items sold.
        /// Ex: The player gets 1 Premier Ball for each 10 Poké Balls sold.
        /// </summary>
        [InfoBox("Promotions the store has.\n"
               + "1 B item will be gifted for each X A items sold.\n"
               + "Ex: The player gets 1 Premier Ball for each 10 Poké Balls sold.")]
        public SerializableDictionary<Item, ObjectPair<uint, Item>> Promotions = new();

        /// <summary>
        /// Build the inventory of the shop with the given number of badges.
        /// </summary>
        /// <param name="numberOfBadges">Number of badges the player has.</param>
        /// <returns>A list of the items to sell.</returns>
        public List<Item> BuildInventoryForBadges(byte numberOfBadges)
        {
            List<Item> inventory = new();

            foreach (KeyValuePair<byte, List<Item>> entry in Inventory)
                if (entry.Key <= numberOfBadges)
                    inventory.AddRange(entry.Value);

            return inventory.OrderBy(item => item.ItemCategory.name).ThenBy(item => item.DefaultPrice).ToList();
        }
    }
}