using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Saves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.UI.Map;
using Varguiniano.YAPU.Runtime.UI.Time;
using Varguiniano.YAPU.Runtime.World;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.GameMenu
{
    /// <summary>
    /// Controller for the game menu screen.
    /// </summary>
    public class GameMenuScreen : MenuSelector
    {
        /// <summary>
        /// Reference to the menu panel.
        /// </summary>
        [SerializeField]
        private Transform MenuPanel;

        /// <summary>
        /// Position to be at when shown.
        /// </summary>
        [SerializeField]
        private Transform ShownPosition;

        /// <summary>
        /// Position to be at when hidden.
        /// </summary>
        [SerializeField]
        private Transform HiddenPosition;

        /// <summary>
        /// Reference to the clock.
        /// </summary>
        [SerializeField]
        private Clock Clock;

        /// <summary>
        /// Action called when the menu is closed.
        /// </summary>
        private Action backEvent;

        /// <summary>
        /// Reference to the player character.
        /// </summary>
        private PlayerCharacter playerCharacterReference;

        /// <summary>
        /// Flag to know if we are exiting the game.
        /// </summary>
        private bool exitingGame;

        /// <summary>
        /// Reference to the savegame manager.
        /// </summary>
        [Inject]
        private SavegameManager savegameManager;

        /// <summary>
        /// Reference to the map scene launcher.
        /// </summary>
        [Inject]
        private MapSceneLauncher mapSceneLauncher;

        /// <summary>
        /// Reference to the scene manager.
        /// </summary>
        [Inject]
        private ISceneManager sceneManager;

        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        [Inject]
        private YAPUSettings settings;

        /// <summary>
        /// Reference to the global grid manager.
        /// </summary>
        [Inject]
        private GlobalGridManager globalGridManager;

        /// <summary>
        /// Slide the menu in.
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="onBack">Action called when the menu is closed.</param>
        /// </summary>
        [Button]
        [HideInEditorMode]
        public void ShowMenu(PlayerCharacter playerCharacter, Action onBack)
        {
            playerCharacterReference = playerCharacter;
            backEvent += onBack;
            if (!Shown) StartCoroutine(ShowRoutine());
        }

        /// <summary>
        /// Slide the menu out.
        /// </summary>
        [Button]
        [HideInEditorMode]
        public void HideMenu() => StartCoroutine(HideRoutine());

        /// <summary>
        /// Slide the menu in.
        /// </summary>
        private IEnumerator ShowRoutine()
        {
            InputManager.BlockInput();

            MenuPanel.localPosition = HiddenPosition.localPosition;

            AudioManager.PlayAudio(SelectAudio);

            Clock.StartUpdating(playerCharacterReference);

            Show();

            yield return MenuPanel.DOLocalMove(ShownPosition.localPosition, .25f)
                                  .SetEase(Ease.OutBack)
                                  .WaitForCompletion();

            OnButtonSelected += ButtonSelected;

            InputManager.BlockInput(false);

            RequestInput();

            yield return ReselectNextFrameRoutine();
        }

        /// <summary>
        /// Slide the menu out.
        /// </summary>
        public IEnumerator HideRoutine()
        {
            OnButtonSelected -= ButtonSelected;

            ReleaseInput();

            InputManager.BlockInput();

            MenuPanel.localPosition = ShownPosition.localPosition;

            yield return MenuPanel.DOLocalMove(HiddenPosition.localPosition, .25f)
                                  .SetEase(Ease.InBack)
                                  .WaitForCompletion();

            Show(false);

            Clock.StopUpdating();

            InputManager.BlockInput(false);
        }

        /// <summary>
        /// Hide the menu on back.
        /// </summary>
        public override void OnBack(InputAction.CallbackContext context)
        {
            // Get input only when key goes down.
            if (context.phase != InputActionPhase.Started) return;

            base.OnBack(context);

            HideMenu();

            backEvent?.Invoke();
        }

        /// <summary>
        /// Hide the menu on the menu key.
        /// </summary>
        public override void OnExtra1(InputAction.CallbackContext context)
        {
            // Get input only when key goes down.
            if (context.phase != InputActionPhase.Started) return;

            if (PlayAudioOnBack) AudioManager.PlayAudio(SelectAudio);

            Deselect(CurrentSelection);

            HideMenu();

            backEvent?.Invoke();
        }

        /// <summary>
        /// Attempt to save.
        /// </summary>
        public override void OnExtra2(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            AudioManager.PlayAudio(SelectAudio);

            StartCoroutine(TrySave());
        }

        /// <summary>
        /// Ask the player if they want to save and save if they do.
        /// </summary>
        private IEnumerator TrySave()
        {
            int choice = -1;

            DialogManager.ShowChoiceMenu(new List<string>
                                         {
                                             "Common/True",
                                             "Common/False"
                                         },
                                         playerChoice => choice = playerChoice,
                                         onBackCallback: () => choice = 1,
                                         showDialog: true,
                                         localizationKey: "Menu/Save/Confirmation");

            yield return new WaitUntil(() => choice != -1);

            if (choice != 0) yield break;

            yield return savegameManager.SaveGameWithCurrentDate(playerCharacterReference);
            yield return DialogManager.ShowDialogAndWait("Menu/Save/Finished");
        }

        /// <summary>
        /// Called when a button is selected.
        /// </summary>
        /// <param name="index">Selected button.</param>
        private void ButtonSelected(int index)
        {
            switch (index)
            {
                case 0: // Dex
                    DialogManager.ShowDexScreen(playerCharacterReference);
                    break;
                case 1: //Mons
                    DialogManager.ShowPlayerRosterMenu(playerCharacterReference);
                    break;
                case 2: // Bag
                    DialogManager.ShowBag(null, playerCharacterReference);
                    break;
                case 3: // Map
                    mapSceneLauncher.ShowMap(playerCharacterReference);
                    break;
                case 4: // Quests
                    DialogManager.ShowQuestsScreen(playerCharacterReference);
                    break;
                case 5: // Profile
                    DialogManager.ShowProfileScreen(playerCharacterReference);
                    break;
                case 6: // Options
                    DialogManager.ShowOptionsMenu();
                    break;
                case 7: // Exit
                    if (!exitingGame) StartCoroutine(ExitRoutine());
                    break;
            }
        }

        /// <summary>
        /// Routine to ask the player to save and then exit.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ExitRoutine()
        {
            exitingGame = true;

            yield return WaitAFrame;
            yield return WaitAFrame;

            int exitChoice = -1;

            DialogManager.ShowChoiceMenu(new List<string>
                                         {
                                             "Menu/Exit/Game",
                                             "Menu/Exit/MainMenu",
                                             "Common/Cancel"
                                         },
                                         playerChoice => exitChoice = playerChoice,
                                         onBackCallback: () => exitChoice = 2);

            yield return new WaitUntil(() => exitChoice != -1);

            yield return WaitAFrame;

            if (exitChoice == 2)
            {
                exitingGame = false;
                yield break;
            }

            yield return HideRoutine();

            int saveChoice = -1;

            DialogManager.ShowChoiceMenu(new List<string>
                                         {
                                             "Common/True",
                                             "Common/False"
                                         },
                                         playerChoice => saveChoice = playerChoice,
                                         onBackCallback: () => saveChoice = 1,
                                         showDialog: true,
                                         localizationKey: "Menu/Exit/SaveConfirmation");

            yield return new WaitUntil(() => saveChoice != -1);

            if (saveChoice == 0)
            {
                yield return savegameManager.SaveGameWithCurrentDate(playerCharacterReference);
                yield return DialogManager.ShowDialogAndWait("Menu/Save/Finished");
            }

            if (exitChoice == 0)
                Utils.CloseGame();
            else
                CoroutineRunner.RunRoutine(LoadMainMenu());
        }

        /// <summary>
        /// Load the game's main menu.
        /// </summary>
        private IEnumerator LoadMainMenu()
        {
            InputManager.BlockInput();
            yield return TransitionManager.BlackScreenFadeInRoutine();
            DialogManager.ShowLoadingIcon();

            DialogManager.Notifications.StopAllNotifications();

            bool loaded = false;

            sceneManager.LoadScene(settings.MainMenuScene,
                                   _ =>
                                   {
                                   },
                                   _ =>
                                   {
                                       loaded = true;
                                   },
                                   LoadSceneMode.Single);

            yield return new WaitUntil(() => loaded);

            // Clean up all player data from the global grid manager.
            // TODO: It will only be called here but should there be a better way to do this?
            globalGridManager.StopCurrentSceneMusic();
            globalGridManager.ClearPlayerCachedData();
            yield return new WaitForSeconds(.2f);

            AudioManager.StopAllAudios();

            InputManager.BlockInput(false);
            DialogManager.ShowLoadingIcon(false);
            yield return TransitionManager.BlackScreenFadeOutRoutine();

            exitingGame = false;
        }
    }
}