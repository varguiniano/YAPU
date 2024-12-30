using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.World
{
    /// <summary>
    /// Global manager for managing the grids that are loaded and allow players and actors to interact with them.
    /// </summary>
    public class GlobalGridManager : Singleton<GlobalGridManager>
    {
        /// <summary>
        /// List of currently loaded grids.
        /// </summary>
        [ReadOnly]
        [ShowInInspector]
        [NonSerialized]
        public readonly List<GridController> LoadedGrids = new();

        /// <summary>
        /// Flag to prevent all actors from looping and ticking.
        /// </summary>
        public bool StopAllActors;

        /// <summary>
        /// Flag to know if an actor is attempting to spot the player.
        /// </summary>
        public bool PlayerSpotAttempt;

        /// <summary>
        /// List of currently loaded scenes.
        /// </summary>
        public List<SceneInfoAsset> LoadedScenes => LoadedGrids.Select(grid => grid.SceneInfo.Asset).ToList();

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
        /// Reference to the player character.
        /// </summary>
        private PlayerCharacter playerCharacter;

        /// <summary>
        /// Reference to the last grid the player was in.
        /// </summary>
        private GridController lastGridPlayerWas;

        /// <summary>
        /// Last grid music that was played.
        /// </summary>
        private AudioReference lastGridMusicPlayed;

        /// <summary>
        /// Name of the last grid the player was in.
        /// </summary>
        private string lastGridName;

        /// <summary>
        /// List of subscribers to the player movement event.
        /// </summary>
        private readonly List<IPlayerMovementSubscriber> playerMovementSubscribers = new();

        /// <summary>
        /// List of subscribers that need to unsubscribe.
        /// </summary>
        private readonly Queue<IPlayerMovementSubscriber> toUnsubscribe = new();

        /// <summary>
        /// Flag to know when we are syncing scenes.
        /// </summary>
        private bool syncingScenes;

        /// <summary>
        /// Register a grid to the manager.
        /// </summary>
        /// <param name="grid">Grid to register.</param>
        public void RegisterGrid(GridController grid)
        {
            if (LoadedGrids.Contains(grid))
            {
                Logger.Error("Grid from scene " + grid.gameObject.scene.name + " is already loaded!");
                return;
            }

            grid.PlayerEnteredGrid += PlayerEnteredGrid;
            grid.PlayerExitedGrid += PlayerExitedGrid;

            LoadedGrids.Add(grid);

            Logger.Info("Registered grid from scene " + grid.gameObject.scene.name + ".");
        }

        /// <summary>
        /// Unregister a grid from the manager.
        /// </summary>
        /// <param name="grid">Grid to unregister.</param>
        public void UnregisterGrid(GridController grid)
        {
            if (!LoadedGrids.Contains(grid))
            {
                Logger.Error("Grid from scene " + grid.gameObject.scene.name + " is not loaded!");
                return;
            }

            grid.PlayerEnteredGrid -= PlayerEnteredGrid;
            grid.PlayerExitedGrid -= PlayerExitedGrid;

            LoadedGrids.Remove(grid);

            if (lastGridPlayerWas == grid) lastGridPlayerWas = null;

            Logger.Info("Unregistered grid from scene " + grid.gameObject.scene.name + ".");
        }

        /// <summary>
        /// Find a grid that has a tile in the given position.
        /// </summary>
        /// <param name="position">Position to check.</param>
        /// <returns>The controller of the grid if found or null if not found.</returns>
        public GridController FindGridWithTileInPosition(Vector3Int position)
        {
            GridController candidate = null;

            foreach (GridController grid in LoadedGrids.Where(grid => grid.HasTile(position))) candidate = grid;

            return candidate;
        }

        /// <summary>
        /// Subscribe to the player movement.
        /// </summary>
        /// <param name="subscriber">Subscriber.</param>
        public void SubscribeToPlayerMovement(IPlayerMovementSubscriber subscriber) =>
            playerMovementSubscribers.Add(subscriber);

        /// <summary>
        /// Unsubscribe from the player movement.
        /// </summary>
        /// <param name="unSubscriber">UnSubscriber.</param>
        public void UnSubscribeFromPlayerMovement(IPlayerMovementSubscriber unSubscriber) =>
            toUnsubscribe.Enqueue(unSubscriber);

        /// <summary>
        /// Called by the player to communicate all objects that it moved.
        /// </summary>
        public IEnumerator PlayerMoved(PlayerCharacter player)
        {
            if (player == null) yield break;

            if (syncingScenes) yield return new WaitWhile(() => syncingScenes);

            while (toUnsubscribe.TryDequeue(out IPlayerMovementSubscriber unSubscriber))
                if (playerMovementSubscribers.Contains(unSubscriber))
                    playerMovementSubscribers.Remove(unSubscriber);

            int finished = 0;

            foreach (IPlayerMovementSubscriber subscriber in playerMovementSubscribers)
                StartCoroutine(subscriber.PlayerMoved(player, () => finished++));

            yield return new WaitUntil(() => finished == playerMovementSubscribers.Count);
        }

        /// <summary>
        /// Called when the player entered a grid.
        /// </summary>
        /// <param name="grid">Grid the player entered.</param>
        /// <param name="playerCharacterReference">Reference to the player.</param>
        /// <param name="forceReenter">Force reentering?</param>
        private void PlayerEnteredGrid(GridController grid, PlayerCharacter playerCharacterReference, bool forceReenter)
        {
            playerCharacter = playerCharacterReference;

            if (!playerCharacter.GlobalGameData.VisitedScenes.Contains(grid.SceneInfo.Asset))
                playerCharacter.GlobalGameData.VisitedScenes.Add(grid.SceneInfo.Asset);

            StartCoroutine(SyncLoadedScenesWithGridsNeighbours(grid));

            bool playNewAudio = true;

            bool lastMusicWasTheSame = lastGridMusicPlayed.Audio == grid.SceneInfo.BackgroundMusic.Audio;

            if (!forceReenter) playNewAudio = !lastMusicWasTheSame;

            if (playNewAudio)
            {
                if (!lastMusicWasTheSame && !forceReenter) StopCurrentSceneMusic();

                audioManager.PlayAudio(grid.SceneInfo.BackgroundMusic,
                                       true,
                                       fadeTime: 1);
            }

            bool showSceneName = true;

            bool lastSceneNameWasTheSame = lastGridName == grid.SceneInfo.LocalizableNameKey;

            if (!forceReenter) showSceneName = !lastSceneNameWasTheSame;

            if (showSceneName)
                DialogManager.Notifications.QueueSceneEnteredNotification(grid.SceneInfo.LocalizableNameKey);
        }

        /// <summary>
        /// Play the music for the current scene.
        /// </summary>
        public void PlayCurrentSceneMusic() =>
            audioManager.PlayAudio(playerCharacter.CharacterController.CurrentGrid.SceneInfo.BackgroundMusic,
                                   true,
                                   fadeTime: 1);

        /// <summary>
        /// Stop the current scene music.
        /// </summary>
        public void StopCurrentSceneMusic()
        {
            if (!lastGridMusicPlayed.Audio.IsNullEmptyOrWhiteSpace()) audioManager.StopAudio(lastGridMusicPlayed, .2f);
        }

        /// <summary>
        /// Clear all cached data from the player.
        /// </summary>
        public void ClearPlayerCachedData()
        {
            lastGridPlayerWas = null;
            StopAllActors = false;
            lastGridMusicPlayed = new AudioReference { Audio = null };
            lastGridName = "";
        }

        /// <summary>
        /// Called when the player exited a grid.
        /// </summary>
        /// <param name="grid">Grid the player exited.</param>
        public void PlayerExitedGrid(GridController grid)
        {
            lastGridPlayerWas = grid;
            lastGridMusicPlayed = lastGridPlayerWas.SceneInfo.BackgroundMusic;
            lastGridName = lastGridPlayerWas.SceneInfo.LocalizableNameKey;
        }

        /// <summary>
        /// Loads all neighbours of a grid and unloads all non neighbours.
        /// </summary>
        /// <param name="grid">Grid to sync.</param>
        private IEnumerator SyncLoadedScenesWithGridsNeighbours(GridController grid)
        {
            syncingScenes = true;

            List<SceneInfoAsset> loaded = LoadedScenes;

            List<SceneReference> toLoad = (from neighbour in grid.SceneInfo.Neighbours
                                           where !loaded.Contains(neighbour)
                                           select neighbour.Scene).ToList();

            List<SceneReference> toUnload = (from loadedScene in loaded
                                             where !grid.SceneInfo.Neighbours.Contains(loadedScene)
                                                && !grid.SceneInfo.Equals(loadedScene)
                                             select loadedScene.Scene).ToList();

            bool finished = false;
            // ReSharper disable once AccessToModifiedClosure
            WaitUntil waitForFinishLoad = new(() => finished);

            Stopwatch stopwatch = new();

            foreach (SceneReference sceneReference in toUnload)
            {
                finished = false;

                stopwatch.Restart();

                sceneManager.UnloadScene(sceneReference,
                                         _ =>
                                         {
                                         },
                                         _ =>
                                         {
                                             finished = true;
                                         });

                yield return waitForFinishLoad;

                stopwatch.Stop();

                Logger.Info("Scene "
                          + sceneReference.SceneName
                          + " unloaded in "
                          + stopwatch.ElapsedMilliseconds
                          + "ms.");
            }

            foreach (SceneReference sceneReference in toLoad)
            {
                finished = false;

                stopwatch.Restart();

                sceneManager.LoadScene(sceneReference,
                                       _ =>
                                       {
                                       },
                                       _ =>
                                       {
                                           finished = true;
                                       });

                yield return waitForFinishLoad;

                stopwatch.Stop();

                Logger.Info("Scene "
                          + sceneReference.SceneName
                          + " loaded in "
                          + stopwatch.ElapsedMilliseconds
                          + "ms.");
            }

            syncingScenes = false;
        }
    }
}