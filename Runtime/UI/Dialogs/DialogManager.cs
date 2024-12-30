using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using Proxima;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph.CommandUtils;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.Monster.Evolution;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.UI.Bags;
using Varguiniano.YAPU.Runtime.UI.Dex;
using Varguiniano.YAPU.Runtime.UI.Dialogs.ChoiceMenu;
using Varguiniano.YAPU.Runtime.UI.Dialogs.MoveReplaceDialog;
using Varguiniano.YAPU.Runtime.UI.Dialogs.MoveTutor;
using Varguiniano.YAPU.Runtime.UI.Dialogs.NewMonsterPopup;
using Varguiniano.YAPU.Runtime.UI.Dialogs.Notifications;
using Varguiniano.YAPU.Runtime.UI.Dialogs.TextInput;
using Varguiniano.YAPU.Runtime.UI.Dialogs.XPGainPopup;
using Varguiniano.YAPU.Runtime.UI.GameMenu;
using Varguiniano.YAPU.Runtime.UI.Map;
using Varguiniano.YAPU.Runtime.UI.Monsters.MonstersMenu;
using Varguiniano.YAPU.Runtime.UI.Monsters.Summary;
using Varguiniano.YAPU.Runtime.UI.Options;
using Varguiniano.YAPU.Runtime.UI.Profile;
using Varguiniano.YAPU.Runtime.UI.Quests;
using Varguiniano.YAPU.Runtime.UI.Shops;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Core.Runtime.DependencyInjection;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.Localization.Runtime.Ui;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Dialogs
{
    /// <summary>
    /// Manager in charge of displaying dialogs.
    /// </summary>
    public class DialogManager : Singleton<DialogManager>, IInputReceiver, IPlayerDataReceiver
    {
        /// <summary>
        /// Is the dialog manager accepting input?
        /// </summary>
        public static bool AcceptInput
        {
            get => Instance.acceptInputInternal;
            set
            {
                Instance.acceptInputInternal = value;

                Instance.NextArrow.SetActive(Instance.acceptInputInternal);
            }
        }

        /// <summary>
        /// Backfield for AcceptInput
        /// </summary>
        private bool acceptInputInternal;

        /// <summary>
        /// Audio to play when changing to the next dialog.
        /// </summary>
        [FoldoutGroup("Basic Dialog")]
        [SerializeField]
        private AudioReference NextDialogAudio;

        /// <summary>
        /// Reference to the text in which the dialogs are showed.
        /// </summary>
        [FoldoutGroup("Basic Dialog")]
        [SerializeField]
        private LocalizedTypeWriterTMP Text;

        /// <summary>
        /// Reference to the arrow to show the dialog can be advanced.
        /// </summary>
        [FoldoutGroup("Basic Dialog")]
        [SerializeField]
        private GameObject NextArrow;

        /// <summary>
        /// Reference to the dialog canvas.
        /// </summary>
        [FoldoutGroup("Basic Dialog")]
        [SerializeField]
        private HidableUiElement BasicDialog;

        /// <summary>
        /// Reference to the basic dialog normal background.
        /// </summary>
        [FoldoutGroup("Basic Dialog")]
        [SerializeField]
        private SerializableDictionary<BasicDialogBackground, HidableUiElement> Backgrounds;

        /// <summary>
        /// Reference to the character panel.
        /// </summary>
        [FoldoutGroup("Basic Dialog")]
        [SerializeField]
        private HidableUiElement CharacterPanel;

        /// <summary>
        /// Reference to the character name.
        /// </summary>
        [FoldoutGroup("Basic Dialog")]
        [SerializeField]
        private LocalizedTextMeshPro CharacterName;

        /// <summary>
        /// Reference to the loading icon.
        /// </summary>
        [FoldoutGroup("Loading")]
        [SerializeField]
        private HidableUiElement LoadingIcon;

        /// <summary>
        /// Reference to the loading text.
        /// </summary>
        [FoldoutGroup("Loading")]
        [SerializeField]
        private LocalizedTextMeshPro LoadingText;

        /// <summary>
        /// Reference to the notifications manager.
        /// </summary>
        [FoldoutGroup("Complex Dialogs")]
        [SerializeField]
        private NotificationManager NotificationManager;

        /// <summary>
        /// Reference to the game menu.
        /// </summary>
        [FoldoutGroup("Complex Dialogs")]
        [SerializeField]
        private GameMenuScreen GameMenu;

        /// <summary>
        /// Reference to the xp gain popup.
        /// </summary>
        [FoldoutGroup("Complex Dialogs")]
        [SerializeField]
        private RosterXPPanel RosterXPPanel;

        /// <summary>
        /// Reference to the stats up popup.
        /// </summary>
        [FoldoutGroup("Complex Dialogs")]
        [SerializeField]
        private StatsUpPanel StatsUpPanel;

        /// <summary>
        /// Reference to the move replace dialog.
        /// </summary>
        [FoldoutGroup("Complex Dialogs")]
        [SerializeField]
        private MoveReplaceDialogController MoveReplacePopup;

        /// <summary>
        /// Reference to the choice menu.
        /// </summary>
        [FoldoutGroup("Complex Dialogs")]
        [SerializeField]
        private ChoiceMenuController ChoiceMenu;

        /// <summary>
        /// Reference to the monster summary.
        /// </summary>
        [FoldoutGroup("Complex Dialogs")]
        [SerializeField]
        private MonsterSummary MonsterSummary;

        /// <summary>
        /// Reference to the Dex Screen.
        /// </summary>
        [FoldoutGroup("Complex Dialogs")]
        [SerializeField]
        private DexScreen DexScreen;

        /// <summary>
        /// Reference to the Dex Screen for a single monster.
        /// </summary>
        [FoldoutGroup("Complex Dialogs")]
        [SerializeField]
        private SingleMonsterDexScreen SingleMonsterDexScreen;

        /// <summary>
        /// Reference to the bag screen.
        /// </summary>
        [FoldoutGroup("Complex Dialogs")]
        [SerializeField]
        private BagScreen BagScreen;

        /// <summary>
        /// Is the bag screen open?
        /// </summary>
        public static bool IsBagScreenOpen => Instance.BagScreen.Shown;

        /// <summary>
        /// Reference to the menu for the quick access items.
        /// </summary>
        [FoldoutGroup("Complex Dialogs")]
        [SerializeField]
        private QuickAccessItemsMenu QuickAccessItemsMenu;

        /// <summary>
        /// Reference to the shop dialog.
        /// </summary>
        [FoldoutGroup("Complex Dialogs")]
        [SerializeField]
        private ShopDialog ShopDialog;

        /// <summary>
        /// Reference to the monsters menu.
        /// </summary>
        [FoldoutGroup("Complex Dialogs")]
        [SerializeField]
        private MonstersMenuScreen MonstersMenu;

        /// <summary>
        /// Reference to the quests screen.
        /// </summary>
        [FoldoutGroup("Complex Dialogs")]
        [SerializeField]
        private QuestsScreen QuestsScreen;

        /// <summary>
        /// Reference to the options menu.
        /// </summary>
        [FoldoutGroup("Complex Dialogs")]
        [SerializeField]
        private OptionsScreen OptionsMenu;

        /// <summary>
        /// Reference to the profile screen.
        /// </summary>
        [FoldoutGroup("Complex Dialogs")]
        [SerializeField]
        private ProfileScreen ProfileScreen;

        /// <summary>
        /// Reference to the new monster popup.
        /// </summary>
        [FoldoutGroup("Complex Dialogs")]
        [SerializeField]
        private NewMonsterPopupDialog NewMonsterPopup;

        /// <summary>
        /// Reference to the text input dialog.
        /// </summary>
        [FoldoutGroup("Complex Dialogs")]
        [SerializeField]
        private TextInputDialog TextInputDialog;

        /// <summary>
        /// Reference to the move tutor dialog.
        /// </summary>
        [FoldoutGroup("Complex Dialogs")]
        [SerializeField]
        private MoveTutorDialog MoveTutorDialog;

        /// <summary>
        /// Event raised when it switches to the next dialog.
        /// </summary>
        public static Action OnNextDialog;

        /// <summary>
        /// Flag to know if there are dialogs pending.
        /// </summary>
        private bool NoDialogsPending => !active;

        /// <summary>
        /// Wait until object for waiting for the dialog to end.
        /// </summary>
        public static WaitUntil WaitForDialog => new(() => Instance.NoDialogsPending);

        /// <summary>
        /// Wait while object for waiting until the typewriter stops.
        /// </summary>
        public static WaitWhile WaitForTypewriter => new(IsTypewritingDialog);

        /// <summary>
        /// Reference to the notifications manager.
        /// </summary>
        public static NotificationManager Notifications => Instance.NotificationManager;

        /// <summary>
        /// Flag to know if the xp panel is being shown.
        /// </summary>
        public static bool XPPanelShown => Instance.RosterXPPanel.Shown;

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
        /// Reference to the player roster.
        /// </summary>
        [Inject]
        private Roster playerRoster;

        /// <summary>
        /// Reference to the evolution manager.
        /// </summary>
        [Inject]
        private EvolutionManager evolutionManager;

        /// <summary>
        /// Reference to the map scene launcher.
        /// </summary>
        [Inject]
        private MapSceneLauncher mapSceneLauncher;

        /// <summary>
        /// Flag to know if it´s currently active.
        /// </summary>
        private bool active;

        /// <summary>
        /// Routine that waits for some seconds and then displays the next dialog.
        /// </summary>
        private Coroutine nextDialogAfterSecondsRoutine;

        /// <summary>
        /// Queue of all dialog objects that are pending to be displayed in the dialog.
        /// </summary>
        private readonly Queue<DialogText> pendingDialogKeys = new();

        /// <summary>
        /// Register the Proxima commands.
        /// </summary>
        private void Awake() => ProximaInspector.RegisterCommands<DialogManager>();

        /// <summary>
        /// Display a dialog on screen.
        /// </summary>
        /// <param name="localizationKey">Localization key for the text.</param>
        [ProximaCommand("Dialogs",
                        description:
                        "Shows a dialog to the player, the string parameter must be the localized key of that dialog.")]
        public static void ShowDialog(string localizationKey) => ShowDialog(localizationKey, null);

        /// <summary>
        /// Display a dialog on screen.
        /// </summary>
        /// <param name="localizationKey">Localization key for the text.</param>
        /// <param name="character">Character saying the dialog.</param>
        /// <param name="monster">Monster saying the dialog.</param>
        /// <param name="acceptInput">Should the dialog accept input? Overriden if it waits for seconds.</param>
        /// <param name="typewriterSpeed">Speed to play the typewriter at.</param>
        /// <param name="switchToNextAfterSeconds">Seconds to wait before switching to the next dialog. -1 means wait for input.</param>
        /// <param name="localizableModifiers">Are the modifiers localizable?</param>
        /// <param name="background">Background to use.</param>
        /// <param name="horizontalAlignment">Horizontal alignment for the text.</param>
        /// <param name="modifiers">Modifiers to apply to the text.</param>
        [FoldoutGroup("Debug")]
        [Button]
        public static void ShowDialog(string localizationKey,
                                      CharacterData character = null,
                                      MonsterInstance monster = null,
                                      bool acceptInput = true,
                                      float typewriterSpeed = .01f,
                                      float switchToNextAfterSeconds = -1,
                                      bool localizableModifiers = true,
                                      BasicDialogBackground background = BasicDialogBackground.Normal,
                                      HorizontalAlignmentOptions horizontalAlignment = HorizontalAlignmentOptions.Left,
                                      params string[] modifiers) =>
            Instance.ShowDialog(new DialogText(localizationKey,
                                               character,
                                               monster,
                                               typewriterSpeed,
                                               switchToNextAfterSeconds,
                                               localizableModifiers,
                                               background,
                                               horizontalAlignment,
                                               modifiers),
                                switchToNextAfterSeconds < 0 && acceptInput);

        /// <summary>
        /// Display a dialog on screen and wait for it.
        /// </summary>
        /// <param name="localizationKey">Localization key for the text.</param>
        /// <param name="character">Character saying the dialog.</param>
        /// <param name="monster">Monster saying the dialog.</param>
        /// <param name="typewriterSpeed">Speed to play the typewriter at.</param>
        /// <param name="switchToNextAfterSeconds">Seconds to wait before switching to the next dialog. -1 means wait for input.</param>
        /// <param name="localizableModifiers">Are the modifiers localizable?</param>
        /// <param name="background">Background to use.</param>
        /// <param name="horizontalAlignment">Horizontal alignment for the text.</param>
        /// <param name="modifiers">Modifiers to apply to the text.</param>
        public static IEnumerator ShowDialogAndWait(string localizationKey,
                                                    CharacterData character = null,
                                                    MonsterInstance monster = null,
                                                    float typewriterSpeed = .01f,
                                                    float switchToNextAfterSeconds = -1,
                                                    bool localizableModifiers = true,
                                                    BasicDialogBackground background = BasicDialogBackground.Normal,
                                                    HorizontalAlignmentOptions horizontalAlignment =
                                                        HorizontalAlignmentOptions.Left,
                                                    params string[] modifiers)
        {
            ShowDialog(localizationKey,
                       character,
                       monster,
                       switchToNextAfterSeconds < 0,
                       typewriterSpeed,
                       switchToNextAfterSeconds,
                       localizableModifiers,
                       background,
                       horizontalAlignment,
                       modifiers);

            yield return WaitForDialog;
        }

        /// <summary>
        /// Show the game menu.
        /// <param name="onButtons">Buttons that should be on.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="onBack">Action called when the menu is closed..</param>
        /// </summary>
        public static void ShowGameMenu(List<bool> onButtons, PlayerCharacter playerCharacter, Action onBack)
        {
            Instance.GameMenu.UpdateLayout(onButtons);

            Instance.GameMenu.ShowMenu(playerCharacter, onBack);
        }

        /// <summary>
        /// Show the monsters menu with the player roster.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="allowCloud">Allow the player to access the cloud.</param>
        /// <param name="allowClosing">Should we allow the player to close the menu?</param>
        /// <param name="openStorageDirectly">Open the storage right after opening the menu?</param>
        /// <param name="isChoosingDialog">Is this opening part of a dialog in which the player must choose a monster?</param>
        /// <param name="compatibilityChecker">Compatibility checker to check if the monster is compatible for the choosing dialog.</param>
        /// <param name="isChoosingToSwapDialog">Is opening part of a dialog to swap a monster for another?</param>
        /// <param name="onBackCallback">It passes a bool stating if the player chose a monster, the monster reference and if it came from the roster (true) or the storage (false).
        /// There params are useful when the menu is opened as a dialog to chose a monster.</param>
        public static void ShowPlayerRosterMenu(PlayerCharacter playerCharacter,
                                                bool allowCloud = true,
                                                bool allowClosing = true,
                                                bool openStorageDirectly = false,
                                                bool isChoosingDialog = false,
                                                MonsterCompatibilityChecker compatibilityChecker = null,
                                                bool isChoosingToSwapDialog = false,
                                                Action<bool, MonsterInstance, bool> onBackCallback = null)
        {
            Instance.MonstersMenu.OnMenuClosed += (chosen, monster, isInRoster) =>
                                                  {
                                                      Instance.MonstersMenu.OnMenuClosed = null;
                                                      onBackCallback?.Invoke(chosen, monster, isInRoster);
                                                  };

            Instance.MonstersMenu.OpenMenu(Instance.playerRoster,
                                           playerCharacter,
                                           allowCloud,
                                           allowClosing,
                                           openStorageDirectly,
                                           isChoosingDialog,
                                           compatibilityChecker,
                                           isChoosingToSwapDialog);
        }

        /// <summary>
        /// Show or hide the loading icon.
        /// </summary>
        /// <param name="show">Show or hide?</param>
        public static void ShowLoadingIcon(bool show = true) => Instance.LoadingIcon.Show(show);

        /// <summary>
        /// Show a loading text.
        /// </summary>
        public static void ShowLoadingText(string text,
                                           bool modifiersAreLocalizableKeys = true,
                                           params string[] valueModifiers) =>
            Instance.LoadingText.SetValue(text, modifiersAreLocalizableKeys, valueModifiers);

        /// <summary>
        /// Clear the loading text.
        /// </summary>
        public static void ClearLoadingText() => Instance.LoadingText.SetValue("");

        /// <summary>
        /// Display a dialog with a monster in the player roster gaining xp.
        /// Then trigger move learning and evolutions.
        /// </summary>
        /// <param name="monster">Monster to raise.</param>
        /// <param name="amount">Amount raised.</param>
        /// <param name="levelUpData">Level ups that were achieved.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="localizer"></param>
        /// <returns>Reference to the localizer.</returns>
        public static IEnumerator ShowXPGainDialogLearnMoveAndEvolveForSingleMonsterInPlayerRoster(
            MonsterInstance monster,
            uint amount,
            List<MonsterInstance.LevelUpData> levelUpData,
            PlayerCharacter playerCharacter,
            ILocalizer localizer)
        {
            if (!Instance.playerRoster.RosterData.Contains(monster))
            {
                Instance.Logger.Error(monster.GetNameOrNickName(localizer) + " is not in the player roster!");
                yield break;
            }

            Instance.inputManager.BlockInput();

            ShowDialog("Dialogs/XPGain/Single",
                       acceptInput: false,
                       localizableModifiers: false,
                       modifiers: new[] {monster.GetNameOrNickName(localizer), amount.ToString()});

            int index = Instance.playerRoster.RosterData.IndexOf(monster);

            List<uint> amounts = new()
                                 {
                                     0,
                                     0,
                                     0,
                                     0,
                                     0,
                                     0
                                 };

            amounts[index] = amount;

            List<List<MonsterInstance.LevelUpData>> levelUps = new()
                                                               {
                                                                   new List<MonsterInstance.LevelUpData>(),
                                                                   new List<MonsterInstance.LevelUpData>(),
                                                                   new List<MonsterInstance.LevelUpData>(),
                                                                   new List<MonsterInstance.LevelUpData>(),
                                                                   new List<MonsterInstance.LevelUpData>(),
                                                                   new List<MonsterInstance.LevelUpData>()
                                                               };

            levelUps[index] = levelUpData;

            yield return Instance.RosterXPPanel.ShowXPGain(Instance.playerRoster.RosterData.ToList(),
                                                           amounts,
                                                           levelUps);

            Instance.Logger.Info("XP dialog finished. Checking level ups for moves and evolutions.");

            AcceptInput = true;

            yield return WaitForDialog;

            foreach (MonsterInstance.LevelUpData levelUp in levelUps.SelectMany(levelUpDataPerMonster =>
                                                                                    levelUpDataPerMonster))
            {
                levelUp.Monster.ExtraData.NeedsLevelUpEvolutionCheck = true;

                foreach (Move move in levelUp.MovesToLearn)
                    yield return ShowMoveLearnPanel(levelUp.Monster,
                                                    move,
                                                    localizer,
                                                    _ =>
                                                    {
                                                    });
            }

            yield return Instance.evolutionManager.TriggerEvolutionsAfterLevelUp(Instance.playerRoster.RosterData
                   .ToList(),
                playerCharacter,
                true);

            Instance.inputManager.BlockInput(false);
        }

        /// <summary>
        /// Display a dialog with a roster gaining xp.
        /// </summary>
        /// <param name="roster">Roster to display.</param>
        /// <param name="amounts">Amounts of XP to gain.</param>
        /// <param name="levelUps">Level ups that were achieved.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        public static IEnumerator ShowXPGainDialogAndLearnMoveInBattle(List<MonsterInstance> roster,
                                                                       List<uint> amounts,
                                                                       List<List<MonsterInstance.LevelUpData>> levelUps,
                                                                       ILocalizer localizer)
        {
            ShowDialog("Dialogs/XPGain/Main", acceptInput: false);

            yield return Instance.RosterXPPanel.ShowXPGain(roster,
                                                           amounts,
                                                           levelUps);

            AcceptInput = true;

            foreach (MonsterInstance.LevelUpData levelUp in levelUps.SelectMany(levelUpDataPerMonster =>
                                                                                    levelUpDataPerMonster))
            {
                levelUp.Monster.ExtraData.NeedsLevelUpEvolutionCheck = true;

                foreach (Move move in levelUp.MovesToLearn)
                    yield return ShowMoveLearnPanel(levelUp.Monster,
                                                    move,
                                                    localizer,
                                                    _ =>
                                                    {
                                                    });
            }
        }

        /// <summary>
        /// Show the panel for the stats up panel.
        /// </summary>
        /// <param name="monster">Monster for that panel.</param>
        /// <param name="previousValues">Previous stats.</param>
        /// <param name="newValues">New stats.</param>
        public static void ShowStatsUpPanel(MonsterInstance monster, uint[] previousValues, uint[] newValues)
        {
            Instance.StatsUpPanel.SetValues(previousValues, newValues);

            ShowDialog("Dialogs/XPGain/LevelUp",
                       localizableModifiers: false,
                       modifiers: monster.GetNameOrNickName(Instance.localizer));

            Instance.StatsUpPanel.Show();
        }

        /// <summary>
        /// Hide the stats up panel.
        /// </summary>
        public static void HideStatsUpPanel() => Instance.StatsUpPanel.Show(false);

        /// <summary>
        /// Show a dialog to nickname and store a new monster.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="newMonster">New monster.</param>
        /// <param name="alreadySeen">Had the player already seen this monster?</param>
        /// <param name="requestNickname">Request the player for a nickname?</param>
        public static IEnumerator ShowNewMonsterDialog(PlayerCharacter playerCharacter,
                                                       MonsterInstance newMonster,
                                                       bool alreadySeen,
                                                       bool requestNickname)
        {
            yield return Instance.NewMonsterPopup.ShowDialog(playerCharacter, newMonster, alreadySeen, requestNickname);
        }

        /// <summary>
        /// Show the move tutor dialog.
        /// </summary>
        /// <param name="monster">Monster that might learn.</param>
        /// <param name="candidates">Move candidates.</param>
        public static IEnumerator ShowMoveTutorDialog(MonsterInstance monster, List<Move> candidates)
        {
            yield return Instance.MoveTutorDialog.ShowDialog(monster, candidates);
        }

        /// <summary>
        /// Show a panel for the monster to learn a move.
        /// </summary>
        /// <param name="monster">Reference to the monster.</param>
        /// <param name="move">Move to learn.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Callback telling if the move was learnt.</param>
        public static IEnumerator ShowMoveLearnPanel(MonsterInstance monster,
                                                     Move move,
                                                     ILocalizer localizer,
                                                     Action<bool> finished)
        {
            if (monster?.IsNullEntry != false || move == null) yield break;

            monster.LearntMoves.Add(move);

            if (monster.KnowsMove(move)) yield break;

            for (int i = 0; i < monster.CurrentMoves.Length; i++)
            {
                if (monster.CurrentMoves[i].Move != null) continue;

                monster.CurrentMoves[i] = new MoveSlot(move);

                Instance.MoveReplacePopup.PlayAudio();

                yield return ShowDialogAndWait("Dialogs/Moves/Learnt",
                                               localizableModifiers: false,
                                               modifiers: new[]
                                                          {
                                                              monster.GetNameOrNickName(localizer),
                                                              localizer[move.LocalizableName]
                                                          });

                finished.Invoke(false);

                yield break;
            }

            yield return ShowDialogAndWait("Dialogs/Moves/WantsToLearn",
                                           localizableModifiers: false,
                                           modifiers: new[]
                                                      {
                                                          monster.GetNameOrNickName(localizer),
                                                          localizer[move.LocalizableName]
                                                      });

            int option = -1;

            ShowChoiceMenu(new List<string>
                           {
                               "Common/True",
                               "Common/False"
                           },
                           choice => option = choice,
                           onBackCallback: () => option = 1,
                           showDialog: true,
                           localizationKey: "Dialogs/Moves/AskIfWantsToReplace",
                           modifiers: move.LocalizableName);

            yield return new WaitUntil(() => option != -1);

            if (option == 1)
            {
                yield return ShowDialogAndWait("Dialogs/Moves/DidntLearn",
                                               localizableModifiers: false,
                                               modifiers: new[]
                                                          {
                                                              monster.GetNameOrNickName(localizer),
                                                              localizer[move.LocalizableName]
                                                          });

                finished.Invoke(false);

                yield break;
            }

            yield return ShowDialogAndWait("Dialogs/Moves/AskToReplace");

            yield return Instance.MoveReplacePopup.RequestReplaceMove(monster, move, finished);
        }

        /// <summary>
        /// Show a choice menu.
        /// And maybe a dialog with it.
        /// </summary>
        /// <param name="options">Options to show on the menu.</param>
        /// <param name="callback">Callback for those options.</param>
        /// <param name="position">Position where to place the choice menu.</param>
        /// <param name="onBackCallback">Callback called when the back button is pressed.
        /// Setting this to something different than null will make the dialog close on back pressed.</param>
        /// <param name="character">Character saying the dialog.</param>
        /// <param name="monster">Monster saying the dialog.</param>
        /// <param name="showDialog">Show a dialog along with it?</param>
        /// <param name="localizationKey">Localization key for the text.</param>
        /// <param name="localizableModifiers">Are the modifiers localizable?</param>
        /// <param name="modifiers">Modifiers to apply to the text.</param>
        public static void ShowChoiceMenu(List<string> options,
                                          Action<int> callback,
                                          Transform position = null,
                                          Action onBackCallback = null,
                                          CharacterData character = null,
                                          MonsterInstance monster = null,
                                          bool showDialog = false,
                                          string localizationKey = "",
                                          bool localizableModifiers = true,
                                          params string[] modifiers) =>
            Instance.ShowChoiceMenuNonStatic(options,
                                             callback,
                                             position,
                                             onBackCallback,
                                             character,
                                             monster,
                                             showDialog,
                                             localizationKey,
                                             localizableModifiers,
                                             modifiers);

        /// <summary>
        /// Show a choice menu.
        /// And maybe a dialog with it.
        /// </summary>
        /// <param name="options">Options to show on the menu.</param>
        /// <param name="callback">Callback for those options.</param>
        /// <param name="position">Position where to place the choice menu.</param>
        /// <param name="onBackCallback">Callback called when the back button is pressed.
        /// Setting this to something different than null will make the dialog close on back pressed.</param>
        /// <param name="character">Character saying the dialog.</param>
        /// <param name="monster">Monster saying the dialog.</param>
        /// <param name="showDialog">Show a dialog along with it?</param>
        /// <param name="localizationKey">Localization key for the text.</param>
        /// <param name="localizableModifiers">Are the modifiers localizable?</param>
        /// <param name="modifiers">Modifiers to apply to the text.</param>
        [FoldoutGroup("Debug")]
        [Button("Show Choice Menu")]
        private void ShowChoiceMenuNonStatic(List<string> options,
                                             Action<int> callback,
                                             Transform position = null,
                                             Action onBackCallback = null,
                                             CharacterData character = null,
                                             MonsterInstance monster = null,
                                             bool showDialog = false,
                                             string localizationKey = "",
                                             bool localizableModifiers = true,
                                             params string[] modifiers)
        {
            ChoiceMenu.Show(false);

            ChoiceMenu.SetPosition(position);

            ChoiceMenu.SetNumberOfOptions(options.Count);

            for (int i = 0; i < options.Count; i++)
                ((ChoiceOption) ChoiceMenu.MenuOptions[i]).Text.SetValue(options[i]);

            ChoiceMenu.OnButtonSelected += selection =>
                                           {
                                               ChoiceMenu.OnButtonSelected = null;
                                               ChoiceMenu.OnBackSelected = null;
                                               callback?.Invoke(selection);
                                               if (showDialog) NextDialog();
                                           };

            ChoiceMenu.NoGoingBack = onBackCallback == null;

            if (onBackCallback != null)
                ChoiceMenu.OnBackSelected += () =>
                                             {
                                                 ChoiceMenu.OnButtonSelected = null;
                                                 ChoiceMenu.OnBackSelected = null;
                                                 ChoiceMenu.Show(false);

                                                 onBackCallback();

                                                 if (showDialog) NextDialog();
                                             };

            if (showDialog)
            {
                // ReSharper disable once ConvertToLocalFunction
                UnityAction textShowedCallback = null;

                textShowedCallback = () =>
                                     {
                                         Text.Typewriter.onTextShowed.RemoveListener(textShowedCallback);
                                         ChoiceMenu.Show();
                                     };

                Text.Typewriter.onTextShowed.AddListener(textShowedCallback);

                ShowDialog(localizationKey,
                           character,
                           monster,
                           false,
                           localizableModifiers: localizableModifiers,
                           modifiers: modifiers);
            }
            else
                ChoiceMenu.Show();
        }

        /// <summary>
        /// Show a monster summary.
        /// </summary>
        /// <param name="roster">The roster the monster belongs to.</param>
        /// <param name="index">The index of the monster in that roster.</param>
        /// <param name="battleManager">Reference to the battle manager if we are in battle.</param>
        /// <param name="playerCharacter">Player character reference.</param>
        /// <param name="onClose">Event telling the last selected index when it was closed.</param>
        public static void ShowMonsterSummary(List<MonsterInstance> roster,
                                              int index,
                                              BattleManager battleManager,
                                              PlayerCharacter playerCharacter,
                                              Action<int> onClose) =>
            Instance.ShowMonsterSummaryNonStatic(roster, index, battleManager, playerCharacter, onClose);

        /// <summary>
        /// Show a monster summary.
        /// </summary>
        /// <param name="roster">The roster the monster belongs to.</param>
        /// <param name="index">The index of the monster in that roster.</param>
        /// <param name="battleManager">Reference to the battle manager if we are in battle.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="onClose">Event telling the last selected index when it was closed.</param>
        private void ShowMonsterSummaryNonStatic(List<MonsterInstance> roster,
                                                 int index,
                                                 BattleManager battleManager,
                                                 PlayerCharacter playerCharacter,
                                                 Action<int> onClose)
        {
            MonsterSummary.OnClose += lastIndex =>
                                      {
                                          MonsterSummary.OnClose = null;

                                          onClose?.Invoke(lastIndex);
                                      };

            MonsterSummary.Show(roster, index, battleManager, playerCharacter);
        }

        /// <summary>
        /// Open and show the dex screen.
        /// </summary>
        public static void ShowDexScreen(PlayerCharacter playerCharacter) =>
            Instance.DexScreen.OpenDexScreen(playerCharacter);

        /// <summary>
        /// Show the dex screen for the given monster.
        /// </summary>
        /// <param name="monster">Monster to display.</param>
        /// <param name="form">Form to display.</param>
        /// <param name="gender">Gender to display.</param>
        /// <param name="personality">Personality to display monster sprites.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="entriesToUse">Entries to use when displaying, if null, it will use the default dex order.</param>
        /// <param name="onClose">Callback stating the last selected index.</param>
        public static void ShowSingleMonsterDexScreen(MonsterEntry monster,
                                                      Form form,
                                                      MonsterGender gender,
                                                      int personality,
                                                      PlayerCharacter playerCharacter,
                                                      List<MonsterDexEntry> entriesToUse = null,
                                                      Action<int> onClose = null)
        {
            Instance.SingleMonsterDexScreen.OnClose += index =>
                                                       {
                                                           Instance.SingleMonsterDexScreen.OnClose = null;
                                                           onClose?.Invoke(index);
                                                       };

            Instance.SingleMonsterDexScreen.ShowScreen(monster,
                                                       form,
                                                       gender,
                                                       entriesToUse,
                                                       personality,
                                                       playerCharacter);
        }

        /// <summary>
        /// Display the player's bag.
        /// </summary>
        /// <param name="onBack">Event raised when the bag gets closed.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="battleManager">Reference to the battle manager if we are in battle.</param>
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
        public static void ShowBag(Action<bool, Item> onBack,
                                   PlayerCharacter playerCharacter = null,
                                   BattleManager battleManager = null,
                                   int currentBattlerIndex = -1,
                                   bool overrideDisplayed = false,
                                   bool[] displayOverride = null,
                                   Action<BattleAction> onBattleItemSelectedCallback = null,
                                   MonsterInstance[] rosterOverride = null,
                                   bool selection = false,
                                   ItemCompatibilityChecker itemCompatibilityChecker = null,
                                   bool selling = false) =>
            Instance.BagScreen.ShowBag(onBack,
                                       playerCharacter,
                                       battleManager,
                                       currentBattlerIndex,
                                       overrideDisplayed,
                                       displayOverride,
                                       onBattleItemSelectedCallback,
                                       rosterOverride,
                                       selection,
                                       itemCompatibilityChecker,
                                       selling);

        /// <summary>
        /// Show the menu for the quick access items.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        [FoldoutGroup("Debug")]
        [Button]
        public static void ShowQuickAccessItems(PlayerCharacter playerCharacter) =>
            Instance.QuickAccessItemsMenu.ShowMenu(playerCharacter);

        /// <summary>
        /// Show the shop dialog.
        /// </summary>
        /// <param name="items">Items available..</param>
        /// <param name="prices">Prices to buy at.</param>
        /// <param name="promotions">Promotions the shop has.</param>
        public static IEnumerator ShowShop(List<Item> items,
                                           List<uint> prices,
                                           SerializableDictionary<Item, ObjectPair<uint, Item>> promotions)
        {
            yield return Instance.ShopDialog.ShowShop(items, prices, promotions);
        }

        /// <summary>
        /// Show the quests screen.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        [FoldoutGroup("Debug")]
        [Button]
        public static void ShowQuestsScreen(PlayerCharacter playerCharacter) =>
            Instance.QuestsScreen.ShowQuests(playerCharacter);

        /// <summary>
        /// Show the profile screen.
        /// </summary>
        [FoldoutGroup("Debug")]
        [Button]
        public static void ShowProfileScreen(PlayerCharacter playerCharacter) =>
            Instance.ProfileScreen.ShowScreen(playerCharacter);

        /// <summary>
        /// Show the options menu.
        /// </summary>
        [FoldoutGroup("Debug")]
        [Button]
        public static void ShowOptionsMenu() => Instance.OptionsMenu.ShowOptions();

        /// <summary>
        /// Request the player to enter a text.
        /// </summary>
        /// <param name="maxSize">Max size of the text.</param>
        /// <param name="promptLocalizationKey">Localization key of the prompt to display to the player.</param>
        /// <param name="promptImage">Image to prompt to the player.</param>
        /// <param name="promptModifiers">Prompt text modifiers.</param>
        /// <param name="result">An event called when the player finishes entering.
        /// The bool states if the player entered or cancelled and the string is the entered text.</param>
        /// <param name="canPlayerCancel">Can the player can and exit?</param>
        /// <param name="defaultText">Default text already entered</param>
        public static IEnumerator RequestTextInput(byte maxSize,
                                                   string promptLocalizationKey,
                                                   string[] promptModifiers,
                                                   Sprite promptImage,
                                                   Action<bool, string> result,
                                                   bool canPlayerCancel = true,
                                                   string defaultText = "")
        {
            bool enteredText = false;
            string text = string.Empty;

            Instance.TextInputDialog.OnFinished += (didEnter, newText) =>
                                                   {
                                                       Instance.TextInputDialog.OnFinished = null;
                                                       text = newText;
                                                       enteredText = didEnter && !text.IsNullEmptyOrWhiteSpace();
                                                   };

            yield return Instance.TextInputDialog.RequestText(maxSize,
                                                              promptLocalizationKey,
                                                              promptModifiers,
                                                              promptImage,
                                                              canPlayerCancel,
                                                              defaultText);

            result?.Invoke(enteredText, text);
        }

        /// <summary>
        /// Display a dialog on screen.
        /// <param name="text">Dialog object.</param>
        /// <param name="acceptInput">Should the dialog accept input?</param>
        /// </summary>
        private void ShowDialog(DialogText text, bool acceptInput = true)
        {
            AcceptInput = acceptInput;

            pendingDialogKeys.Enqueue(text);

            if (active) return;

            inputManager.RequestInput(this);
            NextDialog();
            BasicDialog.Show();
            active = true;
        }

        /// <summary>
        /// Close all player menus.
        /// </summary>
        public static IEnumerator CloseMenus()
        {
            yield return Instance.StartCoroutine(Instance.CloseMenusRoutine());
        }

        /// <summary>
        /// Close all player menus.
        /// </summary>
        private IEnumerator CloseMenusRoutine()
        {
            yield return mapSceneLauncher.CloseMap();

            yield return SingleMonsterDexScreen.HideScreenRoutine();

            yield return DexScreen.CloseRoutine();

            yield return MonsterSummary.CloseRoutine();

            yield return MonstersMenu.CloseMenuRoutine();

            yield return BagScreen.HideRoutine();

            yield return GameMenu.HideRoutine();
        }

        /// <summary>
        /// This input receiver is UI.
        /// </summary>
        public InputType GetInputType() => InputType.UI;

        /// <summary>
        /// Debug name of this object.
        /// </summary>
        public string GetDebugName() => gameObject == null ? "Null" : name;

        /// <summary>
        /// Switch to the next dialog.
        /// </summary>
        /// <param name="context"></param>
        public void OnSelect(InputAction.CallbackContext context)
        {
            // Only read the button press.
            if (!context.started || !AcceptInput) return;
            AudioManager.Instance.PlayAudio(NextDialogAudio);
            NextDialog();
        }

        /// <summary>
        /// Treat the back button just the same as the select one.
        /// </summary>
        public void OnBack(InputAction.CallbackContext context) => OnSelect(context);

        /// <summary>
        /// Switch to the next dialog or skip the typewriter.
        /// </summary>
        [FoldoutGroup("Debug")]
        [Button]
        public static void NextDialog()
        {
            if (Instance.Text.IsTypewriting)
                Instance.Text.Typewriter.SkipTypewriter();
            else
                Instance.NextDialogInternal();
        }

        /// <summary>
        /// Check if the basic dialog is currently typewriting.
        /// </summary>
        /// <returns></returns>
        public static bool IsTypewritingDialog() => Instance.Text.IsTypewriting;

        /// <summary>
        /// Switch to the next dialog.
        /// </summary>
        private void NextDialogInternal()
        {
            if (nextDialogAfterSecondsRoutine != null) StopCoroutine(nextDialogAfterSecondsRoutine);

            OnNextDialog?.Invoke();

            if (PagesLeft())
                Text.Text.pageToDisplay++;
            else if (pendingDialogKeys.TryDequeue(out DialogText nextText))
            {
                Logger.Info("Showing dialog: '" + nextText.ToString(localizer) + "'");

                foreach (KeyValuePair<BasicDialogBackground, HidableUiElement> pair in Backgrounds)
                    pair.Value.Show(pair.Key == nextText.Background);

                Text.Text.horizontalAlignment = nextText.HorizontalAlignment;

                Text.SetTypewriterSpeed(nextText.TypewriterSpeed);
                Text.SetValue(nextText.LocalizationKey, nextText.LocalizableModifiers, nextText.Modifiers);
                Text.Text.pageToDisplay = 1;

                if (nextText.Character != null)
                {
                    CharacterName.SetValue(nextText.Character.LocalizableName);
                    CharacterPanel.Show();
                }
                else if (nextText.Monster != null)
                {
                    CharacterName.Text.SetText(nextText.Monster.GetNameOrNickName(localizer));
                    CharacterPanel.Show();
                }
                else
                    CharacterPanel.Show(false);

                if (nextText.SwitchToNextAfterSeconds > 0)
                    nextDialogAfterSecondsRoutine =
                        StartCoroutine(NextDialogAfterSeconds(nextText.SwitchToNextAfterSeconds));
            }
            else
            {
                inputManager.ReleaseInput(this);
                Text.SetValue("");
                BasicDialog.Show(false);
                active = false;
            }
        }

        /// <summary>
        /// Switch to the next dialog after the seconds have passed.
        /// </summary>
        /// <param name="seconds">Seconds to pass.</param>
        private IEnumerator NextDialogAfterSeconds(float seconds)
        {
            yield return new WaitWhile(() => Text.IsTypewriting);
            yield return new WaitForSeconds(seconds);
            NextDialog();
        }

        /// <summary>
        /// Are there pages left on the text?
        /// </summary>
        private bool PagesLeft() => Text.Text.textInfo.pageCount > Text.Text.pageToDisplay;

        #region UnusedInputCallbacks

        /// <summary>
        /// Called when the input is active.
        /// </summary>
        public void OnStateEnter()
        {
        }

        /// <summary>
        /// Called when the input is inactive.
        /// </summary>
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

        public void OnNavigation(InputAction.CallbackContext context)
        {
        }

        public void OnExtra1(InputAction.CallbackContext context)
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

        /// <summary>
        /// Enumeration of the backgrounds the basic dialog can have.
        /// </summary>
        public enum BasicDialogBackground
        {
            Normal,
            Sign
        }

        /// <summary>
        /// Factory used for dependency injection.
        /// </summary>
        public class Factory : GameObjectFactory<DialogManager>
        {
        }
    }
}