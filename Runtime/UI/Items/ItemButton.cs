using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.UI.Bags;
using WhateverDevs.Core.Runtime.DependencyInjection;
using WhateverDevs.Localization.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Items
{
    /// <summary>
    /// Controller of a button used to display an item.
    /// </summary>
    public class ItemButton : VirtualizedMenuItem
    {
        /// <summary>
        /// Mode this button is on.
        /// </summary>
        [SerializeField]
        private ItemButtonMode Mode;

        /// <summary>
        /// Reference to the item icon.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private ItemIcon Icon;

        /// <summary>
        /// Reference to the item name text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text ItemName;

        /// <summary>
        /// Reference to the x text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text XText;

        /// <summary>
        /// Reference to the item amount text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text ItemAmount;

        /// <summary>
        /// Reference to the money text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private MoneyText MoneyText;

        /// <summary>
        /// Reference to the background image.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Image Background;

        /// <summary>
        /// Reference to the left section image.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Image LeftSection;

        /// <summary>
        /// Color when it is not selected.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private Color BackgroundNonSelectedColor;

        /// <summary>
        /// Color when it is not selected.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private Color TextNonSelectedColor;

        /// <summary>
        /// Color when it is selected.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private Color BackgroundSelectedColor;

        /// <summary>
        /// Color when it is selected.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private Color LeftSectionSelectedColor;

        /// <summary>
        /// Color when it is selected.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private Color TextSelectedColor;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Item being displayed.
        /// </summary>
        public Item Item { get; private set; }

        /// <summary>
        /// Set the item and amount to display.
        /// </summary>
        /// <param name="itemToSet">The item to display.</param>
        /// <param name="amount">The amount of that item.</param>
        [Button]
        public void SetItem(Item itemToSet, int amount)
        {
            Item = itemToSet;

            Icon.SetIcon(Item);

            // Make sure it works on editor.
            ItemName.SetText(localizer == null ? Item.LocalizableName : Item.GetName(localizer));

            switch (Mode)
            {
                case ItemButtonMode.Storage:
                    XText.SetText(Item.CanStack ? "x" : string.Empty);
                    ItemAmount.SetText(Item.CanStack ? amount.ToString() : string.Empty);
                    break;
                case ItemButtonMode.Shop:
                    XText.SetText(string.Empty);
                    MoneyText.SetMoney((uint)amount);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Change the colors.
        /// </summary>
        [Button]
        public override void OnSelect()
        {
            base.OnSelect();

            Background.color = Item.CanStack ? BackgroundSelectedColor : LeftSectionSelectedColor;
            LeftSection.color = LeftSectionSelectedColor;
            ItemName.color = TextSelectedColor;
            XText.color = TextSelectedColor;
            ItemAmount.color = TextSelectedColor;
        }

        /// <summary>
        /// Change the colors.
        /// </summary>
        [Button]
        public override void OnDeselect()
        {
            base.OnDeselect();

            Background.color = BackgroundNonSelectedColor;
            LeftSection.color = BackgroundNonSelectedColor;
            ItemName.color = TextNonSelectedColor;
            XText.color = TextNonSelectedColor;
            ItemAmount.color = TextNonSelectedColor;
        }

        /// <summary>
        /// Factory to be used by dependency injection.
        /// </summary>
        public class Factory : GameObjectFactory<ItemButton>
        {
        }

        /// <summary>
        /// Modes this button can work on.
        /// </summary>
        private enum ItemButtonMode
        {
            Storage,
            Shop
        }
    }
}