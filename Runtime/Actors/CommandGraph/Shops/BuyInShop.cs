using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.Shops;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Shops
{
    /// <summary>
    /// Command to run all shop logic related to buying.
    /// </summary>
    [Serializable]
    public class BuyInShop : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Shop inventory to use.
        /// </summary>
        [SerializeField]
        private ShopInventory ShopInventory;

        /// <summary>
        /// Run the command.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            List<Item> itemsToSell =
                ShopInventory.BuildInventoryForBadges((byte) parameterData
                                                            .GlobalGameData.GetBadges(parameterData.PlayerCharacter
                                                                         .Region)
                                                            .Count);

            yield return DialogManager.ShowShop(itemsToSell,
                                                itemsToSell.Select(item => item.DefaultPrice).ToList(),
                                                ShopInventory.Promotions);
        }
    }
}