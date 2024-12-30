using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Bags;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.Saves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.World;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;
using Zenject;

namespace Varguiniano.YAPU.Runtime.GameFlow
{
    /// <summary>
    /// Manager in charge of setting all up for starting a new game.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/GameFlow/NewGameInitializer",
                     fileName = "NewGameInitializer")]
    public class NewGameInitializer : WhateverScriptable<NewGameInitializer>, IPlayerDataReceiver
    {
        /// <summary>
        /// Initial position to start the game on.
        /// </summary>
        public SceneLocation InitialPosition;

        /// <summary>
        /// Initial player roster.
        /// </summary>
        [SerializeField]
        private Roster InitialRoster;

        /// <summary>
        /// Initial player bag.
        /// </summary>
        [SerializeField]
        private Bag InitialBag;

        /// <summary>
        /// Reference to the game options scene.
        /// </summary>
        [SerializeField]
        private SceneReference GameOptionsScene;

        /// <summary>
        /// Reference to the character selection scene.
        /// </summary>
        [SerializeField]
        private SceneReference CharacterSelectionScene;

        /// <summary>
        /// Reference to the player roster.
        /// </summary>
        [Inject]
        private Roster playerRoster;

        /// <summary>
        /// Reference to the player bag.
        /// </summary>
        [Inject]
        private Bag playerBag;

        /// <summary>
        /// Reference to the player teleporter.
        /// </summary>
        [Inject]
        private PlayerTeleporter teleporter;

        /// <summary>
        /// Reference to the savegame manager.
        /// </summary>
        [Inject]
        private SavegameManager savegameManager;

        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        [Inject]
        private YAPUSettings settings;

        /// <summary>
        /// Reference to the scene manager.
        /// </summary>
        [Inject]
        private ISceneManager sceneManager;

        /// <summary>
        /// Flag to mark when the player has finished with the game options.
        /// </summary>
        private bool gameOptionsFinished;

        /// <summary>
        /// Flag to mark when the character selection has finished.
        /// </summary>
        private bool characterSelectionFinished;

        /// <summary>
        /// Start a new game.
        /// </summary>
        /// <returns></returns>
        public IEnumerator StartNewGame()
        {
            DialogManager.ShowLoadingIcon();
            yield return TransitionManager.BlackScreenFadeInRoutine();

            yield return savegameManager.ResetSave();

            playerRoster.CopyFrom(InitialRoster);
            playerBag.CopyFrom(InitialBag);

            gameOptionsFinished = false;

            bool loaded = false;

            sceneManager.LoadScene(GameOptionsScene,
                                   _ =>
                                   {
                                   },
                                   success =>
                                   {
                                       if (success)
                                           loaded = true;
                                       else
                                           Logger.Error("Error game options screen.");
                                   },
                                   LoadSceneMode.Single);

            yield return new WaitUntil(() => loaded);

            DialogManager.ShowLoadingIcon(false);
            yield return TransitionManager.BlackScreenFadeOutRoutine();

            yield return new WaitUntil(() => gameOptionsFinished);

            DialogManager.ShowLoadingIcon();
            yield return TransitionManager.BlackScreenFadeInRoutine();

            if (settings.AllowPlayerToChooseCharacterOnNewGame)
            {
                characterSelectionFinished = false;

                loaded = false;

                sceneManager.LoadScene(CharacterSelectionScene,
                                       _ =>
                                       {
                                       },
                                       success =>
                                       {
                                           if (success)
                                               loaded = true;
                                           else
                                               Logger.Error("Error loading character selection screen.");
                                       },
                                       LoadSceneMode.Single);

                yield return new WaitUntil(() => loaded);

                DialogManager.ShowLoadingIcon(false);
                yield return TransitionManager.BlackScreenFadeOutRoutine();

                yield return new WaitUntil(() => characterSelectionFinished);

                DialogManager.ShowLoadingIcon();
                yield return TransitionManager.BlackScreenFadeInRoutine();
            }

            yield return teleporter.TeleportPlayer(InitialPosition);
        }

        /// <summary>
        /// Method called by the game options when the player finishes with them.
        /// </summary>
        public void OnGameOptionsFinished() => gameOptionsFinished = true;

        /// <summary>
        /// Method called by the character selector when the selection finishes.
        /// </summary>
        public void OnCharacterSelectionFinished() => characterSelectionFinished = true;
    }
}