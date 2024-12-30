using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Bags;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.UI.Items;
using WhateverDevs.Localization.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Bags
{
    public class QuickAccessItemsMenu : VirtualizedMenuSelector<Item, ItemButton, QuickAccessItemButtonFactory>,
                                        IPlayerDataReceiver
    {
        /// <summary>
        /// Reference to the menu's open position.
        /// </summary>
        [SerializeField]
        private Transform OpenPosition;

        /// <summary>
        /// Reference to the menu's closed position.
        /// </summary>
        [SerializeField]
        private Transform ClosedPosition;

        /// <summary>
        /// Cached access to the attached transform.
        /// </summary>
        private Transform Transform
        {
            get
            {
                if (transformReference == null) transformReference = transform;
                return transformReference;
            }
        }

        /// <summary>
        /// Reference to the attached transform.
        /// </summary>
        private Transform transformReference;

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
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Reference to the player character.
        /// </summary>
        private PlayerCharacter playerCharacter;

        /// <summary>
        /// Method to show the quick access menu.
        /// </summary>
        public void ShowMenu(PlayerCharacter playerCharacterReference)
        {
            playerCharacter = playerCharacterReference;
            StartCoroutine(ShowMenuRoutine());
        }

        /// <summary>
        /// Method to show the quick access menu.
        /// </summary>
        private IEnumerator ShowMenuRoutine()
        {
            InputManager.BlockInput();

            Show(false);

            List<Item> registeredItems = playerBag.GetRegisteredItems();

            switch (registeredItems.Count)
            {
                case 0:
                    yield return DialogManager.ShowDialogAndWait("Item/Registered/None");
                    InputManager.BlockInput(false);
                    yield break;
                case 1:
                {
                    InputManager.BlockInput(false);
                    yield return UseItem(registeredItems[0]);
                    yield break;
                }
            }

            SetButtons(registeredItems);
            OnButtonSelected += OnItemSelected;
            OnBackSelected += CloseMenu;

            Transform.position = ClosedPosition.position;

            Show();

            yield return Transform.DOMove(OpenPosition.position, 0.25f).SetEase(Ease.OutBack).WaitForCompletion();

            InputManager.BlockInput(false);
            InputManager.RequestInput(this);
        }

        /// <summary>
        /// Close the menu.
        /// </summary>
        private void CloseMenu() => StartCoroutine(CloseMenuRoutine());

        /// <summary>
        /// Close the menu.
        /// </summary>
        private IEnumerator CloseMenuRoutine()
        {
            OnButtonSelected -= OnItemSelected;
            OnBackSelected -= CloseMenu;

            InputManager.ReleaseInput(this);
            InputManager.BlockInput();

            yield return Transform.DOMove(ClosedPosition.position, 0.25f).SetEase(Ease.InBack).WaitForCompletion();
            Show(false);

            InputManager.BlockInput(false);
        }

        /// <summary>
        /// Use the selected item when it has been selected.
        /// </summary>
        /// <param name="index">Index of the selected item.</param>
        private void OnItemSelected(int index) => StartCoroutine(UseItem(playerBag.GetRegisteredItems()[index]));

        /// <summary>
        /// Use an item.
        /// </summary>
        /// <param name="item">Item to use.</param>
        private IEnumerator UseItem(Item item)
        {
            if (Shown) yield return CloseMenuRoutine();

            bool consumeItem = false;

            yield return WaitAFrame;

            yield return item.UseOutOfBattle(playerSettings,
                                             playerCharacter,
                                             localizer,
                                             shouldConsume => consumeItem = shouldConsume);

            yield return WaitAFrame;

            if (consumeItem) playerBag.ChangeItemAmount(item, -1);

            DialogManager.AcceptInput = true;

            yield return DialogManager.WaitForDialog;
        }

        /// <summary>
        /// Populate the data for each button.
        /// </summary>
        /// <param name="child">Button to fill.</param>
        /// <param name="childData">Data to set on that button.</param>
        protected override void PopulateChildData(ItemButton child, Item childData) =>
            child.SetItem(childData, playerBag.GetItemAmount(childData));
    }
}