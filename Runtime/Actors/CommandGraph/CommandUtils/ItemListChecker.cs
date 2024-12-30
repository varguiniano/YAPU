using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.CommandUtils
{
    /// <summary>
    /// Scriptable object that only allows items from a list on a choosing dialog.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Dialogs/ItemCompatibility/ItemListChecker", fileName = "ItemListChecker")]
    public class ItemListChecker : ItemCompatibilityChecker
    {
        /// <summary>
        /// List of allowed items.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllItems))]
        #endif
        [SerializeField]
        private List<Item> AllowedItems;

        /// <summary>
        /// Only allow items on the list.
        /// </summary>
        /// <param name="item">Item to check.</param>
        /// <returns>True if it is on the list.</returns>
        public override bool IsItemCompatible(Item item) => AllowedItems.Contains(item);
    }
}