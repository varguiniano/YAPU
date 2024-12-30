using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using ModestTree;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph.CommandUtils;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.Monster.Evolution;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Experience;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Bags;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Berries;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.UI.Items;
using Varguiniano.YAPU.Runtime.UI.Monsters;
using Varguiniano.YAPU.Runtime.UI.Shops;
using Varguiniano.YAPU.Runtime.UI.Sorting;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.Localization.Runtime.Ui;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Bags
{
    /// <summary>
    /// Controller of the bag screen.
    /// </summary>
    public class BagScreen : HidableUiElement<BagScreen>, IInputReceiver, IPlayerDataReceiver
    {
        /// <summary>
        /// Categories that will be displayed in this bag screen.
        /// </summary>
        [FoldoutGroup("Categories")]
        [SerializeField]
        private ItemCategory[] CategoriesToDisplay;

        /// <summary>
        /// List of booleans stating the default categories to display.
        /// </summary>
        [FoldoutGroup("Categories")]
        [SerializeField]
        private bool[] DefaultToDisplay;

        /// <summary>
        /// Reference to the money text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private MoneyText MoneyText;

        /// <summary>
        /// Reference to the field to display the title of the tab.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private LocalizedTextMeshPro TabTitle;

        /// <summary>
        /// Tab icons.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private BagTabIcon[] Icons;

        /// <summary>
        /// Reference to the bag tabs.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private BagTab[] Tabs;

        /// <summary>
        /// Reference to the background image.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Image Background;

        /// <summary>
        /// Reference to the right panel.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform RightPanel;

        /// <summary>
        /// Reference to the right panel hidden position.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform RightPanelHiddenPosition;

        /// <summary>
        /// Reference to the right panel shown position.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform RightPanelShownPosition;

        /// <summary>
        /// Reference to the monsters menu.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private MenuSelector MonsterMenu;

        /// <summary>
        /// Reference to the item description.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private ItemDescription ItemDescription;

        /// <summary>
        /// Reference to the shop amount selector.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private ShopAmountSelector ShopAmountSelector;

        /// <summary>
        /// Reference to the purchase price panel.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private HidableUiElement PurchasePricePanel;

        /// <summary>
        /// Reference to the purchase price value.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private MoneyText PurchasePriceValue;

        /// <summary>
        /// Duration of the open close animation.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private float AnimationDuration = .25f;

        /// <summary>
        /// Audio to play on navigation.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference OnNavigationAudio;

        /// <summary>
        /// Audio to play on back.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference OnBackAudio;

        /// <summary>
        /// Are we in a battle?
        /// </summary>
        private bool inBattle;

        /// <summary>
        /// Index of the currently displayed tab.
        /// </summary>
        private int currentTabIndex;

        /// <summary>
        /// Reference to the current tab.
        /// </summary>
        private BagTab CurrentTab => Tabs[currentTabIndex];

        /// <summary>
        /// Reference to the input manager.
        /// </summary>
        [Inject]
        private IInputManager inputManager;

        /// <summary>
        /// Reference to the audio manager.
        /// </summary>
        [Inject]
        private IAudioManager audioManager;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Reference to the player bag.
        /// </summary>
        [Inject]
        private Bag playerBag;

        /// <summary>
        /// Reference to the player roster.
        /// </summary>
        [Inject]
        private Roster playerRoster;

        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        [Inject]
        private YAPUSettings settings;

        /// <summary>
        /// Reference to the player settings.
        /// </summary>
        [Inject]
        private PlayerSettings playerSettings;

        /// <summary>
        /// Reference to the experience look up table.
        /// </summary>
        [Inject]
        private ExperienceLookupTable experienceLookupTable;

        /// <summary>
        /// Reference to the time manager.
        /// </summary>
        [Inject]
        private TimeManager timeManager;

        /// <summary>
        /// Reference to the evolution manager.
        /// </summary>
        [Inject]
        private EvolutionManager evolutionManager;

        /// <summary>
        /// Categories that should be displayed.
        /// </summary>
        private bool[] displayedCategories;

        /// <summary>
        /// Roster to display.
        /// </summary>
        private MonsterInstance[] displayedRoster;

        /// <summary>
        /// Reference to the player character.
        /// </summary>
        private PlayerCharacter playerCharacter;

        /// <summary>
        /// Reference to the battle manager.
        /// </summary>
        private BattleManager battleManager;

        /// <summary>
        /// Index of the battler requesting the item.
        /// </summary>
        private int currentBattler;

        /// <summary>
        /// Flag used to mark when the bag is being used to select an item.
        /// </summary>
        private bool selectionMode;

        /// <summary>
        /// Compatibility checker to use.
        /// </summary>
        private ItemCompatibilityChecker compatibilityChecker;

        /// <summary>
        /// Flag used to mark when the bag is being used to select and sell an item.
        /// </summary>
        private bool sellingMode;

        /// <summary>
        /// Selected sort option.
        /// </summary>
        private SortOption sortOption = SortOption.DateAdded;

        /// <summary>
        /// Selected sort mode.
        /// </summary>
        private SortMode sortMode = SortMode.Ascending;

        /// <summary>
        /// Event raised when the bag gets closed.
        /// It states true if the player selected an item and the item that was selected.
        /// </summary>
        private Action<bool, Item> onBackCallback;

        /// <summary>
        /// Event raised when an action has been chosen inside battle.
        /// </summary>
        private Action<BattleAction> onBattleItemSelected;

        /// <summary>
        /// Flag to mark if we are switching tabs.
        /// </summary>
        private bool switchingTab;

        /// <summary>
        /// Show the bag screen.
        /// </summary>
        /// <param name="onBack">Event raised when the bag gets closed.</param>
        /// <param name="playerCharacterReference">Reference to the player character.</param>
        /// <param name="battleManagerReference">Reference to the battle manager.</param>
        /// <param name="currentBattlerIndex">Index of the battler requesting the item.</param>
        /// <param name="overrideDisplayed">Override the displayed tabs?</param>
        /// <param name="displayOverride">Tabs to display.</param>
        /// <param name="onBattleItemSelectedCallback">Event raised when an action has been chosen inside battle.</param>
        /// <param name="rosterOverride">Override the roster being shown.</param>
        /// <param name="selection">Flag used to mark when the bag is being used to select and item.</param>
        /// <param name="itemCompatibilityChecker">Checker that can filter so that only certain items can be chosen.</param>
        /// <param name="selling">Flag used to mark when the bag is being used to select and sell an item.</param>
        [FoldoutGroup("Debug")]
        [Button]
        public void ShowBag(Action<bool, Item> onBack,
                            PlayerCharacter playerCharacterReference,
                            BattleManager battleManagerReference,
                            int currentBattlerIndex,
                            bool overrideDisplayed,
                            bool[] displayOverride,
                            Action<BattleAction> onBattleItemSelectedCallback,
                            MonsterInstance[] rosterOverride,
                            bool selection,
                            ItemCompatibilityChecker itemCompatibilityChecker,
                            bool selling)
        {
            selectionMode = selection;
            sellingMode = selling;

            compatibilityChecker = selectionMode ? itemCompatibilityChecker : null;

            displayedCategories = overrideDisplayed ? displayOverride :
                                  compatibilityChecker != null ? compatibilityChecker.TabFilter : DefaultToDisplay;

            onBackCallback = onBack;

            playerCharacter = playerCharacterReference;

            inBattle = battleManagerReference != null;
            battleManager = battleManagerReference;
            currentBattler = currentBattlerIndex;
            onBattleItemSelected = onBattleItemSelectedCallback;

            displayedRoster = rosterOverride ?? playerRoster.RosterData;

            StartCoroutine(ShowRoutine());
        }

        /// <summary>
        /// Routine to display the bag screen.
        /// </summary>
        private IEnumerator ShowRoutine()
        {
            inputManager.BlockInput();

            MonsterMenu.Show(false);
            PurchasePricePanel.Show(false);

            DisplayMonsters();
            MoneyText.SetMoney(playerBag);

            RightPanel.localPosition = RightPanelHiddenPosition.localPosition;

            Show();

            Background.DOFade(1, AnimationDuration);

            int initialTab = 0;

            DialogManager.ShowLoadingIcon();

            for (int i = 0; i < Tabs.Length; i++)
            {
                if (!displayedCategories[initialTab]) initialTab = i;

                // Keep the last used tab if it is still valid for this view.
                if (currentTabIndex == i && displayedCategories[i]) initialTab = i;

                Icons[i].Show(displayedCategories[i]);

                yield return UpdateTab(i, true);

                Tabs[i].Show(false);
            }

            DialogManager.ShowLoadingIcon(false);

            yield return RightPanel.DOLocalMove(RightPanelShownPosition.localPosition, AnimationDuration)
                                   .SetEase(Ease.OutBack)
                                   .WaitForCompletion();

            MonsterMenu.Show(!selectionMode);
            PurchasePricePanel.Show(selectionMode && sellingMode);

            inputManager.BlockInput(false);
            inputManager.RequestInput(this);

            StartCoroutine(SwitchTab(initialTab, true));
        }

        /// <summary>
        /// Hide the bag screen.
        /// </summary>
        [FoldoutGroup("Debug")]
        [Button]
        public void Hide() => StartCoroutine(HideRoutine());

        /// <summary>
        /// Hide the bag screen.
        /// </summary>
        public IEnumerator HideRoutine()
        {
            if (!Shown) yield break;

            inputManager.ReleaseInput(this);
            inputManager.BlockInput();

            Icons[currentTabIndex].OnDeselect();

            CurrentTab.Show(false);

            MonsterMenu.Show(false);
            PurchasePricePanel.Show(false);

            RightPanel.localPosition = RightPanelShownPosition.localPosition;

            Background.DOFade(0, AnimationDuration);

            yield return RightPanel.DOLocalMove(RightPanelHiddenPosition.localPosition, AnimationDuration)
                                   .SetEase(Ease.InBack)
                                   .WaitForCompletion();

            Show(false);
            inputManager.BlockInput(false);
            foreach (BagTab tab in Tabs) tab.ClearButtons(true);
        }

        /// <summary>
        /// Called when it starts receiving input.
        /// </summary>
        public void OnStateEnter()
        {
        }

        /// <summary>
        /// Called when it stops receiving input.
        /// </summary>
        public void OnStateExit()
        {
        }

        /// <summary>
        /// This is a UI receiver.
        /// </summary>
        /// <returns></returns>
        public InputType GetInputType() => InputType.UI;

        /// <summary>
        /// Name used for input debugging.
        /// </summary>
        public string GetDebugName() => name;

        /// <summary>
        /// Switch monsters on up and down.
        /// Switch tabs on left an right.
        /// </summary>
        public void OnNavigation(InputAction.CallbackContext context)
        {
            // Get input only when key goes down.
            if (context.phase == InputActionPhase.Started)
            {
                float xInput = context.ReadValue<Vector2>().x;

                int newTab = currentTabIndex;

                do
                    switch (xInput)
                    {
                        case 1:
                            if (newTab == Tabs.Length - 1)
                                newTab = 0;
                            else
                                newTab++;

                            break;
                        case -1:
                            if (newTab == 0)
                                newTab = Tabs.Length - 1;
                            else
                                newTab--;

                            break;
                    }
                while (!displayedCategories[newTab]);

                audioManager.PlayAudio(OnNavigationAudio);

                CurrentTab.OnNavigation(context);

                // This flag attempts to fix a glitch that happens specifically on this input receiver and only on the Steam Deck,
                // where the on navigation event keeps getting called with the started flag.
                if (!switchingTab) StartCoroutine(SwitchTab(newTab));
            }
            else
                CurrentTab.OnNavigation(context);
        }

        /// <summary>
        /// Sort the buttons according to the player choices.
        /// </summary>
        public void OnExtra1(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            StartCoroutine(SortingOptions());
        }

        /// <summary>
        /// Show the player prompts for sorting and then apply the options.
        /// </summary>
        private IEnumerator SortingOptions()
        {
            int choice = -1;

            yield return WaitAFrame;

            DialogManager.ShowChoiceMenu(new List<string>
                                         {
                                             SortHelper.SortOptionToLocalizableString(SortOption.DateAdded),
                                             SortHelper.SortOptionToLocalizableString(SortOption.Alphabetically)
                                         },
                                         playerChoice => choice = playerChoice);

            yield return new WaitWhile(() => choice == -1);

            sortOption = choice == 0 ? SortOption.DateAdded : SortOption.Alphabetically;

            choice = -1;

            yield return WaitAFrame;

            DialogManager.ShowChoiceMenu(new List<string>
                                         {
                                             SortHelper.SortModeToLocalizableString(SortMode.Ascending),
                                             SortHelper.SortModeToLocalizableString(SortMode.Descending)
                                         },
                                         playerChoice => choice = playerChoice);

            yield return new WaitWhile(() => choice == -1);

            yield return WaitAFrame;

            sortMode = choice == 0 ? SortMode.Ascending : SortMode.Descending;

            yield return UpdateTab(currentTabIndex, true);
        }

        /// <summary>
        /// Show context menu for the selected item.
        /// </summary>
        /// <param name="context"></param>
        public void OnSelect(InputAction.CallbackContext context)
        {
            // Get input only when key goes down.
            if (context.phase != InputActionPhase.Started) return;

            audioManager.PlayAudio(OnNavigationAudio);

            Item item = CurrentTab.Data[CurrentTab.CurrentSelection].Key;

            List<string> options = BuildSubMenuOptions(item,
                                                       out int useTargetOption,
                                                       out bool useTargetOptionCondition,
                                                       out int useOnMoveOption,
                                                       out bool useOnMoveCondition,
                                                       out int useOption,
                                                       out bool useCondition,
                                                       out int giveTargetOption,
                                                       out bool giveTargetOptionCondition,
                                                       out int registerOption,
                                                       out bool registerCondition,
                                                       out int deregisterOption,
                                                       out bool deregisterCondition,
                                                       out int berryStatsOption,
                                                       out bool berryStatsCondition);

            DialogManager.ShowChoiceMenu(options,
                                         index =>
                                         {
                                             if (selectionMode)
                                             {
                                                 if (!IsItemCompatible(item)) return;

                                                 switch (index)
                                                 {
                                                     case 0 when sellingMode:
                                                         StartCoroutine(AttemptSellItem(item));
                                                         break;
                                                     case 0:
                                                         PlayerSelectItem(item);
                                                         break;
                                                     case 1 when item is Berry berry:
                                                         ShowBerryStats(berry);
                                                         break;
                                                 }
                                             }
                                             else
                                             {
                                                 if (index == useTargetOption && useTargetOptionCondition)
                                                     RequestItemUseOnTarget(item);

                                                 if (index == useOnMoveOption && useOnMoveCondition)
                                                     RequestItemUseOnTargetMove(item);

                                                 if (index == useOption && useCondition) ItemUse(item);

                                                 if (index == giveTargetOption && giveTargetOptionCondition)
                                                     RequestGiveItemToTarget(item);

                                                 if (index == registerOption && registerCondition)
                                                     playerBag.RegisterItem(item);

                                                 if (index == deregisterOption && deregisterCondition)
                                                     playerBag.DeregisterItem(item);

                                                 if (index == berryStatsOption && berryStatsCondition)
                                                     ShowBerryStats(item as Berry);
                                             }
                                         },
                                         onBackCallback: () => audioManager.PlayAudio(OnBackAudio));
        }

        /// <summary>
        /// Build the options for the item submenu.
        /// </summary>
        /// <param name="item">Item to build the submenu from.</param>
        /// <param name="useTargetOption">Index of the option to use on a target.</param>
        /// <param name="useTargetOptionCondition">Can it be used on a target?</param>
        /// <param name="useOnMoveOption">Index of the option to use on a move.</param>
        /// <param name="useOnMoveCondition">Can it be used on a move?</param>
        /// <param name="useOption">Index of the option to use.</param>
        /// <param name="useCondition">Can it be used?</param>
        /// <param name="giveTargetOption">Index of the option to give.</param>
        /// <param name="giveTargetOptionCondition">Can it be given?</param>
        /// <param name="registerOption">Option to register this item.</param>
        /// <param name="registerCondition">Can it be registered?</param>
        /// <param name="deregisterOption">Option to deregister this item.</param>
        /// <param name="deregisterCondition">Can it be deregistered?</param>
        /// <param name="berryStatsOption">Option to check the berry's stats.</param>
        /// <param name="berryStatsCondition">Can the berry stats be checked?</param>
        /// <returns>List of the options.</returns>
        private List<string> BuildSubMenuOptions(Item item,
                                                 out int useTargetOption,
                                                 out bool useTargetOptionCondition,
                                                 out int useOnMoveOption,
                                                 out bool useOnMoveCondition,
                                                 out int useOption,
                                                 out bool useCondition,
                                                 out int giveTargetOption,
                                                 out bool giveTargetOptionCondition,
                                                 out int registerOption,
                                                 out bool registerCondition,
                                                 out int deregisterOption,
                                                 out bool deregisterCondition,
                                                 out int berryStatsOption,
                                                 out bool berryStatsCondition)
        {
            List<string> options = new();

            if (selectionMode)
            {
                if (IsItemCompatible(item)) options.Add(sellingMode ? "Dialogs/PokeCenter/Mart/Sell" : "Common/Select");

                useTargetOption = -1;
                useTargetOptionCondition = false;
                useOnMoveOption = -1;
                useOnMoveCondition = false;
                useOption = -1;
                useCondition = false;
                giveTargetOption = -1;
                giveTargetOptionCondition = false;
                registerOption = -1;
                registerCondition = false;
                deregisterOption = -1;
                deregisterCondition = false;

                berryStatsCondition = item is Berry;
                berryStatsOption = berryStatsCondition ? 1 : -1;

                if (berryStatsCondition) options.Add("Dialogs/Berry/Stats");
            }
            else
                options = BuildNormalSubMenuOptions(options,
                                                    item,
                                                    out useTargetOption,
                                                    out useTargetOptionCondition,
                                                    out useOnMoveOption,
                                                    out useOnMoveCondition,
                                                    out useOption,
                                                    out useCondition,
                                                    out giveTargetOption,
                                                    out giveTargetOptionCondition,
                                                    out registerOption,
                                                    out registerCondition,
                                                    out deregisterOption,
                                                    out deregisterCondition,
                                                    out berryStatsOption,
                                                    out berryStatsCondition);

            options.Add("Common/Cancel");

            return options;
        }

        /// <summary>
        /// Build the options for the item submenu when not in selecting or selling mode.
        /// </summary>
        /// <param name="options">Current options.</param>
        /// <param name="item">Item to build the submenu from.</param>
        /// <param name="useTargetOption">Index of the option to use on a target.</param>
        /// <param name="useTargetOptionCondition">Can it be used on a target?</param>
        /// <param name="useOnMoveOption">Index of the option to use on a move.</param>
        /// <param name="useOnMoveCondition">Can it be used on a move?</param>
        /// <param name="useOption">Index of the option to use.</param>
        /// <param name="useCondition">Can it be used?</param>
        /// <param name="giveTargetOption">Index of the option to give.</param>
        /// <param name="giveTargetOptionCondition">Can it be given?</param>
        /// <param name="registerOption">Option to register this item.</param>
        /// <param name="registerCondition">Can it be registered?</param>
        /// <param name="deregisterOption">Option to deregister this item.</param>
        /// <param name="deregisterCondition">Can it be deregistered?</param>
        /// <param name="berryStatsOption">Option to check the berry's stats.</param>
        /// <param name="berryStatsCondition">Can the berry stats be checked?</param>
        /// <returns>List of the options.</returns>
        // ReSharper disable once CyclomaticComplexity
        private List<string> BuildNormalSubMenuOptions(List<string> options,
                                                       Item item,
                                                       out int useTargetOption,
                                                       out bool useTargetOptionCondition,
                                                       out int useOnMoveOption,
                                                       out bool useOnMoveCondition,
                                                       out int useOption,
                                                       out bool useCondition,
                                                       out int giveTargetOption,
                                                       out bool giveTargetOptionCondition,
                                                       out int registerOption,
                                                       out bool registerCondition,
                                                       out int deregisterOption,
                                                       out bool deregisterCondition,
                                                       out int berryStatsOption,
                                                       out bool berryStatsCondition)
        {
            useTargetOption = 0;

            useTargetOptionCondition =
                (item.CanBeUsedOnTarget && !inBattle) || (item.CanBeUsedInBattleOnTarget && inBattle);

            if (useTargetOptionCondition) options.Add("Common/Use");

            useOption = useTargetOption + (useTargetOptionCondition ? 1 : 0);

            useCondition =
                (item.CanBeUsedInBattle
              && inBattle
              && item.CanBeUsedInBattleRightNow(battleManager,
                                                battleManager.Battlers.GetBattlerFromBattleIndex(BattlerType.Ally,
                                                    currentBattler)))
             || (item.CanBeUsed && !inBattle);

            if (useCondition) options.Add("Common/Use");

            useOnMoveOption = useOption + (useCondition ? 1 : 0);

            useOnMoveCondition = (item.CanBeUsedOnTargetMove && !inBattle)
                              || (item.CanBeUsedInBattleOnTargetMove && inBattle);

            if (useOnMoveCondition) options.Add("Common/Use");

            giveTargetOption = useOnMoveOption + (useOnMoveCondition ? 1 : 0);
            giveTargetOptionCondition = item.CanBeHeld && !inBattle;

            if (giveTargetOptionCondition) options.Add("Common/Give");

            registerOption = giveTargetOption + (giveTargetOptionCondition ? 1 : 0);
            registerCondition = !inBattle && item.CanBeRegistered && !playerBag.IsItemRegistered(item);

            if (registerCondition) options.Add("Common/Register");

            deregisterOption = registerOption + (registerCondition ? 1 : 0);
            deregisterCondition = !inBattle && item.CanBeRegistered && playerBag.IsItemRegistered(item);

            if (deregisterCondition) options.Add("Common/Deregister");

            berryStatsOption = deregisterOption + (deregisterCondition ? 1 : 0);
            berryStatsCondition = item is Berry;

            if (berryStatsCondition) options.Add("Dialogs/Berry/Stats");

            return options;
        }

        /// <summary>
        /// Close the bag when back if pressed.
        /// </summary>
        /// <param name="context"></param>
        public void OnBack(InputAction.CallbackContext context)
        {
            // Get input only when key goes down.
            if (context.phase != InputActionPhase.Started) return;

            onBackCallback?.Invoke(false, null);

            audioManager.PlayAudio(OnBackAudio);

            Hide();
        }

        /// <summary>
        /// Switch to the given tab.
        /// </summary>
        /// <param name="index">Index of the new tab.</param>
        /// <param name="force">Force switching events.</param>
        private IEnumerator SwitchTab(int index, bool force = false)
        {
            if (!force && index == currentTabIndex) yield break;

            switchingTab = true;

            CurrentTab.OnHovered -= OnItemHovered;

            CurrentTab.OnStateExit();

            Icons[currentTabIndex].OnDeselect();

            CurrentTab.Show(false);

            yield return UpdateTab(index, false);

            yield return WaitAFrame;

            currentTabIndex = index;

            CurrentTab.OnHovered += OnItemHovered;

            CurrentTab.Show();

            TabTitle.SetValue(CategoriesToDisplay[currentTabIndex].LocalizableName);

            Icons[currentTabIndex].OnSelect();

            CurrentTab.OnStateEnter();

            if (CurrentTab.Data.Count > 0) OnItemHovered(CurrentTab.CurrentSelection);

            switchingTab = false;
        }

        /// <summary>
        /// Called when an item is hovered.
        /// </summary>
        /// <param name="index">Hovered item.</param>
        private void OnItemHovered(int index)
        {
            Item item = CurrentTab.Data[index].Key;

            if (inBattle)
                for (int i = 0; i < MonsterMenu.MenuOptions.Count; i++)
                {
                    MonsterButtonWithCompatibilityPanel button =
                        MonsterMenu.MenuOptions[i] as MonsterButtonWithCompatibilityPanel;

                    if (button == null) return;

                    if (battleManager.Rosters.GetRoster(BattlerType.Ally, 0).Count > i)
                    {
                        Battler battler = battleManager
                                         .Battlers.GetBattlerFromRosterAndIndex(BattlerType.Ally,
                                                                                    0,
                                                                                    i);

                        button.SetCompatibility(item == null
                                             || (!item // Don't display no effect if it can't be used.
                                                    .CanBeUsedInBattleOnTarget
                                              && !item
                                                    .CanBeUsedInBattleOnTargetMove)
                                             || item.IsCompatible(battleManager, battler)
                                             || item.IsMonsterCompatibleForMoveTargeting(battleManager, battler));
                    }
                    else
                        button.SetCompatibility(false);
                }
            else
                foreach (MonsterButtonWithCompatibilityPanel button in MonsterMenu.MenuOptions
                            .Cast<
                                 MonsterButtonWithCompatibilityPanel>())
                    if (button.Monster is {IsNullEntry: false})
                        button.SetCompatibility(item == null
                                             || (!item // Don't display no effect if it can't be used.
                                                    .CanBeUsedOnTarget
                                              && !item
                                                    .CanBeUsedOnTargetMove)
                                             || item.IsCompatible(settings,
                                                                  timeManager,
                                                                  playerCharacter,
                                                                  button.Monster)
                                             || item.IsMonsterCompatibleForMoveTargeting(button.Monster));

            ItemDescription.SetItem(item);
            PurchasePriceValue.SetMoney(item.SellPrice);
            PurchasePricePanel.Show(selectionMode && sellingMode && item.CanBeSold);
        }

        /// <summary>
        /// Select an item and close the bag.
        /// </summary>
        /// <param name="item">Item selected.</param>
        private void PlayerSelectItem(Item item)
        {
            onBackCallback?.Invoke(true, item);

            audioManager.PlayAudio(OnBackAudio);

            Hide();
        }

        /// <summary>
        /// Sell the selected item.
        /// </summary>
        /// <param name="item">Item to sell.</param>
        private IEnumerator AttemptSellItem(Item item)
        {
            if (item.CanBeSold)
            {
                uint amountToSell = 1;
                uint bagAmount = (uint) playerBag.GetItemAmount(item);

                if (bagAmount > 1)
                    yield return ShopAmountSelector.RequestAmount(item.SellPrice,
                                                                  bagAmount,
                                                                  (newAmount, accepted) =>
                                                                  {
                                                                      amountToSell = accepted ? newAmount : 0;
                                                                  });

                if (amountToSell == 0) yield break;

                uint finalPrice = amountToSell * item.SellPrice;

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
                                                            amountToSell.ToString(),
                                                            item.GetName(localizer),
                                                            MoneyHelper.BuildMoneyString(finalPrice,
                                                                settings,
                                                                localizer)
                                                        });

                yield return new WaitWhile(() => choice == -1);

                if (choice == 1) yield break;

                int result = playerBag.ChangeItemAmount(item, -(int) amountToSell);
                playerBag.Money += finalPrice;

                MoneyText.SetMoney(playerBag.Money);

                // If item was removed, need to refresh whole tab.
                yield return UpdateTab(currentTabIndex, result == 2);

                yield return DialogManager.ShowDialogAndWait("Dialogs/PokeCenter/Mart/ThanksSell");
            }
            else
                yield return DialogManager.ShowDialogAndWait("Dialogs/PokeCenter/Mart/CantBuy");
        }

        /// <summary>
        /// Use an item.
        /// </summary>
        /// <param name="itemToUse">Item to use.</param>
        private void ItemUse(Item itemToUse) => StartCoroutine(ItemUseRoutine(itemToUse));

        /// <summary>
        /// Use an item.
        /// </summary>
        /// <param name="itemToUse">Item to use.</param>
        private IEnumerator ItemUseRoutine(Item itemToUse)
        {
            if (inBattle)
            {
                onBattleItemSelected?.Invoke(new BattleAction
                                             {
                                                 BattlerType = BattlerType.Ally,
                                                 Index = currentBattler,
                                                 ActionType = BattleAction.Type.Item,
                                                 Parameters =
                                                     new[] {0, playerBag.GetIndexOfItem(itemToUse)}
                                             });

                Hide();
            }
            else
            {
                bool consumeItem = false;

                yield return WaitAFrame;

                yield return itemToUse.UseOutOfBattle(playerSettings,
                                                      playerCharacter,
                                                      localizer,
                                                      shouldConsume => consumeItem = shouldConsume);

                yield return WaitAFrame;

                if (itemToUse.NeedsPanelAnimationInBagScreen)
                {
                    inputManager.BlockInput();

                    int finished = 0;

                    foreach (MonsterButton button in MonsterMenu.MenuOptions.Cast<MonsterButton>())
                        if (button.Monster != null)
                            // ReSharper disable once AccessToModifiedClosure
                            button.Panel.UpdatePanel(1, false, true, () => finished++);
                        else
                            finished++;

                    yield return new WaitWhile(() => finished < 6);

                    inputManager.BlockInput(false);
                }

                int result = -1;

                if (consumeItem) result = playerBag.ChangeItemAmount(itemToUse, -1);

                DialogManager.AcceptInput = true;

                yield return DialogManager.WaitForDialog;

                // If item was removed, need to refresh whole tab.
                if (consumeItem) yield return UpdateTab(currentTabIndex, result == 2);

                // Update the item name and description in case they changed.
                if (CurrentTab.Data.Count > 0) OnItemHovered(CurrentTab.CurrentSelection);
            }
        }

        /// <summary>
        /// Request the player to select a target to use an item on.
        /// </summary>
        /// <param name="itemToUse">Item to be used.</param>
        private void RequestItemUseOnTarget(Item itemToUse) => StartCoroutine(RequestItemUseOnTargetRoutine(itemToUse));

        /// <summary>
        /// Request the player to select a target to use an item on.
        /// </summary>
        /// <param name="itemToUse">Item to be used.</param>
        private IEnumerator RequestItemUseOnTargetRoutine(Item itemToUse)
        {
            yield return WaitAFrame;

            MonsterMenu.OnButtonSelected += index =>
                                            {
                                                MonsterButtonWithCompatibilityPanel button =
                                                    MonsterMenu.MenuOptions[index] as
                                                        MonsterButtonWithCompatibilityPanel;

                                                MonsterInstance monster = null;

                                                if (button != null) monster = button.Monster;

                                                if (button == null
                                                 || monster?.IsNullEntry != false
                                                 || !button.IsCompatible)
                                                {
                                                    // Reselect as it gets auto deselected.
                                                    DOVirtual.DelayedCall(.01f, () => MonsterMenu.Select(index));

                                                    return;
                                                }

                                                MonsterMenu.OnButtonSelected = null;
                                                MonsterMenu.OnBackSelected = null;

                                                MonsterMenu.ReleaseInput();

                                                if (inBattle)
                                                {
                                                    onBattleItemSelected?.Invoke(new BattleAction
                                                        {
                                                            BattlerType = BattlerType.Ally,
                                                            Index = currentBattler,
                                                            ActionType = BattleAction.Type.Item,
                                                            Parameters = new[]
                                                                         {
                                                                             1,
                                                                             playerBag.GetIndexOfItem(itemToUse),
                                                                             (int) BattlerType.Ally,
                                                                             0,
                                                                             index
                                                                         }
                                                        });

                                                    Hide();
                                                }
                                                else
                                                    StartCoroutine(UseItemOnMonster(itemToUse, monster, button));
                                            };

            MonsterMenu.OnBackSelected += () =>
                                          {
                                              MonsterMenu.OnButtonSelected = null;
                                              MonsterMenu.OnBackSelected = null;

                                              MonsterMenu.ReleaseInput();
                                          };

            MonsterMenu.RequestInput();
        }

        /// <summary>
        /// Request the player to choose a monster to give an item to.
        /// </summary>
        /// <param name="itemToGive">Item to give.</param>
        private void RequestGiveItemToTarget(Item itemToGive) =>
            StartCoroutine(RequestGiveItemToTargetRoutine(itemToGive));

        /// <summary>
        /// Request the player to choose a monster to give an item to.
        /// </summary>
        /// <param name="itemToGive">Item to give.</param>
        private IEnumerator RequestGiveItemToTargetRoutine(Item itemToGive)
        {
            yield return WaitAFrame;

            MonsterMenu.OnButtonSelected += index =>
                                            {
                                                MonsterButtonWithCompatibilityPanel button =
                                                    MonsterMenu.MenuOptions[index] as
                                                        MonsterButtonWithCompatibilityPanel;

                                                MonsterInstance monster = null;

                                                if (button != null) monster = button.Monster;

                                                if (button == null
                                                 || monster?.IsNullEntry != false)
                                                {
                                                    // Reselect as it gets auto deselected.
                                                    DOVirtual.DelayedCall(.01f, () => MonsterMenu.Select(index));

                                                    return;
                                                }

                                                MonsterMenu.OnButtonSelected = null;
                                                MonsterMenu.OnBackSelected = null;

                                                MonsterMenu.ReleaseInput();

                                                StartCoroutine(AttemptGiveItem(itemToGive, monster, button));
                                            };

            MonsterMenu.OnBackSelected += () =>
                                          {
                                              MonsterMenu.OnButtonSelected = null;
                                              MonsterMenu.OnBackSelected = null;

                                              MonsterMenu.ReleaseInput();
                                          };

            MonsterMenu.RequestInput();
        }

        /// <summary>
        /// Request the player to select a target move to use an item on.
        /// </summary>
        /// <param name="itemToUse">Item to be used.</param>
        private void RequestItemUseOnTargetMove(Item itemToUse) =>
            StartCoroutine(RequestItemUseOnTargetMoveRoutine(itemToUse));

        /// <summary>
        /// Request the player to select a target move to use an item on.
        /// </summary>
        /// <param name="itemToUse">Item to be used.</param>
        private IEnumerator RequestItemUseOnTargetMoveRoutine(Item itemToUse)
        {
            yield return WaitAFrame;

            MonsterMenu.OnButtonSelected += index =>
                                            {
                                                MonsterButtonWithCompatibilityPanel button =
                                                    MonsterMenu.MenuOptions[index] as
                                                        MonsterButtonWithCompatibilityPanel;

                                                MonsterInstance monster = null;

                                                if (button != null) monster = button.Monster;

                                                if (button == null
                                                 || monster?.IsNullEntry != false
                                                 || !button.IsCompatible)
                                                {
                                                    // Reselect as it gets auto deselected.
                                                    DOVirtual.DelayedCall(.01f, () => MonsterMenu.Select(index));

                                                    return;
                                                }

                                                MonsterMenu.OnButtonSelected = null;
                                                MonsterMenu.OnBackSelected = null;

                                                MonsterMenu.ReleaseInput();

                                                RequestItemUseOnTargetMonsterAndMove(itemToUse, button, monster, index);
                                            };

            MonsterMenu.OnBackSelected += () =>
                                          {
                                              MonsterMenu.OnButtonSelected = null;
                                              MonsterMenu.OnBackSelected = null;

                                              MonsterMenu.ReleaseInput();
                                          };

            MonsterMenu.RequestInput();
        }

        /// <summary>
        /// After selecting a monster, ask the player to choose a compatible move of that monster to use the item on.
        /// </summary>
        /// <param name="itemToUse">Item to use.</param>
        /// <param name="button">Button representing that monster.</param>
        /// <param name="monster">Targeted monster.</param>
        /// <param name="monsterIndex">Index of that monster.</param>
        private void RequestItemUseOnTargetMonsterAndMove(Item itemToUse,
                                                          MonsterButton button,
                                                          MonsterInstance monster,
                                                          int monsterIndex) =>
            StartCoroutine(RequestItemUseOnTargetMonsterAndMoveRoutine(itemToUse, button, monster, monsterIndex));

        /// <summary>
        /// After selecting a monster, ask the player to choose a compatible move of that monster to use the item on.
        /// </summary>
        /// <param name="itemToUse">Item to use.</param>
        /// <param name="button">Button representing that monster.</param>
        /// <param name="monster">Targeted monster.</param>
        /// <param name="monsterIndex">Index of that monster.</param>
        private IEnumerator RequestItemUseOnTargetMonsterAndMoveRoutine(Item itemToUse,
                                                                        MonsterButton button,
                                                                        MonsterInstance monster,
                                                                        int monsterIndex)
        {
            List<string> options = new();
            Dictionary<int, int> menuIndexToMoveIndex = new();

            for (int i = 0; i < monster.CurrentMoves.Length; ++i)
            {
                if ((!inBattle && !itemToUse.IsMoveCompatible(monster, i))
                 || (inBattle
                  && !itemToUse.IsMoveCompatible(battleManager,
                                                 battleManager.Battlers.GetBattlerFromRosterAndIndex(BattlerType.Ally,
                                                     0,
                                                     monsterIndex),
                                                 i)))
                    continue;

                options.Add(monster.CurrentMoves[i].Move.LocalizableName);
                menuIndexToMoveIndex[options.Count - 1] = i;
            }

            options.Add("Common/Cancel");
            menuIndexToMoveIndex[options.Count - 1] = -1;

            DialogManager.ShowChoiceMenu(options,
                                         selectedMove =>
                                         {
                                             int moveIndex = menuIndexToMoveIndex[selectedMove];

                                             if (moveIndex == -1) return; // Similar to canceling.

                                             if (inBattle)
                                             {
                                                 onBattleItemSelected?.Invoke(new BattleAction
                                                                              {
                                                                                  BattlerType = BattlerType.Ally,
                                                                                  Index = currentBattler,
                                                                                  ActionType = BattleAction.Type.Item,
                                                                                  Parameters = new[]
                                                                                      {
                                                                                          2,
                                                                                          playerBag
                                                                                             .GetIndexOfItem(itemToUse),
                                                                                          (int) BattlerType.Ally,
                                                                                          0,
                                                                                          monsterIndex,
                                                                                          moveIndex
                                                                                      }
                                                                              });

                                                 Hide();
                                             }
                                             else
                                                 StartCoroutine(UseItemOnMove(itemToUse,
                                                                              monster,
                                                                              button,
                                                                              moveIndex));
                                         },
                                         button.ContextMenuPosition,
                                         onBackCallback: () =>
                                                         {
                                                             // Empty function is enough to enable going back.
                                                         });

            yield break; // TODO: Maybe it doesn't need to be a coroutine.
        }

        /// <summary>
        /// Use an item on the target monster.
        /// </summary>
        /// <param name="itemToUse">Item to be used.</param>
        /// <param name="monster">Monster to use it on.</param>
        /// <param name="button">Button that monster has.</param>
        private IEnumerator UseItemOnMonster(Item itemToUse,
                                             MonsterInstance monster,
                                             MonsterButton button)
        {
            bool consume = false;

            yield return itemToUse.UseOnTarget(monster,
                                               settings,
                                               experienceLookupTable,
                                               playerCharacter,
                                               timeManager,
                                               evolutionManager,
                                               inputManager,
                                               localizer,
                                               shouldConsume => consume = shouldConsume);

            int result = -1;

            if (consume) result = playerBag.ChangeItemAmount(itemToUse, -1);

            // Update the item name and description in case they changed.
            if (CurrentTab.Data.Count > 0) OnItemHovered(CurrentTab.CurrentSelection);

            if (!consume) yield break;

            // If item was removed, need to refresh whole tab.
            yield return UpdateTab(currentTabIndex, result == 2);

            bool finished = false;

            button.Panel.UpdatePanel(1, false, itemToUse.NeedsPanelAnimationInBagScreen, () => finished = true);

            if (itemToUse.RequiresUpdatingTheEntirePartyInBagScreen) DisplayMonsters();

            yield return new WaitUntil(() => finished);

            DialogManager.AcceptInput = true;

            yield return DialogManager.WaitForDialog;
        }

        /// <summary>
        /// Use an item on the target monster.
        /// </summary>
        /// <param name="itemToUse">Item to be used.</param>
        /// <param name="monster">Monster to use it on.</param>
        /// <param name="button">Button that monster has.</param>
        /// <param name="moveIndex">Move index</param>
        private IEnumerator UseItemOnMove(Item itemToUse,
                                          MonsterInstance monster,
                                          MonsterButton button,
                                          int moveIndex)
        {
            yield return WaitAFrame;

            bool consume = false;

            yield return itemToUse.UseOnTargetMove(monster,
                                                   moveIndex,
                                                   playerCharacter,
                                                   localizer,
                                                   shouldConsume => consume = shouldConsume);

            int result = -1;

            if (consume) result = playerBag.ChangeItemAmount(itemToUse, -1);

            yield return WaitAFrame;

            // Update the item name and description in case they changed.
            if (CurrentTab.Data.Count > 0) OnItemHovered(CurrentTab.CurrentSelection);

            // If item was removed, need to refresh whole tab.
            yield return UpdateTab(currentTabIndex, result == 2);

            bool finished = false;

            button.Panel.UpdatePanel(1, false, true, () => finished = true);

            yield return new WaitUntil(() => finished);

            DialogManager.AcceptInput = true;

            yield return DialogManager.WaitForDialog;
        }

        /// <summary>
        /// Attempt to give an item to the selected monster.
        /// </summary>
        /// <param name="itemToGive">Item to give.</param>
        /// <param name="monster">Monster to give it to.</param>
        /// <param name="button">That monster's button.</param>
        private IEnumerator AttemptGiveItem(Item itemToGive,
                                            MonsterInstance monster,
                                            MonsterButton button)
        {
            bool readyToGive = false;
            bool cancel = false;

            if (monster.EggData.IsEgg)
            {
                yield return DialogManager.ShowDialogAndWait("Item/EggsCantHold");
                yield break;
            }

            if (monster.HeldItem != null)
                DialogManager.ShowChoiceMenu(new List<string>
                                             {
                                                 "Common/True",
                                                 "Common/False"
                                             },
                                             index =>
                                             {
                                                 if (index == 0)
                                                 {
                                                     DialogManager.ShowDialog("Item/PutBack",
                                                                              localizableModifiers: false,
                                                                              modifiers: monster.HeldItem
                                                                                 .GetName(localizer));

                                                     int result = playerBag.ChangeItemAmount(monster.HeldItem, 1);

                                                     StartCoroutine(UpdateTab(CategoriesToDisplay
                                                                                 .IndexOf(monster.HeldItem
                                                                                     .ItemCategory),
                                                                              result == 2));

                                                     monster.HeldItem = null;

                                                     readyToGive = true;
                                                 }
                                                 else
                                                 {
                                                     readyToGive = true;
                                                     cancel = true;
                                                 }
                                             },
                                             showDialog: true,
                                             localizationKey: "Item/SwapDialog",
                                             localizableModifiers: false,
                                             modifiers: new[]
                                                        {
                                                            monster.HeldItem.GetName(localizer),
                                                            itemToGive.GetName(localizer)
                                                        });
            else
                readyToGive = true;

            yield return new WaitUntil(() => readyToGive);

            if (cancel) yield break;

            monster.HeldItem = itemToGive;

            int result = playerBag.ChangeItemAmount(itemToGive, -1);

            yield return DialogManager.ShowDialogAndWait("Item/NowHolding",
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        monster.GetNameOrNickName(localizer),
                                                                        itemToGive.GetName(localizer)
                                                                    });

            button.Panel.UpdatePanel(1, false);

            yield return UpdateTab(currentTabIndex, result == 2);
        }

        /// <summary>
        /// Show the stats of the given berry.
        /// </summary>
        /// <param name="berry">Berry to show.</param>
        private void ShowBerryStats(Berry berry) =>
            DialogManager.ShowDialog("Common/Wildcard",
                                     localizableModifiers: false,
                                     modifiers: berry.GetLocalizedStatInformation(localizer));

        /// <summary>
        /// Update a single tab.
        /// </summary>
        /// <param name="index">Index of the tab to update.</param>
        /// <param name="clearButtons">Clear the previous buttons?</param>
        private IEnumerator UpdateTab(int index, bool clearButtons)
        {
            inputManager.BlockInput();

            Tabs[index].OnStateExit();

            if (clearButtons) Tabs[index].ClearButtons();

            Tabs[index].SetButtons(GetSortedItemList(index), clearButtons);

            yield return WaitAFrame;

            Tabs[index].OnStateEnter();

            inputManager.BlockInput(false);
        }

        /// <summary>
        /// Get the sorted item list for the given tab index.
        /// </summary>
        /// <param name="index">Index of the tab.</param>
        /// <returns>The sorted list of items.</returns>
        private Dictionary<Item, int> GetSortedItemList(int index)
        {
            Dictionary<Item, int> items = playerBag.GetItemsForCategory(CategoriesToDisplay[index]);

            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            return sortOption switch
            {
                SortOption.DateAdded when sortMode == SortMode.Ascending => items,
                SortOption.DateAdded => items.Reverse().ToDictionary(x => x.Key, x => x.Value),
                SortOption.Alphabetically when sortMode == SortMode.Ascending => items
                   .OrderBy(pair => pair.Key.GetName(localizer))
                   .ToDictionary(x => x.Key, x => x.Value),
                SortOption.Alphabetically => items.OrderByDescending(pair => pair.Key.GetName(localizer))
                                                  .ToDictionary(x => x.Key, x => x.Value),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        /// <summary>
        /// Display the monsters on the sid panel by the bag.
        /// </summary>
        private void DisplayMonsters()
        {
            MonsterInstance[] monsters = GetMonsters();

            for (int i = 0; i < 6; ++i)
            {
                MonsterInstance monster = monsters.Length > i ? monsters[i] : null;

                if (monster?.IsNullEntry != false)
                    MonsterMenu.MenuOptions[i].Hide();
                else
                {
                    ((MonsterButton) MonsterMenu.MenuOptions[i]).Panel.SetMonster(monster, false);
                    MonsterMenu.MenuOptions[i].Show();
                }
            }
        }

        /// <summary>
        /// Get the monsters to display by the bag.
        /// </summary>
        /// <returns></returns>
        private MonsterInstance[] GetMonsters() =>
            inBattle
                // ReSharper disable once CoVariantArrayConversion
                ? battleManager.Rosters.GetRoster(BattlerType.Ally, 0).ToArray()
                : displayedRoster.ToArray();

        /// <summary>
        /// Check if an item is compatible according to the compatibility checker.
        /// </summary>
        /// <param name="item">Item to check.</param>
        /// <returns>True if it is.</returns>
        private bool IsItemCompatible(Item item) =>
            compatibilityChecker == null || compatibilityChecker.IsItemCompatible(item);

        #region Unused input callbacks

        public void OnMovement(InputAction.CallbackContext context)
        {
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
        }

        public void OnMenu(InputAction.CallbackContext context)
        {
        }

        public void OnRun(InputAction.CallbackContext context)
        {
        }

        public void OnExtra(InputAction.CallbackContext context)
        {
        }

        public void OnExtra2(InputAction.CallbackContext context)
        {
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
        }

        public void OnTextBackspace(InputAction.CallbackContext context)
        {
        }

        public void OnTextSubmit(InputAction.CallbackContext context)
        {
        }

        public void OnTextCancel(InputAction.CallbackContext context)
        {
        }

        public void OnAnyTextKey(InputAction.CallbackContext context)
        {
        }

        #endregion
    }
}