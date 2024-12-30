using System;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Saves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.MainMenu
{
    /// <summary>
    /// Controller for the menu on the main title screen.
    /// </summary>
    [RequireComponent(typeof(MenuSelector))]
    public class MainMenuController : WhateverBehaviour<MainMenuController>
    {
        /// <summary>
        /// Reference to the screen to load games.
        /// </summary>
        [SerializeField]
        private LoadGameScreen LoadGameScreen;

        /// <summary>
        /// Reference to the credits screen.
        /// </summary>
        [SerializeField]
        private CreditsScreen CreditsScreen;

        /// <summary>
        /// Initializer for a new game.
        /// </summary>
        [Inject]
        private NewGameInitializer newGameInitializer;

        /// <summary>
        /// Reference to the savegame manager.
        /// </summary>
        [Inject]
        private SavegameManager savegameManager;

        /// <summary>
        /// Subscribe to the menu.
        /// </summary>
        private void OnEnable()
        {
            bool savegamesAvailable = SavegameManager.AreThereSavegames();

            GetCachedComponent<MenuSelector>()
               .UpdateLayout(new List<bool>
                             {
                                 savegamesAvailable,
                                 savegamesAvailable,
                                 true,
                                 true,
                                 true,
                                 true
                             });

            GetCachedComponent<MenuSelector>().OnButtonSelected += OnOptionSelected;
            GetCachedComponent<MenuSelector>().Show();
        }

        /// <summary>
        /// Release input before unloading the scene.
        /// </summary>
        private void OnDisable() => GetCachedComponent<MenuSelector>().ReleaseInput();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        private void OnOptionSelected(int index)
        {
            switch (index)
            {
                case 0:
                    GetCachedComponent<MenuSelector>().Show(false);
                    CoroutineRunner.RunRoutine(savegameManager.LoadLastSavegame());
                    break;
                case 1:
                    StartCoroutine(LoadGameScreen.ShowScreen());
                    break;
                case 2:
                    GetCachedComponent<MenuSelector>().Show(false);
                    CoroutineRunner.RunRoutine(newGameInitializer.StartNewGame());
                    break;
                case 3:
                    DialogManager.ShowOptionsMenu();
                    break;
                case 4:
                    CreditsScreen.Show();
                    break;
                case 5:
                    GetCachedComponent<MenuSelector>().Show(false);
                    Utils.CloseGame();
                    break;
            }
        }
    }
}