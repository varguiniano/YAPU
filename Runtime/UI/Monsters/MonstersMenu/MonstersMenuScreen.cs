using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph.CommandUtils;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Bags;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.UI.Map;
using Varguiniano.YAPU.Runtime.UI.Types;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.Localization.Runtime.Ui;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Monsters.MonstersMenu
{
    /// <summary>
    /// Controller for the screen that shows the monsters menu.
    /// </summary>
    public class MonstersMenuScreen : HidableUiElement<MonstersMenuScreen>, IPlayerDataReceiver, IInputReceiver
    {
        /// <summary>
        /// Event called when the menu is closed.
        /// It passes a bool stating if the player chose a monster, the monster reference and if it came from the roster (true) or the storage (false).
        /// There params are useful when the menu is opened as a dialog to chose a monster.
        /// </summary>
        public Action<bool, MonsterInstance, bool> OnMenuClosed;

        /// <summary>
        /// Reference to the background image.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private Image Background;

        /// <summary>
        /// Left section of the menu.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private Transform LeftSection;

        /// <summary>
        /// Position for the left section when it is closed.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private Transform LeftSectionClosedPosition;

        /// <summary>
        /// Position of the left section when it is open.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private Transform LeftSectionOpenPosition;

        /// <summary>
        /// Storage section of the menu.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private Transform StorageSection;

        /// <summary>
        /// Position for the Storage section when it is closed.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private Transform StorageSectionClosedPosition;

        /// <summary>
        /// Position of the Storage section when it is open.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private Transform StorageSectionOpenPosition;

        /// <summary>
        /// Reference to the storage tip.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private HidableUiElement StorageTip;

        /// <summary>
        /// Upper section of the menu.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private Transform UpperSection;

        /// <summary>
        /// Position for the Upper section when it is closed.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private Transform UpperSectionClosedPosition;

        /// <summary>
        /// Position of the Upper section when it is open.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private Transform UpperSectionOpenPosition;

        /// <summary>
        /// Reference to the monsters selector.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private MenuSelector MonstersSelector;

        /// <summary>
        /// Reference to the storage selector.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private StorageMenu StorageSelector;

        /// <summary>
        /// State of this monster.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private LocalizedTextMeshPro State;

        /// <summary>
        /// Reference to the first type badge.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private TypeBadge FirstType;

        /// <summary>
        /// Reference to the second type badge.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private TypeBadge SecondType;

        /// <summary>
        /// Reference to the monster sprite.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private HidableUiElement MonsterSpriteHider;

        /// <summary>
        /// Reference to the monster sprite.
        /// </summary>
        [FormerlySerializedAs("MonsterSprite")]
        [FoldoutGroup("References")]
        [SerializeField]
        private UIMonsterSprite UIMonsterSprite;

        /// <summary>
        /// Transform to indicate where the context menu should be.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform ContextMenuPosition;

        /// <summary>
        /// Reference to the summary shown when the storage is open.
        /// </summary>
        [FormerlySerializedAs("StorageMonsterSummary")]
        [FoldoutGroup("References")]
        [SerializeField]
        private MiniMonsterSummary MiniMonsterSummary;

        /// <summary>
        /// Reference to the sorting menu.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private StorageSortingMenu SortingMenu;

        /// <summary>
        /// Open position for menu tips.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform TipsOpenPosition;

        /// <summary>
        /// Closed position for menu tips.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform TipsClosedPosition;

        /// <summary>
        /// Tip to prompt the user to swap.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform SwappingTip;

        /// <summary>
        /// Tip to prompt the user to sort and filter the storage.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private HidableUiElement SortingTip;

        /// <summary>
        /// Duration of the open/close animation.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("Animation")]
        private float OpenCloseDuration = 0.25f;

        /// <summary>
        /// Audio to play when the summary menu is closed.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("Animation")]
        private AudioReference OnSummaryBackAudio;

        /// <summary>
        /// Audio to play when the storage is opened and closed.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("Animation")]
        private AudioReference StorageOpenCloseAudio;

        /// <summary>
        /// Audio to play when navigating from one menu to another.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("Animation")]
        private AudioReference NavigationAudio;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Reference to the audio manager.
        /// </summary>
        [Inject]
        private IAudioManager audioManager;

        /// <summary>
        /// Reference to the player bag.
        /// </summary>
        [Inject]
        private Bag playerBag;

        /// <summary>
        /// Reference to the player's storage.
        /// </summary>
        [Inject]
        private MonsterStorage storage;

        /// <summary>
        /// Reference to the input manager.
        /// </summary>
        [Inject]
        private IInputManager inputManager;

        /// <summary>
        /// Reference to the map scene launcher.
        /// </summary>
        [Inject]
        private MapSceneLauncher mapSceneLauncher;

        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        [Inject]
        private YAPUSettings settings;

        /// <summary>
        /// Is the menu opening or closing?
        /// </summary>
        private bool OpeningOrClosing => opening || closing;

        /// <summary>
        /// Is the menu opening?
        /// </summary>
        private bool opening;

        /// <summary>
        /// Is the menu closing?
        /// </summary>
        private bool closing;

        /// <summary>
        /// Reference to the player character.
        /// </summary>
        private PlayerCharacter playerCharacter;

        /// <summary>
        /// Currently used roster.
        /// </summary>
        private Roster currentRoster;

        /// <summary>
        /// List of displayed monsters.
        /// </summary>
        private List<MonsterInstance> monsters;

        /// <summary>
        /// Index of the monster currently selected.
        /// </summary>
        private int currentSelectedIndex;

        /// <summary>
        /// Flag to know when the monsters are being swapped.
        /// </summary>
        private bool Swapping
        {
            get => swapping;
            set
            {
                swapping = value;

                SwappingTip.DOLocalMove(swapping ? TipsOpenPosition.localPosition : TipsClosedPosition.localPosition,
                                        .25f)
                           .SetEase(swapping ? Ease.OutBack : Ease.InBack);
            }
        }

        /// <summary>
        /// Backfield for swapping.
        /// </summary>
        private bool swapping;

        /// <summary>
        /// Flag to know when we are downloading a monster and swapping it with one on the roster.
        /// </summary>
        private bool DownloadingAndSwapping
        {
            get => downloadingAndSwapping;

            set
            {
                downloadingAndSwapping = value;

                SwitchSelectedMenu(MonstersSelector);

                SwappingTip.DOLocalMove(downloadingAndSwapping
                                            ? TipsOpenPosition.localPosition
                                            : TipsClosedPosition.localPosition,
                                        .25f)
                           .SetEase(downloadingAndSwapping ? Ease.OutBack : Ease.InBack);
            }
        }

        /// <summary>
        /// Backfield for DownloadingAndSwapping
        /// </summary>
        private bool downloadingAndSwapping;

        /// <summary>
        /// Index of the monster being swap.
        /// </summary>
        private int indexToSwap = -1;

        /// <summary>
        /// Can the player open the storage?
        /// </summary>
        private bool canOpenStorage;

        /// <summary>
        /// Currently selected menu.
        /// </summary>
        private MenuSelector currentSelectedMenu;

        /// <summary>
        /// Flag to mark if the storage is open.
        /// </summary>
        private bool isStorageOpen;

        /// <summary>
        /// Should we allow the player to close the menu?
        /// </summary>
        private bool shouldAllowClosing;

        /// <summary>
        /// Flag to know if the storage was opened directly when the menu was opened.
        /// </summary>
        private bool storageWasOpenedDirectly;

        /// <summary>
        /// Flag to know if the menu was opened as a dialog to select a monster.
        /// </summary>
        private bool isSelectingMonster;

        /// <summary>
        /// Checker to use to check if a monster is compatible with the current selection dialog.
        /// </summary>
        private MonsterCompatibilityChecker monsterCompatibilityChecker;

        /// <summary>
        /// Flag to know if the menu was opened as a dialog to select a monster to swap.
        /// </summary>
        private bool isSwapDialog;

        /// <summary>
        /// Subscribe to the main menu selecting moves.
        /// </summary>
        private void OnEnable()
        {
            MonstersSelector.OnButtonSelected += OnMonsterSelected;
            MonstersSelector.OnBackSelected += OnBackSelected;
            MonstersSelector.OnHovered += OnMonsterHovered;
            StorageSelector.OnButtonSelected += OnMonsterSelected;
            StorageSelector.OnBackSelected += OnBackSelected;
            StorageSelector.OnHovered += OnMonsterHovered;
            SortingMenu.OnMenuClosed += OnSortingMenuClosed;
        }

        /// <summary>
        /// Unsubscribe.
        /// </summary>
        private void OnDisable()
        {
            MonstersSelector.OnButtonSelected -= OnMonsterSelected;
            MonstersSelector.OnBackSelected -= OnBackSelected;
            MonstersSelector.OnHovered -= OnMonsterHovered;
            StorageSelector.OnButtonSelected -= OnMonsterSelected;
            StorageSelector.OnBackSelected -= OnBackSelected;
            StorageSelector.OnHovered -= OnMonsterHovered;
            SortingMenu.OnMenuClosed -= OnSortingMenuClosed;
        }

        /// <summary>
        /// Open the menu.
        /// </summary>
        /// <param name="monstersToDisplay">Monsters to show on the menu.</param>
        /// <param name="playerCharacterReference">Reference to the player character.</param>
        /// <param name="allowStorage">Allow to open the storage? The current scene must also allow it.</param>
        /// <param name="allowClosing">Allow closing the menu.</param>
        /// <param name="openStorageDirectly">Open the storage right when the menu opens?</param>
        /// <param name="isChoosingDialog">Is this opening part of a dialog in which the player must choose a monster?</param>
        /// <param name="compatibilityChecker">Compatibility checker to check if the monster is compatible for the choosing dialog.</param>
        /// <param name="isChoosingToSwapDialog">Is opening part of a dialog to swap a monster for another?</param>
        [Button]
        [FoldoutGroup("Debug")]
        public void OpenMenu(Roster monstersToDisplay,
                             PlayerCharacter playerCharacterReference,
                             bool allowStorage,
                             bool allowClosing,
                             bool openStorageDirectly,
                             bool isChoosingDialog,
                             MonsterCompatibilityChecker compatibilityChecker,
                             bool isChoosingToSwapDialog)
        {
            if (!OpeningOrClosing)
                StartCoroutine(OpenMenuRoutine(monstersToDisplay,
                                               playerCharacterReference,
                                               allowStorage,
                                               allowClosing,
                                               openStorageDirectly,
                                               isChoosingDialog,
                                               compatibilityChecker,
                                               isChoosingToSwapDialog));
        }

        /// <summary>
        /// Open the menu.
        /// </summary>
        /// <param name="monstersToDisplay">Monsters to show on the menu.</param>
        /// <param name="playerCharacterReference">Reference to the player character.</param>
        /// <param name="allowStorage">Allow to open the storage? The current scene must also allow it.</param>
        /// <param name="allowClosing">Allow closing the menu.</param>
        /// <param name="openStorageDirectly">Open the storage right when the menu opens?</param>
        /// <param name="isChoosingDialog">Is this opening part of a dialog in which the player must choose a monster?</param>
        /// <param name="compatibilityChecker">Compatibility checker to check if the monster is compatible for the choosing dialog.</param>
        /// <param name="isChoosingToSwapDialog">Is opening part of a dialog to swap a monster for another?</param>
        private IEnumerator OpenMenuRoutine(Roster monstersToDisplay,
                                            PlayerCharacter playerCharacterReference,
                                            bool allowStorage,
                                            bool allowClosing,
                                            bool openStorageDirectly,
                                            bool isChoosingDialog,
                                            MonsterCompatibilityChecker compatibilityChecker,
                                            bool isChoosingToSwapDialog)
        {
            opening = true;

            inputManager.BlockInput();

            playerCharacter = playerCharacterReference;

            canOpenStorage = allowStorage && playerCharacter.CharacterController.CurrentGrid.SceneInfo.CanOpenStorage;

            storageWasOpenedDirectly = openStorageDirectly;

            isSelectingMonster = isChoosingDialog;
            monsterCompatibilityChecker = compatibilityChecker;
            isSwapDialog = isChoosingToSwapDialog;

            shouldAllowClosing = allowClosing;

            SetMonsters(monstersToDisplay);

            MonsterSpriteHider.Show(false);

            LeftSection.localPosition = LeftSectionClosedPosition.localPosition;

            StorageSection.localPosition = StorageSectionClosedPosition.localPosition;

            UpperSection.localPosition = UpperSectionClosedPosition.localPosition;

            SwappingTip.localPosition = TipsClosedPosition.localPosition;

            currentSelectedIndex = 0;

            Show();

            DialogManager.ShowLoadingIcon();

            Background.DOFade(1, OpenCloseDuration);

            yield return UpdateStorageRoutine();

            UpperSection.DOLocalMove(UpperSectionOpenPosition.localPosition, OpenCloseDuration);

            yield return LeftSection.DOLocalMove(LeftSectionOpenPosition.localPosition, OpenCloseDuration)
                                    .WaitForCompletion();

            MonstersSelector.Show();
            StorageTip.Show(canOpenStorage);

            SwitchSelectedMenu(MonstersSelector);

            if (isSwapDialog) SwappingTip.DOLocalMove(TipsOpenPosition.localPosition, .25f).SetEase(Ease.OutBack);

            if (storageWasOpenedDirectly)
                OpenStorage(true, FinishOpening);
            else
                FinishOpening();
        }

        /// <summary>
        /// Finish opening the menu.
        /// </summary>
        private void FinishOpening()
        {
            inputManager.BlockInput(false);

            inputManager.RequestInput(this);

            DialogManager.ShowLoadingIcon(false);

            currentSelectedMenu.ReselectAfterFrames();

            opening = false;
        }

        /// <summary>
        /// Close the menu.
        /// </summary>
        [Button]
        [FoldoutGroup("Debug")]
        public void CloseMenu()
        {
            if (!OpeningOrClosing) StartCoroutine(CloseMenuRoutine());
        }

        /// <summary>
        /// Close the menu.
        /// </summary>
        public IEnumerator CloseMenuRoutine(bool chooseMonster = false, bool byPassAllowClosing = false)
        {
            if (!Shown || (!shouldAllowClosing && !byPassAllowClosing))
            {
                if (currentSelectedMenu != null) currentSelectedMenu.ReselectAfterFrames();
                yield break;
            }

            closing = true;
            inputManager.BlockInput();

            MonsterInstance chosenMonster = null;
            bool comesFromRoster = false;

            if (chooseMonster)
            {
                chosenMonster = GetCurrentMonster(currentSelectedIndex);
                comesFromRoster = currentSelectedMenu == MonstersSelector;
            }

            if (isStorageOpen)
            {
                bool storageClosed = false;
                OpenStorage(false, () => storageClosed = true);
                yield return new WaitUntil(() => storageClosed);
            }

            MonsterSpriteHider.Show(false);

            StorageSelector.ClearButtons();

            StorageTip.Show(false);
            MonstersSelector.Show(false);

            UpperSection.DOLocalMove(UpperSectionClosedPosition.localPosition, OpenCloseDuration);
            LeftSection.DOLocalMove(LeftSectionClosedPosition.localPosition, OpenCloseDuration);

            yield return Background.DOFade(0, OpenCloseDuration).WaitForCompletion();

            inputManager.BlockInput(false);
            inputManager.ReleaseInput(this);
            Show(false);

            currentSelectedIndex = 0;
            currentSelectedMenu = null;

            closing = false;

            OnMenuClosed?.Invoke(chooseMonster, chosenMonster, comesFromRoster);
        }

        /// <summary>
        /// Sets the monsters of the menu.
        /// </summary>
        /// <param name="newMonsters">List of monsters to set.</param>
        private void SetMonsters(Roster newMonsters)
        {
            currentRoster = newMonsters;

            monsters = new List<MonsterInstance>();

            foreach (MonsterInstance monster in newMonsters)
                if (monster is {IsNullEntry: false})
                    monsters.Add(monster);

            for (int i = 0; i < MonstersSelector.MenuOptions.Count; i++)
            {
                MenuItem menuItem = MonstersSelector.MenuOptions[i];

                if (i >= monsters.Count)
                    menuItem.Hide();
                else
                {
                    MonsterButtonWithCompatibilityPanel button = (MonsterButtonWithCompatibilityPanel) menuItem;
                    button.Panel.SetMonster(monsters[i], false);

                    button.SetCompatibility(!isSelectingMonster
                                         || monsterCompatibilityChecker.IsMonsterCompatible(monsters[i], settings));

                    if (isSelectingMonster)
                        button.SetCompatibilityText(monsterCompatibilityChecker
                                                       .GetNotCompatibleLocalizationKey(monsters[i]));

                    menuItem.Show();
                }
            }
        }

        /// <summary>
        /// Called when a monster is hovered.
        /// </summary>
        /// <param name="index">Index of the monster hovered.</param>
        private void OnMonsterHovered(int index)
        {
            if (currentSelectedMenu == null) return;
            if (closing) return;

            MonsterInstance monster = GetCurrentMonster(index);

            UIMonsterSprite.SetMonster(monster);
            MonsterSpriteHider.Show();

            State.SetValue(monster.EggData.IsEgg ? "Monsters/Egg" :
                           monster.CurrentHP == 0 ? "Status/Fainted" :
                           monster.GetStatus() != null ? monster.GetStatus().LocalizableName : "Common/Healthy");

            (MonsterType firstType, MonsterType secondType) = monster.GetTypes(settings);

            FirstType.SetType(firstType);
            SecondType.SetType(secondType);

            MiniMonsterSummary.SetMonster(monster);
        }

        /// <summary>
        /// Get the currently hovered monster.
        /// </summary>
        /// <param name="index">Index of the monster.</param>
        /// <returns>That monster.</returns>
        private MonsterInstance GetCurrentMonster(int index) =>
            currentSelectedMenu == MonstersSelector ? monsters[index] : StorageSelector.Data[index];

        /// <summary>
        /// Is the currently hovered monster compatible?
        /// </summary>
        /// <param name="index">Index of the monster.</param>
        /// <returns>True if compatible.</returns>
        private bool IsCurrentMonsterCompatible(int index) =>
            monsterCompatibilityChecker.IsMonsterCompatible(GetCurrentMonster(index), settings);

        /// <summary>
        /// Called when a monster is selected.
        /// </summary>
        /// <param name="index">Index of the selected monster.</param>
        private void OnMonsterSelected(int index)
        {
            currentSelectedIndex = index;

            if (DownloadingAndSwapping)
            {
                Logger.Info("Downloading and swapping monsters " + index + " and " + indexToSwap + ".");

                storage.ExchangeMonsterWithRoster(currentRoster, index, indexToSwap);

                indexToSwap = -1;
                DownloadingAndSwapping = false;

                SetMonsters(currentRoster);
                UpdateStorage(() => currentSelectedMenu.ReselectAfterFrames());
            }
            else if (Swapping)
            {
                Logger.Info("Swapping monsters " + index + " and " + indexToSwap + ".");

                currentRoster.Exchange(index, indexToSwap);
                indexToSwap = -1;
                Swapping = false;

                SetMonsters(currentRoster);

                currentSelectedMenu.ReselectAfterFrames();
            }
            else
            {
                List<string> options = BuildSubmenuOptions(index, out Vector3 newContextMenuPosition);

                ContextMenuPosition.position = newContextMenuPosition;

                DialogManager.ShowChoiceMenu(options,
                                             position: ContextMenuPosition,
                                             callback: OnMonsterSubmenuChosen,
                                             onBackCallback: () =>
                                                             {
                                                                 audioManager.PlayAudio(OnSummaryBackAudio);
                                                                 currentSelectedMenu.ReselectAfterFrames();
                                                             });
            }
        }

        /// <summary>
        /// Build the options of the submenu when a monster is selected.
        /// </summary>
        /// <param name="index">Index of the selected monster.</param>
        /// <param name="newContextMenuPosition">Position in which to place the submenu.</param>
        /// <returns>Option list.</returns>
        private List<string> BuildSubmenuOptions(int index, out Vector3 newContextMenuPosition)
        {
            List<string> options = new();

            MonsterButtonWithCompatibilityPanel button =
                (MonsterButtonWithCompatibilityPanel) currentSelectedMenu.CurrentSelectedButton;

            if (isSelectingMonster && IsCurrentMonsterCompatible(index))
                options.Add("Common/Select");
            else if (isStorageOpen && !isSelectingMonster)
                options.Add(currentSelectedMenu == StorageSelector
                                ? "Menu/Pokemon/Cloud/Download"
                                : "Menu/Pokemon/Cloud/Upload");

            options.Add("Monsters/Summary");

            newContextMenuPosition = button.ContextMenuPosition.position;

            if (!isSelectingMonster)
            {
                if (currentSelectedMenu == MonstersSelector) options.Add("Monsters/Swap");

                options.Add("Monsters/HeldItem");

                if (currentSelectedMenu == MonstersSelector)
                {
                    if (!isStorageOpen)
                    {
                        List<Move> moves = currentRoster[index].GetOutOfBattleMoves();

                        options.AddRange(moves.Select(move => move.LocalizableName));

                        // Only for the first two, lower the menu the more options there are.
                        if (index <= 1) newContextMenuPosition.y -= moves.Count * 41;
                    }
                    // If it's the first and the storage is open, lower it a little.
                    else if (index == 0) newContextMenuPosition.y -= 30;
                }
                else if (currentSelectedMenu == StorageSelector)
                {
                    if (index == 0)
                        // Lower it if its the top.
                        newContextMenuPosition.y -= 50;
                    else if (index == StorageSelector.RowCount - 1
                          && StorageSelector.RowCount > 6)
                        // If it's on the bottom of the storage, get it a little up so it doesn't get out of the screen.
                        newContextMenuPosition.y += 60;
                }
            }

            options.Add("Common/Cancel");

            return options;
        }

        /// <summary>
        /// Called when an option on the monster submenu is chosen.
        /// </summary>
        /// <param name="index">Index of the chosen option.</param>
        private void OnMonsterSubmenuChosen(int index) => StartCoroutine(OnMonsterSubmenuChosenRoutine(index));

        /// <summary>
        /// Called when an option on the monster submenu is chosen.
        /// We need to keep the dialog on top of other menus so routine time.
        /// </summary>
        /// <param name="index">Index of the chosen option.</param>
        private IEnumerator OnMonsterSubmenuChosenRoutine(int index)
        {
            if (currentSelectedMenu == null) yield break;

            MonsterInstance monster = GetCurrentMonster(currentSelectedMenu.CurrentSelection);
            List<Move> moves = monster.GetOutOfBattleMoves();

            if (isSelectingMonster)
                yield return MonsterSubmenuWhenSelectingMonster(index);
            else if (!isStorageOpen)
                yield return MonsterSubmenuWhenStorageClosed(index, monster, moves);
            else
                yield return MonsterSubmenuWhenStorageOpen(index);
        }

        /// <summary>
        /// Process the monster submenu when selecting a monster.
        /// </summary>
        /// <param name="index">Index of the select monster.</param>
        private IEnumerator MonsterSubmenuWhenSelectingMonster(int index)
        {
            switch (index)
            {
                case 0 when IsCurrentMonsterCompatible(currentSelectedIndex):
                    yield return CloseMenuRoutine(true, true);
                    break;
                case 0:
                case 1 when IsCurrentMonsterCompatible(currentSelectedIndex):
                    ShowMonstersSummary(currentSelectedMenu == StorageSelector);
                    break;
                default:
                    currentSelectedMenu.ReselectAfterFrames();
                    break;
            }
        }

        /// <summary>
        /// Process the monster submenu when the storage is closed.
        /// </summary>
        /// <param name="index">Index chosen.</param>
        /// <param name="monster">Monster chosen.</param>
        /// <param name="moves">Moves this monster has.</param>
        private IEnumerator MonsterSubmenuWhenStorageClosed(int index, MonsterInstance monster, List<Move> moves)
        {
            switch (index)
            {
                case 0:
                    ShowMonstersSummary(false);
                    break;
                case 1:
                    StartMonsterSwap();
                    break;
                case 2:
                    yield return ShowHeldItemSubmenu();
                    break;
                case 3 when moves.Count > 0:
                    yield return UseMove(monster, moves, 0);
                    break;
                case 4 when moves.Count > 1:
                    yield return UseMove(monster, moves, 1);
                    break;
                case 5 when !isStorageOpen && moves.Count > 2:
                    yield return UseMove(monster, moves, 2);
                    break;
                case 6 when !isStorageOpen && moves.Count > 3:
                    yield return UseMove(monster, moves, 3);
                    break;
                default:
                    currentSelectedMenu.ReselectAfterFrames();
                    break;
            }
        }

        /// <summary>
        /// Called when an option in the context menu is chosen on normal operation of the menu, no selecting for a dialog or exchanging.
        /// </summary>
        /// <param name="index">Index of the chosen option.</param>
        private IEnumerator MonsterSubmenuWhenStorageOpen(int index)
        {
            switch (index)
            {
                case 0 when currentSelectedMenu == MonstersSelector:
                    yield return TryUploadMonster();
                    break;
                case 0 when currentSelectedMenu == StorageSelector:
                    yield return TryDownloadMonster();
                    break;
                case 1:
                    ShowMonstersSummary(currentSelectedMenu == StorageSelector);
                    break;
                case 2 when currentSelectedMenu == MonstersSelector:
                    StartMonsterSwap();
                    break;
                case 2 when currentSelectedMenu == StorageSelector:
                case 3 when currentSelectedMenu == MonstersSelector:
                    yield return ShowHeldItemSubmenu();
                    break;
            }
        }

        /// <summary>
        /// Show the currently selected monsters summary.
        /// </summary>
        /// <param name="storageSummary">Show the storage summary instead?</param>
        private void ShowMonstersSummary(bool storageSummary) =>
            DialogManager.ShowMonsterSummary(storageSummary ? StorageSelector.Data : monsters,
                                             currentSelectedIndex,
                                             null,
                                             playerCharacter,
                                             lastIndex =>
                                                 DOVirtual.DelayedCall(.1f,
                                                                       () =>
                                                                       {
                                                                           if (storageSummary)
                                                                               StorageSelector
                                                                                  .Select(lastIndex, false);
                                                                           else
                                                                               MonstersSelector
                                                                                  .Select(lastIndex, false);
                                                                       }));

        /// <summary>
        /// Start swapping the currently selected monster.
        /// </summary>
        private void StartMonsterSwap()
        {
            indexToSwap = currentSelectedIndex;
            Swapping = true;
            currentSelectedMenu.ReselectAfterFrames();
        }

        /// <summary>
        /// Use a move outside of battle.
        /// </summary>
        /// <param name="monster">Monster using the move.</param>
        /// <param name="moves">Moves this monster has.</param>
        /// <param name="moveIndex">Index of the move to use.</param>
        private IEnumerator UseMove(MonsterInstance monster, IReadOnlyList<Move> moves, int moveIndex)
        {
            yield return WaitAFrame;

            yield return moves[moveIndex].UseOutOfBattle(playerCharacter, monster, localizer, mapSceneLauncher);

            if (currentSelectedMenu != null) currentSelectedMenu.ReselectAfterFrames();
        }

        /// <summary>
        /// Try to upload the selected monster to the storage.
        /// </summary>
        private IEnumerator TryUploadMonster()
        {
            if (currentRoster.RosterSize == 1)
                yield return DialogManager.ShowDialogAndWait("Menu/Pokemon/Cloud/CantUploadLast");
            else
            {
                storage.TransferMonsterFromRoster(currentRoster, currentSelectedIndex);

                SetMonsters(currentRoster);

                if (MonstersSelector.CurrentSelection >= currentRoster.RosterSize)
                {
                    currentSelectedIndex = currentRoster.RosterSize - 1;
                    MonstersSelector.Select(currentSelectedIndex);
                }

                yield return UpdateStorageRoutine();
            }

            currentSelectedMenu.ReselectAfterFrames();
        }

        /// <summary>
        /// Try to download the selected monster from the storage.
        /// </summary>
        private IEnumerator TryDownloadMonster()
        {
            if (currentRoster.RosterSize < 6)
            {
                DialogManager.ShowLoadingIcon();

                int newIndex = storage.TransferMonsterToRoster(currentRoster,
                                                               storage.GetMonsters()
                                                                      .IndexOf(StorageSelector.Data
                                                                                   [currentSelectedIndex]));

                yield return UpdateStorageRoutine();

                SetMonsters(currentRoster);

                yield return WaitAFrame;
                yield return WaitAFrame;

                SwitchSelectedMenu(MonstersSelector);

                MonstersSelector.Select(newIndex);

                DialogManager.ShowLoadingIcon(false);
            }
            else
            {
                indexToSwap = storage.GetMonsters().IndexOf(StorageSelector.Data[currentSelectedIndex]);

                DownloadingAndSwapping = true;
            }

            currentSelectedMenu.ReselectAfterFrames();
        }

        /// <summary>
        /// Show the held item submenu.
        /// </summary>
        private IEnumerator ShowHeldItemSubmenu()
        {
            yield return WaitAFrame;
            yield return WaitAFrame;

            List<string> choices = new() {"Monsters/HeldItem/Give"};

            if (GetCurrentMonster(currentSelectedIndex).HeldItem != null) choices.Add("Monsters/HeldItem/PutBack");

            choices.Add("Common/Cancel");

            DialogManager.ShowChoiceMenu(choices,
                                         position: ((MonsterButton) currentSelectedMenu.CurrentSelectedButton)
                                        .ContextMenuPosition,
                                         callback: OnHeldItemSubmenuChosen,
                                         onBackCallback: () =>
                                                         {
                                                             audioManager.PlayAudio(OnSummaryBackAudio);
                                                             currentSelectedMenu.ReselectAfterFrames();
                                                         });
        }

        /// <summary>
        /// Called when an option on the held item submenu is chosen.
        /// </summary>
        /// <param name="index">Option chosen.</param>
        private void OnHeldItemSubmenuChosen(int index) => StartCoroutine(OnHeldItemSubmenuChosenRoutine(index));

        /// <summary>
        /// Called when an option on the held item submenu is chosen.
        /// </summary>
        /// <param name="index">Option chosen.</param>
        private IEnumerator OnHeldItemSubmenuChosenRoutine(int index)
        {
            switch (index)
            {
                case 0: // Open the bag to give.
                    yield return WaitAFrame;

                    DialogManager.ShowBag((_, _) =>
                                          {
                                              SetMonsters(currentRoster);
                                              StorageSelector.Refresh();
                                              currentSelectedMenu.ReselectAfterFrames();
                                          },
                                          rosterOverride: new[] {GetCurrentMonster(currentSelectedIndex)},
                                          playerCharacter: playerCharacter);

                    break;
                case 1 when GetCurrentMonster(currentSelectedIndex).HeldItem != null: // Put back in the bag.
                    yield return WaitAFrame;

                    DialogManager.ShowDialog("Item/PutBack",
                                             localizableModifiers: false,
                                             modifiers: GetCurrentMonster(currentSelectedIndex)
                                                       .HeldItem
                                                       .GetName(localizer));

                    playerBag.ChangeItemAmount(GetCurrentMonster(currentSelectedIndex).HeldItem, 1);

                    GetCurrentMonster(currentSelectedIndex).HeldItem = null;

                    SetMonsters(currentRoster);
                    StorageSelector.Refresh();

                    currentSelectedMenu.ReselectAfterFrames();

                    break;
                default:
                    currentSelectedMenu.ReselectAfterFrames();
                    break;
            }
        }

        /// <summary>
        /// Switch the selected menu.
        /// </summary>
        private void SwitchSelectedMenu(MenuSelector menu)
        {
            if (menu == currentSelectedMenu) return;

            audioManager.PlayAudio(NavigationAudio);

            if (currentSelectedMenu != null) currentSelectedMenu.OnStateExit();

            currentSelectedMenu = menu;

            currentSelectedMenu.OnStateEnter();
        }

        /// <summary>
        /// Called when the back button has been pressed.
        /// </summary>
        private void OnBackSelected()
        {
            if (DownloadingAndSwapping)
            {
                indexToSwap = -1;
                DownloadingAndSwapping = false;

                currentSelectedMenu.ReselectAfterFrames();
            }
            else if (Swapping)
            {
                indexToSwap = -1;
                Swapping = false;

                currentSelectedMenu.ReselectAfterFrames();
            }
            else if (isStorageOpen && !storageWasOpenedDirectly)
                OpenStorage(false);
            else
                CloseMenu();
        }

        /// <summary>
        /// Open or close the storage.
        /// </summary>
        public void OnExtra1(InputAction.CallbackContext context)
        {
            if (!canOpenStorage || Swapping || DownloadingAndSwapping || context.phase != InputActionPhase.Started)
                return;

            if (!isStorageOpen)
                OpenStorage(true);
            else if (!SortingMenu.IsOpen)
            {
                SortingMenu.Open();
                SortingTip.Show(false);
            }
        }

        /// <summary>
        /// Open or close the storage.
        /// </summary>
        /// <param name="open">Open?</param>
        /// <param name="callback">Callback called when finished.</param>
        private void OpenStorage(bool open, Action callback = null)
        {
            if (isStorageOpen == open)
            {
                callback?.Invoke();
                return;
            }

            audioManager.PlayAudio(StorageOpenCloseAudio);

            if (open)
                StorageSection.DOLocalMove(StorageSectionOpenPosition.localPosition, OpenCloseDuration)
                              .SetEase(Ease.OutBack)
                              .OnComplete(() => callback?.Invoke());
            else
            {
                if (currentSelectedMenu == StorageSelector) SwitchSelectedMenu(MonstersSelector);

                StorageSection.DOLocalMove(StorageSectionClosedPosition.localPosition, OpenCloseDuration)
                              .SetEase(Ease.InBack)
                              .OnComplete(() => callback?.Invoke());
            }

            isStorageOpen = open;

            SortingTip.Show(isStorageOpen);

            currentSelectedMenu.ReselectAfterFrames();
        }

        /// <summary>
        /// Called when the sorting menu is closed.
        /// </summary>
        private void OnSortingMenuClosed()
        {
            Logger.Info("Applying filters.");

            UpdateStorage(() => SortingTip.Show());
        }

        /// <summary>
        /// Update the storage menu.
        /// </summary>
        private void UpdateStorage(Action finished) => StartCoroutine(UpdateStorageRoutine(finished));

        /// <summary>
        /// Update the storage menu.
        /// </summary>
        private IEnumerator UpdateStorageRoutine(Action finished = null)
        {
            inputManager.BlockInput();
            DialogManager.ShowLoadingIcon();

            yield return WaitAFrame;
            yield return WaitAFrame;

            List<MonsterInstance> sortedMonsters = SortingMenu.FilterStorage(storage.GetMonsters());

            yield return WaitAFrame;

            StorageSelector.IsChoosingDialog = isSelectingMonster;
            StorageSelector.CompatibilityChecker = monsterCompatibilityChecker;
            StorageSelector.SetButtons(sortedMonsters);

            inputManager.BlockInput(false);
            DialogManager.ShowLoadingIcon(false);

            yield return WaitAFrame;

            if (sortedMonsters.Count == 0)
                SwitchSelectedMenu(MonstersSelector);
            else if (StorageSelector.CurrentSelection >= sortedMonsters.Count) StorageSelector.Select(0);

            if (currentSelectedMenu != null) currentSelectedMenu.ReselectAfterFrames();

            finished?.Invoke();
        }

        /// <summary>
        /// Pass the input to the selected menu.
        /// </summary>
        public void OnNavigation(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started && !Swapping && !DownloadingAndSwapping)
            {
                if (context.ReadValue<Vector2>().x < 0 && currentSelectedMenu == StorageSelector)
                    SwitchSelectedMenu(MonstersSelector);
                else if (canOpenStorage
                      && isStorageOpen
                      && context.ReadValue<Vector2>().x > 0
                      && currentSelectedMenu == MonstersSelector
                      && storage.Count > 0)
                    SwitchSelectedMenu(StorageSelector);
            }

            currentSelectedMenu.OnNavigation(context);
        }

        /// <summary>
        /// Pass the input to the selected menu.
        /// </summary>
        public void OnSelect(InputAction.CallbackContext context) => currentSelectedMenu.OnSelect(context);

        /// <summary>
        /// Pass the input to the selected menu.
        /// </summary>
        public void OnBack(InputAction.CallbackContext context) => currentSelectedMenu.OnBack(context);

        /// <summary>
        /// Reselect the current menu when the input state is entered.
        /// </summary>
        public void OnStateEnter()
        {
            if (currentSelectedMenu != null && !closing) currentSelectedMenu.ReselectAfterFrames();
        }

        /// <summary>
        /// This receiver is of type UI.
        /// </summary>
        /// <returns></returns>
        public InputType GetInputType() => InputType.UI;

        /// <summary>
        /// Name used for input debugging.
        /// </summary>
        /// <returns></returns>
        public string GetDebugName() => name;

        #region Unused input callbacks

        public void OnStateExit()
        {
        }

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