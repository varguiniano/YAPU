using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.UI.Bags
{
    /// <summary>
    /// Controller for the icon of a tab in the bag menu.
    /// </summary>
    public class BagTabIcon : WhateverBehaviour<BagTabIcon>
    {
        /// <summary>
        /// Color when not selected.
        /// </summary>
        [SerializeField]
        private Color NormalColor;

        /// <summary>
        /// Color when selected.
        /// </summary>
        [SerializeField]
        private Color SelectedColor;

        /// <summary>
        /// Show or hide the icon.
        /// </summary>
        /// <param name="show">Show or hide.</param>
        public void Show(bool show) => GetCachedComponent<Image>().enabled = show;

        /// <summary>
        /// Reset on enable.
        /// </summary>
        private void OnEnable() => OnDeselect();

        /// <summary>
        /// Set the icon for this tab.
        /// </summary>
        /// <param name="category">Category to display.</param>
        [Button]
        public void SetTab(ItemCategory category) => GetCachedComponent<Image>().sprite = category.Icon;

        /// <summary>
        /// Called when selected.
        /// </summary>
        public void OnSelect() => GetCachedComponent<Image>().color = SelectedColor;

        /// <summary>
        /// Called when deselected.
        /// </summary>
        public void OnDeselect() => GetCachedComponent<Image>().color = NormalColor;
    }
}