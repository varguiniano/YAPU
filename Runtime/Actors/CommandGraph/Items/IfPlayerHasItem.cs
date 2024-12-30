using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph.Variables;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Items
{
    /// <summary>
    /// Command that checks if the player has an item.
    /// </summary>
    [Serializable]
    public class IfPlayerHasItem : IfCondition
    {
        /// <summary>
        /// Item to check.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllItems))]
        #endif
        [SerializeField]
        private Item Item;

        /// <summary>
        /// Check if they have the item.
        /// </summary>
        protected override bool CheckCondition(CommandParameterData parameterData) =>
            parameterData.PlayerCharacter.PlayerBag.Contains(Item);
    }
}