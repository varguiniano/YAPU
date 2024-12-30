using UnityEngine;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.UI.Items
{
    /// <summary>
    /// Controller for an item icon.
    /// </summary>
    public class ItemIcon : WhateverBehaviour<ItemIcon>
    {
        /// <summary>
        /// Reference to the sprite to be displayed when there is no icon.
        /// </summary>
        [SerializeField]
        private Sprite EmptyIcon;

        /// <summary>
        /// Set the item of this icon.
        /// </summary>
        /// <param name="item">Item to display.</param>
        public void SetIcon(Item item) => GetCachedComponent<Image>().sprite = item == null ? EmptyIcon : item.Icon;
    }
}