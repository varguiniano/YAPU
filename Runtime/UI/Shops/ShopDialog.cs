using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Bags;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.UI.Bags;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.UI.Items;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Shops
{
    /// <summary>
    /// Controller for the shop dialog.
    /// </summary>
    public class ShopDialog : HidableUiElement<ShopDialog>, IPlayerDataReceiver
    {
        /// <summary>
        /// Reference to the shop menu.
        /// </summary>
        [SerializeField]
        private ShopItemSelector ShopMenu;

        /// <summary>
        /// Reference to the amount selector.
        /// </summary>
        [SerializeField]
        private ShopAmountSelector ShopAmountSelector;

        /// <summary>
        /// Reference to the text that displays the money.
        /// </summary>
        [SerializeField]
        private MoneyText MoneyText;

        /// <summary>
        /// Name of the item in the description.
        /// </summary>
        [SerializeField]
        private TMP_Text DescriptionName;

        /// <summary>
        /// Description text of the item.
        /// </summary>
        [SerializeField]
        private TMP_Text DescriptionText;

        /// <summary>
        /// Reference to the text that displays the amount of that item in the bag.
        /// </summary>
        [SerializeField]
        private TMP_Text BagAmount;

        /// <summary>
        /// Reference to the shop container.
        /// </summary>
        [SerializeField]
        private Transform ShopContainer;

        /// <summary>
        /// Reference to the shop container open position.
        /// </summary>
        [SerializeField]
        private Transform ShopContainerOpenPosition;

        /// <summary>
        /// Reference to the shop container closed position.
        /// </summary>
        [SerializeField]
        private Transform ShopContainerClosedPosition;

        /// <summary>
        /// Promotions of the store.
        /// </summary>
        private SerializableDictionary<Item, ObjectPair<uint, Item>> promotions;

        /// <summary>
        /// Reference to the player bag.
        /// </summary>
        [Inject]
        private Bag playerBag;

        /// <summary>
        /// Reference to the player settings.
        /// </summary>
        [Inject]
        private PlayerSettings playerSettings;

        /// <summary>
        /// Reference to the input manager.
        /// </summary>
        [Inject]
        private IInputManager inputManager;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        [Inject]
        private YAPUSettings settings;

        /// <summary>
        /// Show the shop menu.
        /// </summary>
        /// <param name="items">Items to display.</param>
        /// <param name="prices">Prices to display.</param>
        /// <param name="shopPromotions">Promotions the shop has.</param>
        public IEnumerator ShowShop(List<Item> items,
                                    List<uint> prices,
                                    SerializableDictionary<Item, ObjectPair<uint, Item>> shopPromotions)
        {
            inputManager.BlockInput();

            Show(false);

            ShopMenu.Show(false);
            ShopAmountSelector.SetClosedPosition();

            ShopContainer.localPosition = ShopContainerClosedPosition.localPosition;

            ShopMenu.SetPrices(prices);
            ShopMenu.SetButtons(items);

            promotions = shopPromotions;

            UpdateBagData();

            ShopMenu.OnBackSelected += () => StartCoroutine(HideShop());
            ShopMenu.OnHovered += OnItemHovered;
            ShopMenu.OnButtonSelected += index => StartCoroutine(OnItemSelected(index));

            Show();

            yield return ShopContainer.DOLocalMove(ShopContainerOpenPosition.localPosition, .25f)
                                      .SetEase(Ease.OutBack)
                                      .WaitForCompletion();

            inputManager.BlockInput(false);

            ShopMenu.Show();

            yield return new WaitWhile(() => Shown);
        }

        /// <summary>
        /// Hide the shop menu.
        /// </summary>
        private IEnumerator HideShop()
        {
            inputManager.BlockInput();

            ShopMenu.Show(false);

            ShopMenu.OnBackSelected = null;
            ShopMenu.OnHovered = null;
            ShopMenu.OnButtonSelected = null;

            yield return ShopContainer.DOLocalMove(ShopContainerClosedPosition.localPosition, .25f)
                                      .SetEase(Ease.InBack)
                                      .WaitForCompletion();

            inputManager.BlockInput(false);

            Show(false);
        }

        /// <summary>
        /// Called when an item is selected.
        /// </summary>
        /// <param name="index">Index of the selected item.</param>
        private IEnumerator OnItemSelected(int index)
        {
            Item item = ShopMenu.Data[index];
            uint price = ShopMenu.GetPrice(index);

            if (!item.CanStack && playerBag.Contains(item))
            {
                yield return DialogManager.ShowDialogAndWait("Dialogs/PokeCenter/Mart/AlreadyHave");
                yield break;
            }

            uint amountToBuy = 0;
            uint maxBuyableAmount = item.CanStack ? playerBag.Money / price : 1;

            switch (maxBuyableAmount)
            {
                case 0:
                    yield return DialogManager.ShowDialogAndWait("Dialogs/PokeCenter/Mart/CantAfford");
                    break;
                case > 1:
                    yield return ShopAmountSelector.RequestAmount(price,
                                                                  maxBuyableAmount,
                                                                  (amount, accepted) =>
                                                                  {
                                                                      if (accepted) amountToBuy = amount;
                                                                  });

                    break;
                default:
                    amountToBuy = 1;
                    break;
            }

            if (amountToBuy == 0) yield break;

            uint finalPrice = price * amountToBuy;

            int choice = -1;

            DialogManager.ShowChoiceMenu(new List<string>
                                         {
                                             "Common/True",
                                             "Common/False"
                                         },
                                         playerChoice => choice = playerChoice,
                                         onBackCallback: () => choice = 1,
                                         showDialog: true,
                                         localizationKey: "Dialogs/PokeCenter/Mart/Confirmation",
                                         localizableModifiers: false,
                                         modifiers: new[]
                                                    {
                                                        amountToBuy.ToString(),
                                                        item.GetName(localizer),
                                                        MoneyHelper.BuildMoneyString(finalPrice, settings, localizer)
                                                    });

            yield return new WaitWhile(() => choice == -1);

            if (choice == 1) yield break;

            playerBag.Money -= finalPrice;
            playerBag.ChangeItemAmount(item, (int)amountToBuy);

            OnItemHovered(index);

            yield return DialogManager.ShowDialogAndWait("Dialogs/PokeCenter/Mart/Thanks");

            if (!promotions.ContainsKey(item) || promotions[item].Key > amountToBuy) yield break;

            int promotionItemsWon = (int)(amountToBuy / promotions[item].Key);

            playerBag.ChangeItemAmount(promotions[item].Value, promotionItemsWon);

            OnItemHovered(index);

            yield return DialogManager.ShowDialogAndWait("Dialogs/PokeCenter/Mart/Promotion",
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        promotionItemsWon.ToString(),
                                                                        promotions[item].Value.GetName(localizer)
                                                                    });
        }

        /// <summary>
        /// Called when an item is hovered.
        /// </summary>
        /// <param name="index">Index of the hovered item.</param>
        private void OnItemHovered(int index)
        {
            Item item = ShopMenu.Data[index];

            DescriptionName.SetText(item.GetName(localizer));
            DescriptionText.SetText(item.GetDescription(localizer, playerSettings));

            BagAmount.SetText(playerBag.Contains(item) ? playerBag.GetItemAmount(item).ToString() : "0");

            UpdateBagData();
        }

        /// <summary>
        /// Updates the screen with the data from the player bag.
        /// </summary>
        private void UpdateBagData() => MoneyText.SetMoney(playerBag);
    }
}