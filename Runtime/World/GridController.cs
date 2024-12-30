using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;
using Varguiniano.YAPU.Runtime.Actors;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.World.OutOfBattleWeathers;
using Varguiniano.YAPU.Runtime.World.Tiles;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using Zenject;
using CharacterController = Varguiniano.YAPU.Runtime.Characters.CharacterController;

namespace Varguiniano.YAPU.Runtime.World
{
    /// <summary>
    /// Behaviour that acts as a controller of a grid and keeps track of tiles, actors, characters, etc on it.
    /// </summary>
    [RequireComponent(typeof(Grid))]
    public class GridController : WhateverBehaviour<GridController>, ISceneMember
    {
        /// <summary>
        /// Called when the player entered this grid.
        /// It passes the grid, the player reference and if reentering should be forced.
        /// </summary>
        public Action<GridController, PlayerCharacter, bool> PlayerEnteredGrid;

        /// <summary>
        /// Called when the player exited this grid.
        /// </summary>
        public Action<GridController> PlayerExitedGrid;

        /// <summary>
        /// Cached reference to this Grid's tilemaps.
        /// </summary>
        private List<Tilemap> Tilemaps => tilemaps ??= GetComponentsInChildren<Tilemap>(true).ToList();

        /// <summary>
        /// Backfield for Tilemaps.
        /// </summary>
        private List<Tilemap> tilemaps;

        /// <summary>
        /// Cached rendering sort orders of all tilemaps.
        /// </summary>
        private Dictionary<Tilemap, int> SortOrders
        {
            get
            {
                // ReSharper disable once InvertIf
                if (sortOrders == null)
                {
                    sortOrders = new Dictionary<Tilemap, int>();

                    foreach (Tilemap tilemap in Tilemaps)
                        sortOrders[tilemap] = tilemap.GetComponent<TilemapRenderer>().sortingOrder;
                }

                return sortOrders;
            }
        }

        /// <summary>
        /// Backfield for sort orders.
        /// </summary>
        private Dictionary<Tilemap, int> sortOrders;

        /// <summary>
        /// List of characters in this grid.
        /// </summary>
        [ReadOnly]
        [ShowInInspector]
        private List<CharacterController> characters = new();

        /// <summary>
        /// List of actors on this grid.
        /// </summary>
        [ReadOnly]
        [ShowInInspector]
        private List<GridSubscribingActor> actors = new();

        /// <summary>
        /// Reference to the tile data library.
        /// </summary>
        [Inject]
        private TileData tileData;

        /// <summary>
        /// Reference to the grid's scene info.
        /// </summary>
        [Inject]
        [HideInInspector]
        public SceneInfo SceneInfo;

        /// <summary>
        /// Reference to the global grid manager.
        /// </summary>
        [Inject]
        private GlobalGridManager globalGridManager;

        /// <summary>
        /// Cache of the character interacting tiles in each position.
        /// </summary>
        private readonly Dictionary<Vector3Int, List<ICharacterInteractingTile>>
            cachedCharacterInteractingTiles = new();

        /// <summary>
        /// Cache of if each tile has blocking interactions.
        /// </summary>
        private readonly Dictionary<Vector3Int, bool> blockingInteractions = new();

        /// <summary>
        /// Register to the manager.
        /// </summary>
        private void OnEnable()
        {
            SceneInfo.RegisterGrid(this);

            globalGridManager.RegisterGrid(this);

            StartCoroutine(LoadAllMaps());
        }

        /// <summary>
        /// Loads all disabled maps one by one across several frames to reduce lag.
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoadAllMaps()
        {
            foreach (Tilemap tilemap in Tilemaps.Where(tilemap => !tilemap.gameObject.activeSelf))
            {
                tilemap.gameObject.SetActive(true);

                yield return WaitAFrame;

                for (int i = tilemap.origin.x; i < tilemap.origin.x + tilemap.size.x; ++i)
                {
                    for (int j = tilemap.origin.y; j < tilemap.origin.y + tilemap.size.y; ++j)
                        tilemap.RefreshTile(new Vector3Int(i, j));
                }

                yield return WaitAFrame;
            }

            Logger.Info("All tilemaps in " + SceneInfo.Scene.SceneName + " loaded.");
        }

        /// <summary>
        /// Unregister from the manager.
        /// </summary>
        private void OnDisable() => globalGridManager.UnregisterGrid(this);

        /// <summary>
        /// Does this grid have a tile on that position?
        /// </summary>
        /// <param name="position">Position to check.</param>
        /// <returns>True if it has.</returns>
        public bool HasTile(Vector3Int position) => Tilemaps.Any(map => map.HasTile(position));

        /// <summary>
        /// Get the type of the tile that is at the given position on the layer directly below a specific sort order.
        /// </summary>
        /// <param name="tilePosition">Position to check.</param>
        /// <param name="sortOrder">Sort order to check.</param>
        /// <returns>The type of tile or HasCollider if it does.</returns>
        [FoldoutGroup("Debug")]
        [HideInEditorMode]
        [Button]
        public TileType GetTypeOfTileDirectlyBelowSortOrder(Vector3Int tilePosition, int sortOrder)
        {
            List<Tilemap> availableMaps = Tilemaps
                                         .Where(map => SortOrders[map] < sortOrder && map.HasTile(tilePosition))
                                         .OrderByDescending(map => SortOrders[map])
                                         .ToList();

            if (availableMaps.Count == 0) return TileType.NonExistent;

            Tilemap tilemap = null;
            int tileSortOrder = int.MinValue;

            TileBase tile = availableMaps.Select(map =>
                                                 {
                                                     tilemap = map;
                                                     tileSortOrder = SortOrders[map];
                                                     return map.GetTile(tilePosition);
                                                 })
                                         .First();

            ITileTypeOverride typeOverride =
                tilemap.GetInstantiatedObject(tilePosition)?.GetComponent<ITileTypeOverride>();

            return typeOverride?.GetOverride(this, tilemap, tile, tilePosition, tileSortOrder) ?? tileData[tile];
        }

        /// <summary>
        /// Get the attached gameobject of the tile that is at the given position on the layer directly below a specific sort order.
        /// </summary>
        /// <param name="tilePosition">Position to check.</param>
        /// <param name="sortOrder">Sort order to check.</param>
        /// <returns>The attached gameobject or null if it doesn't have one.</returns>
        public GameObject GetGameObjectOfTileDirectlyBelowSortOrder(Vector3Int tilePosition, int sortOrder)
        {
            List<Tilemap> availableMaps = Tilemaps
                                         .Where(map => SortOrders[map] < sortOrder
                                                    && map.HasTile(tilePosition))
                                         .OrderByDescending(map => SortOrders[map])
                                         .ToList();

            return availableMaps.Count == 0 ? null : availableMaps.First().GetInstantiatedObject(tilePosition);
        }

        /// <summary>
        /// Get the tiles in a position.
        /// </summary>
        /// <param name="tilePosition">Position to get.</param>
        /// <returns>A list of the tiles in that position.</returns>
        public List<TileBase> GetTilesOnPosition(Vector3Int tilePosition)
        {
            List<TileBase> tiles = new();

            List<Tilemap> availableMaps = Tilemaps
                                         .Where(map => map.HasTile(tilePosition))
                                         .ToList();

            if (availableMaps.Count == 0) return tiles;

            tiles.AddRange(availableMaps.Select(map => map.GetTile(tilePosition)));

            return tiles;
        }

        /// <summary>
        /// Get the game objects attached to tiles in a position directly below the character.
        /// </summary>
        /// <param name="tilePosition">Position to get.</param>
        /// <param name="characterSortOrder">Sort order of the character.</param>
        /// <returns>A list of the game objects in that position.</returns>
        public GameObject GetAttachedTileObjectOnPositionDirectlyBelowPlayer(Vector3Int tilePosition,
                                                                             int characterSortOrder)
        {
            List<Tilemap> availableMaps = Tilemaps
                                         .Where(map => SortOrders[map] < characterSortOrder
                                                    && map.HasTile(tilePosition))
                                         .OrderByDescending(map => SortOrders[map])
                                         .ToList();

            return availableMaps.Count == 0 ? null : availableMaps.First().GetInstantiatedObject(tilePosition);
        }

        /// <summary>
        /// Get the game objects attached to tiles in a position.
        /// </summary>
        /// <param name="tilePosition">Position to get.</param>
        /// <returns>A list of the game objects in that position.</returns>
        public List<GameObject> GetAttachedTileObjectsOnPosition(Vector3Int tilePosition)
        {
            List<GameObject> tiles = new();

            List<Tilemap> availableMaps = Tilemaps
                                         .Where(map => map.HasTile(tilePosition))
                                         .ToList();

            if (availableMaps.Count == 0) return tiles;

            tiles.AddRange(availableMaps.Select(map => map.GetInstantiatedObject(tilePosition)));

            return tiles;
        }

        /// <summary>
        /// Check if there are no other characters or items on a tile.
        /// </summary>
        /// <param name="tilePosition">Tile to check.</param>
        /// <param name="ignorePlayer">Ignore the player and treat them as if they are not an obstacle.</param>
        /// <returns>True if there aren't.</returns>
        public bool IsTileFree(Vector3Int tilePosition, bool ignorePlayer = false) =>
            !AreThereCharactersInTile(tilePosition, ignorePlayer) && !AreThereBlockingActorsInTile(tilePosition);

        /// <summary>
        /// Check if there are any characters on a tile.
        /// </summary>
        /// <param name="tilePosition">Tile to check.</param>
        /// <param name="ignorePlayer">Ignore the player and treat them as if they are not an obstacle.</param>
        /// <returns>True if there are.</returns>
        private bool AreThereCharactersInTile(Vector3Int tilePosition, bool ignorePlayer = false) =>
            characters.Any(character =>
                           {
                               Actor attachedActor = character.GetCachedComponent<Actor>();

                               return character.Transform.position.ToInts() == tilePosition
                                   && (!ignorePlayer || !character.IsPlayer)
                                   && (attachedActor == null || attachedActor.BlocksMovement);
                           });

        /// <summary>
        /// Check if there are actors on a tile that block movement.
        /// </summary>
        /// <param name="tilePosition">Tile to check.</param>
        /// <returns>True if there are.</returns>
        private bool AreThereBlockingActorsInTile(Vector3Int tilePosition) =>
            actors.Any(actor => actor.BlocksMovement && actor.Transform.position.ToInts() == tilePosition);

        /// <summary>
        /// Does the given tile have blocking interactions?
        /// </summary>
        public bool HasBlockingInteraction(Vector3Int tilePosition) => blockingInteractions[tilePosition];

        /// <summary>
        /// Called when the player entered the grid.
        /// </summary>
        public void PlayerEnterGrid(PlayerCharacter player, bool forceReenter = false)
        {
            Logger.Info("Player entered " + SceneInfo.Scene.SceneName + ".");
            player.ApplySceneFX(SceneInfo);

            OutOfBattleWeather weatherToSet;

            if (SceneInfo.HasDefaultWeather)
                weatherToSet = forceReenter ? player.CurrentWeather : SceneInfo.DefaultWeather;
            else
            {
                if (forceReenter && SceneInfo.AllowedWeathers.Contains(player.CurrentWeather))
                    weatherToSet = player.CurrentWeather;
                else if (SceneInfo.AllowedWeathers.Contains(SceneInfo.DefaultWeather))
                    weatherToSet = SceneInfo.DefaultWeather;
                else
                    weatherToSet = null;
            }

            CoroutineRunner.RunRoutine(player.UpdateWeather(weatherToSet,
                                                            forceReenter));

            PlayerEnteredGrid?.Invoke(this, player, forceReenter);
        }

        /// <summary>
        /// Called when the player exited the grid.
        /// </summary>
        public void PlayerExitGrid(PlayerCharacter player)
        {
            Logger.Info("Player exited " + SceneInfo.Scene.SceneName + ".");
            PlayerExitedGrid?.Invoke(this);
        }

        /// <summary>
        /// Called by actors to enter the grid.
        /// </summary>
        /// <param name="actor">Actor entering.</param>
        public void ActorEnterGrid(GridSubscribingActor actor)
        {
            if (!actors.Contains(actor)) actors.Add(actor);
        }

        /// <summary>
        /// Called by actors to exit the grid.
        /// </summary>
        /// <param name="actor">Actor exiting.</param>
        public void ActorExitGrid(GridSubscribingActor actor)
        {
            if (actors.Contains(actor)) actors.Remove(actor);
        }

        /// <summary>
        /// Called by a character controller when it is about to enter a tile.
        /// </summary>
        /// <param name="characterController">Character that entered the tile.</param>
        /// <param name="tilePosition">Position of the entered tile.</param>
        public void CharacterAboutToEnterTileAsync(CharacterController characterController, Vector3Int tilePosition)
        {
            foreach (ICharacterInteractingTile interactor in
                     GetEnumerableOfCharacterInteractingTilesInPosition(tilePosition))
                StartCoroutine(interactor.CharacterAboutToEnterTileAsync(characterController));
        }

        /// <summary>
        /// Called by a character controller when it enters a tile.
        /// </summary>
        /// <param name="characterController">Character that entered the tile.</param>
        /// <param name="tilePosition">Position of the entered tile.</param>
        public void CharacterEnterTileAsync(CharacterController characterController, Vector3Int tilePosition)
        {
            if (!characters.Contains(characterController)) characters.Add(characterController);

            foreach (ICharacterInteractingTile interactor in
                     GetEnumerableOfCharacterInteractingTilesInPosition(tilePosition))
                StartCoroutine(interactor.CharacterEnterTileAsync(characterController));
        }

        /// <summary>
        /// Called by a character controller when it is about to leave a tile.
        /// </summary>
        /// <param name="characterController">Character that left the tile.</param>
        /// <param name="tilePosition">Position of the left tile.</param>
        public void CharacterAboutToLeaveTileAsync(CharacterController characterController, Vector3Int tilePosition)
        {
            foreach (ICharacterInteractingTile interactor in
                     GetEnumerableOfCharacterInteractingTilesInPosition(tilePosition))
                StartCoroutine(interactor.CharacterAboutToLeaveTileAsync(characterController));
        }

        /// <summary>
        /// Called by a character controller when it leaves a tile.
        /// </summary>
        /// <param name="characterController">Character that entered the tile.</param>
        /// <param name="tilePosition">Position of the left tile.</param>
        public void CharacterLeftTileAsync(CharacterController characterController, Vector3Int tilePosition)
        {
            foreach (ICharacterInteractingTile interactor in
                     GetEnumerableOfCharacterInteractingTilesInPosition(tilePosition))
                StartCoroutine(interactor.CharacterLeftTileAsync(characterController));

            if (characters.Contains(characterController)) characters.Remove(characterController);
        }

        /// <summary>
        /// Remove a character from the grid without any left tile callbacks.
        /// Useful when destroying characters.
        /// </summary>
        /// <param name="characterController">Character to remove.</param>
        public void RemoveCharacterFromGridWithoutLeftCallbacks(CharacterController characterController)
        {
            if (characters.Contains(characterController)) characters.Remove(characterController);
        }

        /// <summary>
        /// Called by a character controller when it is about to enter a tile.
        /// </summary>
        /// <param name="characterController">Character that entered the tile.</param>
        /// <param name="tilePosition">Position of the entered tile.</param>
        /// <param name="finished">Callback used to determine if the character can still move to that position and if wild encounters will still be triggered.</param>
        public IEnumerator CharacterAboutToEnterTile(CharacterController characterController,
                                                     Vector3Int tilePosition,
                                                     Action<bool, bool> finished)
        {
            bool move = true;
            bool triggerWilds = true;

            foreach (ICharacterInteractingTile interactor in
                     GetEnumerableOfCharacterInteractingTilesInPosition(tilePosition))
                yield return interactor.CharacterAboutToEnterTile(characterController,
                                                                  (keepMoving, keepTriggering) =>
                                                                  {
                                                                      move &= keepMoving;
                                                                      triggerWilds &= keepTriggering;
                                                                  });

            finished.Invoke(move, triggerWilds);
        }

        /// <summary>
        /// Called by a character controller when it enters a tile.
        /// </summary>
        /// <param name="characterController">Character that entered the tile.</param>
        /// <param name="tilePosition">Position of the entered tile.</param>
        /// <param name="finished">Callback used to determine if wild encounters will still be triggered.</param>
        public IEnumerator CharacterEnterTile(CharacterController characterController,
                                              Vector3Int tilePosition,
                                              Action<bool> finished)
        {
            bool triggerWilds = true;

            foreach (ICharacterInteractingTile interactor in
                     GetEnumerableOfCharacterInteractingTilesInPosition(tilePosition))
                yield return interactor.CharacterEnterTile(characterController,
                                                           keepTriggering => triggerWilds &= keepTriggering);

            finished.Invoke(triggerWilds);
        }

        /// <summary>
        /// Called by a character controller when it is about to leave a tile.
        /// </summary>
        /// <param name="characterController">Character that left the tile.</param>
        /// <param name="tilePosition">Position of the left tile.</param>
        /// <param name="finished">Callback used to determine if wild encounters will still be triggered.</param>
        public IEnumerator CharacterAboutToLeaveTile(CharacterController characterController,
                                                     Vector3Int tilePosition,
                                                     Action<bool> finished)
        {
            bool triggerWilds = true;

            foreach (ICharacterInteractingTile interactor in
                     GetEnumerableOfCharacterInteractingTilesInPosition(tilePosition))
                yield return interactor.CharacterAboutToLeaveTile(characterController,
                                                                  keepTriggering => triggerWilds &= keepTriggering);

            finished.Invoke(triggerWilds);
        }

        /// <summary>
        /// Called by a character controller when it leaves a tile.
        /// </summary>
        /// <param name="characterController">Character that entered the tile.</param>
        /// <param name="tilePosition">Position of the left tile.</param>
        /// <param name="finished">Callback used to determine if wild encounters will still be triggered.</param>
        public IEnumerator CharacterLeftTile(CharacterController characterController,
                                             Vector3Int tilePosition,
                                             Action<bool> finished)
        {
            bool triggerWilds = true;

            foreach (ICharacterInteractingTile interactor in
                     GetEnumerableOfCharacterInteractingTilesInPosition(tilePosition))
                yield return interactor.CharacterLeftTile(characterController,
                                                          keepTriggering => triggerWilds &= keepTriggering);

            finished.Invoke(triggerWilds);
        }

        /// <summary>
        /// Get an enumerable of the tiles in a position that have interaction with a character when it enters them.
        /// </summary>
        /// <param name="position">Position to check.</param>
        /// <returns>An enumerable object with those tiles.</returns>
        private IEnumerable<ICharacterInteractingTile> GetEnumerableOfCharacterInteractingTilesInPosition(
            Vector3Int position)
        {
            if (cachedCharacterInteractingTiles.TryGetValue(position, out List<ICharacterInteractingTile> inPosition))
                return inPosition;

            cachedCharacterInteractingTiles[position] = new List<ICharacterInteractingTile>();
            blockingInteractions[position] = false;

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (GameObject tile in GetAttachedTileObjectsOnPosition(position))
            {
                if (tile == null) continue;

                foreach (ICharacterInteractingTile characterInteractingTile in tile
                            .GetComponentsInChildren<ICharacterInteractingTile>())
                {
                    cachedCharacterInteractingTiles[position].Add(characterInteractingTile);
                    blockingInteractions[position] |= characterInteractingTile.HasBlockingInteraction();
                }
            }

            return cachedCharacterInteractingTiles[position];
        }

        /// <summary>
        /// Get a list of the actors in a position that the player can interact with.
        /// </summary>
        /// <param name="position">Position to check.</param>
        /// <returns>A list with those actors.</returns>
        public List<IPlayerInteractable>
            GetListOfPlayerInteractableActorsInPosition(Vector3Int position)
        {
            List<IPlayerInteractable> interactables = new();

            interactables.AddRange(characters.Where(character => character.Transform.position.ToInts() == position)
                                             .Select(character => character.GetCachedComponent<IPlayerInteractable>())
                                             .Where(interactable =>
                                                        interactable != null && !interactable.InteractsWhenOnTop()));

            interactables.AddRange(actors.Where(character => character.Transform.position.ToInts() == position)
                                         .Select(character => character.GetCachedComponent<IPlayerInteractable>())
                                         .Where(interactable =>
                                                    interactable != null && !interactable.InteractsWhenOnTop()));

            return interactables;
        }

        /// <summary>
        /// Get a list of the actors in a position that the player can interact with when they're on top of them.
        /// </summary>
        /// <param name="position">Position to check.</param>
        /// <returns>A list with those actors.</returns>
        public List<IPlayerInteractable>
            GetListOfPlayerInteractableActorsInPositionWhenOnTop(Vector3Int position)
        {
            List<IPlayerInteractable> interactables = new();

            interactables.AddRange(characters.Where(character => character.Transform.position.ToInts() == position)
                                             .Select(character => character.GetCachedComponent<IPlayerInteractable>())
                                             .Where(interactable =>
                                                        interactable != null && interactable.InteractsWhenOnTop()));

            interactables.AddRange(actors.Where(character => character.Transform.position.ToInts() == position)
                                         .Select(character => character.GetCachedComponent<IPlayerInteractable>())
                                         .Where(interactable =>
                                                    interactable != null && interactable.InteractsWhenOnTop()));

            return interactables;
        }

        /// <summary>
        /// Get a list of the given actor types in the given position.
        /// </summary>
        /// <param name="position">Position to check.</param>
        /// <typeparam name="T">Type to check.</typeparam>
        /// <returns>A list of those actors.</returns>
        public List<T> GetListOfActorTypesInPosition<T>(Vector3Int position)
        {
            List<T> interactables = new();

            // ReSharper disable twice CompareNonConstrainedGenericWithNull

            interactables.AddRange(characters.Where(character => character.Transform.position.ToInts() == position)
                                             .Select(character => character.GetCachedComponent<T>())
                                             .Where(interactable => interactable != null));

            interactables.AddRange(actors.Where(character => character.Transform.position.ToInts() == position)
                                         .Select(character => character.GetCachedComponent<T>())
                                         .Where(interactable => interactable != null));

            return interactables;
        }
    }
}