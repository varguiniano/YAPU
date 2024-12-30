using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using Varguiniano.YAPU.Runtime.Actors;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Other;
using Varguiniano.YAPU.Runtime.Saves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.World;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Configuration;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.GameFlow
{
    /// <summary>
    /// Controller class to teleport the player around between scenes.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/GameFlow/PlayerTeleporter",
                     fileName = "PlayerTeleporter")]
    public class PlayerTeleporter : WhateverScriptable<PlayerTeleporter>
    {
        /// <summary>
        /// Reference to an empty scene to load while teleporting.
        /// </summary>
        [SerializeField]
        private SceneReference EmptyScene;

        /// <summary>
        /// Reference to the current player character.
        /// </summary>
        private PlayerCharacter currentPlayerCharacter;

        /// <summary>
        /// Reference to the player character factory.
        /// </summary>
        [Inject]
        private PlayerCharacter.Factory playerCharacterFactory;

        /// <summary>
        /// Reference to the scene manager.
        /// </summary>
        [Inject]
        private ISceneManager sceneManager;

        /// <summary>
        /// Reference to the audio manager.
        /// </summary>
        [Inject]
        private IAudioManager audioManager;

        /// <summary>
        /// Reference to the configuration manager.
        /// </summary>
        [Inject]
        private IConfigurationManager configurationManager;

        /// <summary>
        /// Reference to the savegame manager.
        /// </summary>
        [Inject]
        private SavegameManager savegameManager;

        /// <summary>
        /// Reference to the input manager.
        /// </summary>
        [Inject]
        private IInputManager inputManager;

        /// <summary>
        /// Teleport the player to the given location.
        /// </summary>
        /// <param name="sceneLocation">Location to teleport to.</param>
        [Button("Teleport")]
        [HideInEditorMode]
        private void TestTeleportPlayer(SceneLocation sceneLocation) =>
            CoroutineRunner.RunRoutine(TeleportPlayer(sceneLocation));

        /// <summary>
        /// Teleport the player to the given location.
        /// </summary>
        /// <param name="sceneLocation">Location to teleport to.</param>
        /// <param name="overBridge">Place the character over a bridge?</param>
        /// <param name="isGameLoading">Is the game loading?</param>
        public IEnumerator TeleportPlayer(SceneLocation sceneLocation,
                                          bool overBridge = false,
                                          bool isGameLoading = false)
        {
            inputManager.BlockInput();

            CoroutineRunner.RunRoutine(audioManager.MuteAllAudios(.3f));

            yield return TransitionManager.BlackScreenFadeInRoutine();

            DialogManager.ShowLoadingIcon();

            SceneInfoAsset oldScene = null;

            bool wasStrong = false;
            bool wasUsingFlash = false;

            bool wasRunning = false;
            bool wasBiking = false;

            bool repelInfoRetrieved = false;

            (uint, Repel) repelInfo = (0, null);

            if (currentPlayerCharacter != null && currentPlayerCharacter.gameObject != null)
            {
                oldScene = currentPlayerCharacter.CharacterController.CurrentGrid.SceneInfo.Asset;

                wasStrong = currentPlayerCharacter.Strong;
                wasUsingFlash = currentPlayerCharacter.Flash;

                wasRunning = currentPlayerCharacter.CharacterController.IsRunning;
                wasBiking = currentPlayerCharacter.CharacterController.IsBiking;

                repelInfo = currentPlayerCharacter.GetRepelInfo();
                repelInfoRetrieved = true;

                Destroy(currentPlayerCharacter.gameObject);
                currentPlayerCharacter = null;
            }

            inputManager.BlockInput(false);

            yield return WaitAFrame;
            yield return WaitAFrame;

            bool loaded = false;

            sceneManager.LoadScene(EmptyScene,
                                   _ =>
                                   {
                                   },
                                   _ => loaded = true,
                                   LoadSceneMode.Single);

            yield return new WaitUntil(() => loaded);

            loaded = false;

            sceneManager.LoadScene(sceneLocation.Scene.Scene,
                                   _ =>
                                   {
                                   },
                                   _ => loaded = true,
                                   LoadSceneMode.Single);

            yield return new WaitUntil(() => loaded);

            currentPlayerCharacter =
                playerCharacterFactory.CreateGameObject(null, (Vector2) sceneLocation.Location, Quaternion.identity);

            yield return WaitAFrame;

            inputManager.BlockInput();

            sceneManager.MoveObjectToActiveScene(currentPlayerCharacter.gameObject);

            yield return WaitAFrame;

            // TODO: Destroy any blocking actors in the way of the player.

            if (overBridge) currentPlayerCharacter.CharacterController.PlaceOnTopOfBridge();

            currentPlayerCharacter.CharacterController.Initialize();

            yield return WaitAFrame;

            GridController grid = currentPlayerCharacter.CharacterController.CurrentGrid;

            SceneInfo newScene = grid.SceneInfo;

            if (oldScene != null)
                // If the player is still in the same area, keep the strong and flash values.
                if (newScene.LocalizableNameKey == oldScene.LocalizableNameKey)
                {
                    currentPlayerCharacter.Strong = wasStrong;
                    currentPlayerCharacter.Flash = wasUsingFlash;
                }

            // Reapply FX.
            currentPlayerCharacter.ApplySceneFX(newScene);

            currentPlayerCharacter.CharacterController.IsRunning = wasRunning;
            currentPlayerCharacter.CharacterController.IsBiking = wasBiking;

            if (repelInfoRetrieved) currentPlayerCharacter.SetRepelInfo(repelInfo.Item1, repelInfo.Item2);

            List<TriggerActor> triggers =
                grid.GetListOfActorTypesInPosition<TriggerActor>(currentPlayerCharacter.Position);

            foreach (TriggerActor trigger in triggers)
                yield return trigger.PlayerAboutToBeTeleported(currentPlayerCharacter);

            if (!configurationManager.GetConfiguration(out GameplayConfiguration configuration))
                Logger.Error("Couldn't retrieve gameplay configuration, things will break.");

            if (!isGameLoading && configuration.AutoSaveOnTeleporting)
                yield return savegameManager.Autosave(currentPlayerCharacter);

            CoroutineRunner.RunRoutine(audioManager.UnmuteAllAudios(.3f));

            DialogManager.ShowLoadingIcon(false);

            TransitionManager.BlackScreenFadeOut();

            foreach (TriggerActor trigger in triggers) yield return trigger.PlayerTeleported(currentPlayerCharacter);

            inputManager.BlockInput(false);
            currentPlayerCharacter.LockInput(false);
        }
    }
}