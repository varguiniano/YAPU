using System;
using System.Collections;
using System.IO;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.Rendering;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.World;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Localization.Runtime;
using Zenject;
using Random = UnityEngine.Random;

namespace Varguiniano.YAPU.Runtime.Initialization
{
    /// <summary>
    /// Behaviour in charge of all YAPU initialization to be executed before the game is ready.
    /// </summary>
    public class YAPUInitialization : WhateverBehaviour<YAPUInitialization>
    {
        /// <summary>
        /// Reference to the first time language selector.
        /// </summary>
        [SerializeField]
        private FirstTimeLanguageSelector FirstTimeLanguageSelector;

        /// <summary>
        /// Reference to the controls view.
        /// </summary>
        [SerializeField]
        private ControlsView ControlsView;

        /// <summary>
        /// Called when the app is initialized.
        /// </summary>
        public Action Initialized;

        /// <summary>
        /// Initialize after dependency injection.
        /// </summary>
        [Inject]
        public void Construct(DialogManager.Factory dialogManagerFactory,
                              RenderingManager renderingManager,
                              MonsterDatabaseInstance monsterDatabase,
                              WorldDatabase worldDatabase,
                              ILocalizer localizer) =>
            StartCoroutine(Initialize(dialogManagerFactory,
                                      renderingManager,
                                      monsterDatabase,
                                      worldDatabase,
                                      localizer));

        /// <summary>
        /// Initialize one frame after dependency injection.
        /// </summary>
        private IEnumerator Initialize(DialogManager.Factory dialogManagerFactory,
                                       RenderingManager renderingManager,
                                       MonsterDatabaseInstance monsterDatabase,
                                       WorldDatabase worldDatabase,
                                       ILocalizer localizer)
        {
            yield return WaitAFrame;

            Logger.Info("Starting YAPU initialization.");

            TransitionManager.BlackScreenFadeIn();

            InitDialogManager(dialogManagerFactory);

            yield return WaitAFrame;

            DialogManager.ShowLoadingIcon();

            InitRandom();

            DisableCursor();

            // Wait some frames to let the Windows graphics API catch up.
            for (int i = 0; i < 40; i++) yield return WaitAFrame;

            SetScreenSettings(renderingManager);

            monsterDatabase.LoadLookupTables();
            worldDatabase.LoadLookupTables();

            yield return DownloadLatestLocalizationTables(localizer);

            yield return CheckAndRunFirstTimeSetup();

            Logger.Info("Finished YAPU initialization.");

            Initialized?.Invoke();
        }

        /// <summary>
        /// Init the random generator with random values.
        /// </summary>
        private static void InitRandom() => Random.InitState(Random.Range(int.MinValue, int.MaxValue));

        /// <summary>
        /// Set the screen settings from config.
        /// </summary>
        private void SetScreenSettings(RenderingManager renderingManager)
        {
            renderingManager.UpdateToDisplayOnCurrentMonitor();

            #if !UNITY_STANDALONE_WIN && !UNITY_EDITOR
            renderingManager.SetWindowType(WindowType.ExclusiveFullscreen);
            #endif

            StartCoroutine(renderingManager.UpdateResolutionAndFullscreen(false));
        }

        /// <summary>
        /// Hide the cursor.
        /// </summary>
        private void DisableCursor()
        {
            #if UNITY_EDITOR
            Logger.Info("Not disabling cursor in editor.");
            #else
            Cursor.visible = false;
            #endif
        }

        /// <summary>
        /// Initialize the dialog manager instance.
        /// Doing it through a factory ensures it receives DI.
        /// </summary>
        private static void InitDialogManager(DialogManager.Factory dialogManagerFactory) =>
            dialogManagerFactory.CreateGameObject(null, Vector3.zero, Quaternion.identity).name = "DialogManager";

        /// <summary>
        /// Download the latest localization tables available.
        /// </summary>
        private IEnumerator DownloadLatestLocalizationTables(ILocalizer localizer)
        {
            DialogManager.ShowLoadingText("Dialogs/LoadingLanguages");
            yield return WaitAFrame;
            yield return WaitAFrame;

            try
            {
                localizer.ReDownloadLanguagesFromGoogleSheet();
            }
            catch (Exception e)
            {
                Logger.Error("Exception downloading localization tables: " + e.Message, e);
            }

            DialogManager.ClearLoadingText();
        }

        /// <summary>
        /// Check and run the first time setup.
        /// Ask the player for the language.
        /// Create the first time run file to mark that the game has been run at least once.
        /// </summary>
        private IEnumerator CheckAndRunFirstTimeSetup()
        {
            string firstTimeRunFile = Application.persistentDataPath + "/FirstTimeRun";

            if (File.Exists(firstTimeRunFile)) yield break;

            Logger.Info("Running game for the first time, asking the player for language.");

            bool languageChosen = false;

            FirstTimeLanguageSelector.OnLanguageChosen += () => languageChosen = true;

            FirstTimeLanguageSelector.Show();

            DialogManager.ShowLoadingIcon(false);
            yield return TransitionManager.BlackScreenFadeOutRoutine();

            yield return new WaitUntil(() => languageChosen);

            DialogManager.ShowLoadingIcon();
            yield return TransitionManager.BlackScreenFadeInRoutine();

            FirstTimeLanguageSelector.Show(false);

            bool controlsReviewed = false;

            ControlsView.PlayerContinued += () => controlsReviewed = true;

            ControlsView.Show();

            DialogManager.ShowLoadingIcon(false);
            yield return TransitionManager.BlackScreenFadeOutRoutine();

            yield return new WaitUntil(() => controlsReviewed);

            DialogManager.ShowLoadingIcon();
            yield return TransitionManager.BlackScreenFadeInRoutine();

            File.Create(firstTimeRunFile);
        }
    }
}