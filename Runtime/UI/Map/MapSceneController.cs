using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Badges;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dex;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.World;
using Varguiniano.YAPU.Runtime.World.Encounters;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime.Ui;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Map
{
    /// <summary>
    /// Controller for the map scene.
    /// </summary>
    public class MapSceneController : WhateverBehaviour<MapSceneController>, IInputReceiver
    {
        /// <summary>
        /// Cursor speed.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private float CursorSpeed;

        /// <summary>
        /// Cursor X limits.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private Vector2 CursorXLimits = new(-2.05f, 2.05f);

        /// <summary>
        /// Cursor Y limits.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private Vector2 CursorYLimits = new(-1.15f, 1.15f);

        /// <summary>
        /// Reference to the select audio.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private AudioReference SelectAudio;

        /// <summary>
        /// Reference to the map renderer.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private SpriteRenderer MapRenderer;

        /// <summary>
        /// Reference to the cursor.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform Cursor;

        /// <summary>
        /// Reference to the location name.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private LocalizedTextMeshPro LocationName;

        /// <summary>
        /// Reference to the tip to tell the player they can fly to a location.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private HidableUiElement FlyTip;

        /// <summary>
        /// Reference to the player map icon.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private PlayerMapIcon PlayerMapIcon;

        /// <summary>
        /// Reference to the move fly.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Move Fly;

        /// <summary>
        /// Reference to the encounter info panel.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private MapEncounterInfo MapEncounterInfo;

        /// <summary>
        /// Reference to the map scene launcher.
        /// </summary>
        [Inject]
        private MapSceneLauncher mapSceneLauncher;

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
        /// Reference to the player teleporter.
        /// </summary>
        [Inject]
        private PlayerTeleporter playerTeleporter;

        /// <summary>
        /// Reference to the global grid manager.
        /// </summary>
        [Inject]
        private GlobalGridManager globalGridManager;

        /// <summary>
        /// Reference to the player character.
        /// </summary>
        private PlayerCharacter playerCharacterReference;

        /// <summary>
        /// Species to display.
        /// </summary>
        private MonsterEntry monsterSpecies;

        /// <summary>
        /// Form to display.
        /// </summary>
        private Form form;

        /// <summary>
        /// Encounters that can be found in this map.
        /// </summary>
        private Dictionary<MapLocation, EncounterSetDexData> mapEncounters;

        /// <summary>
        /// Current region to display.
        /// </summary>
        private Region currentRegion;

        /// <summary>
        /// Map location list for the current map.
        /// </summary>
        private MapLocationList currentLocationList;

        /// <summary>
        /// Current location in the map.
        /// </summary>
        private MapLocation CurrentLocation => currentLocations.LastOrDefault();

        /// <summary>
        /// All locations that the cursor is currently over.
        /// </summary>
        private readonly List<MapLocation> currentLocations = new();

        /// <summary>
        /// Cached copy of the last navigation input.
        /// </summary>
        private Vector2 lastNavigationInput;

        /// <summary>
        /// Is the player holding navigation?
        /// </summary>
        private bool holdingNavigation;

        /// <summary>
        /// Should navigation be allowed?
        /// </summary>
        private bool allowNavigation;

        /// <summary>
        /// Flag storing if the player can fly.
        /// </summary>
        private bool canPlayerFly;

        /// <summary>
        /// Can the player fly to the current location?
        /// </summary>
        private bool canPlayerFlyToCurrentLocation;

        /// <summary>
        /// Cached reference to the location name transform.
        /// </summary>
        private Transform locationNameTransform;

        /// <summary>
        /// Initialize the map scene.
        /// </summary>
        private void OnEnable() => StartCoroutine(Initialize());

        /// <summary>
        /// Release the input.
        /// </summary>
        private void OnDisable() => inputManager.ReleaseInput(this);

        /// <summary>
        /// Initialize the map scene.
        /// </summary>
        /// <returns></returns>
        private IEnumerator Initialize()
        {
            locationNameTransform = LocationName.transform;

            playerCharacterReference = mapSceneLauncher.GetPlayerCharacter();

            List<(SceneInfoAsset, EncounterType, EncounterSetDexData)> allEncounters;

            ((monsterSpecies, form), allEncounters) = mapSceneLauncher.GetEncounters();

            currentRegion = allEncounters == null
                                ? playerCharacterReference.Region
                                : allEncounters.First().Item1.Region;

            bool playerInSameRegion = currentRegion == playerCharacterReference.Region;

            canPlayerFly = playerInSameRegion
                        && playerCharacterReference.Scene.CanFlyFromHere
                        && playerCharacterReference.PlayerRoster.AnyHasMove(Fly);

            // If the player can fly make sure the move is not locked in this region.
            if (canPlayerFly && currentRegion.IsMoveLockedByBadge(Fly, out Badge badge))
                canPlayerFly = playerCharacterReference.GlobalGameData.HasBadge(badge, currentRegion);

            // TODO: Flying taxi?

            MapRenderer.sprite = currentRegion.MapSprite;

            SetUIAsNoLocationSelected();

            Cursor.GetComponent<MapCursorLocationDetector>().EnteredLocation += OnCursorEnterLocation;
            Cursor.GetComponent<MapCursorLocationDetector>().ExitedLocation += OnCursorExitLocation;

            currentLocationList = Instantiate(currentRegion.MapLocationList, transform);

            yield return WaitAFrame;

            FilterAndStoreEncounterLocations(allEncounters);

            SceneInfoAsset currentObjective = mapSceneLauncher.GetCurrentQuestObjective();

            // Set up fly icons and current objective.
            foreach (MapLocation location in currentLocationList.MapLocations)
            {
                location.CanPlayerFlyHere = CanPlayerFlyToLocation(location);

                location.IsCurrentObjectiveHere =
                    currentObjective != null && location.IsSceneInLocation(currentObjective);
            }

            PlayerMapIcon.SetPlayer(playerCharacterReference.CharacterController.GetCharacterData(),
                                    currentLocationList.FindLocation(playerCharacterReference.Scene.Asset));

            yield return TransitionManager.BlackScreenFadeOutRoutine(.1f);
            DialogManager.ShowLoadingIcon(false);

            inputManager.BlockInput(false);
            inputManager.RequestInput(this);
        }

        /// <summary>
        /// Filter encounter locations so that we only use one for each map location.
        /// </summary>
        private void FilterAndStoreEncounterLocations(
            List<(SceneInfoAsset, EncounterType, EncounterSetDexData)> allEncounters)
        {
            if (allEncounters == null)
            {
                mapEncounters = null;
                return;
            }

            Dictionary<MapLocation, List<EncounterSetDexData>> uncompressedEncounters = new();

            foreach ((SceneInfoAsset Scene, EncounterType Type, EncounterSetDexData Data) encounter in allEncounters)
            {
                MapLocation location = currentLocationList.FindLocation(encounter.Scene);

                location.HasEncounter = true;

                if (!uncompressedEncounters.ContainsKey(location))
                    uncompressedEncounters[location] = new List<EncounterSetDexData>();

                uncompressedEncounters[location].Add(encounter.Data);
            }

            mapEncounters = new Dictionary<MapLocation, EncounterSetDexData>();

            foreach (KeyValuePair<MapLocation, List<EncounterSetDexData>> uncompressedEncounter in
                     uncompressedEncounters)
                mapEncounters[uncompressedEncounter.Key] = uncompressedEncounter.Value.MergeIntoOne();
        }

        /// <summary>
        /// Called when the cursor enters a location.
        /// </summary>
        /// <param name="location">Location entered.</param>
        private void OnCursorEnterLocation(MapLocation location)
        {
            currentLocations.AddIfNew(location);

            LocationName.SetValue(CurrentLocation.FirstScene.LocalizableNameKey);

            locationNameTransform.DOScale(new Vector3(1.2f, 1.2f, 1), .1f)
                                 .OnComplete(() => locationNameTransform.DOScale(new Vector3(1f, 1f, 1), .1f));

            canPlayerFlyToCurrentLocation = CanPlayerFlyToLocation(CurrentLocation);

            FlyTip.Show(canPlayerFlyToCurrentLocation);

            bool encounterAvailable = mapEncounters != null && mapEncounters.ContainsKey(CurrentLocation);

            if (encounterAvailable) MapEncounterInfo.SetData(monsterSpecies, form, mapEncounters[CurrentLocation]);

            MapEncounterInfo.Show(encounterAvailable);
        }

        /// <summary>
        /// Called when the cursor exits a location.
        /// </summary>
        /// <param name="location">Location entered.</param>
        private void OnCursorExitLocation(MapLocation location)
        {
            currentLocations.Remove(location);

            if (CurrentLocation == null)
                SetUIAsNoLocationSelected();
            else
                OnCursorEnterLocation(CurrentLocation);
        }

        /// <summary>
        /// Can the player fly to the given location?
        /// </summary>
        private bool CanPlayerFlyToLocation(MapLocation mapLocation) =>
            canPlayerFly
         && mapLocation.Flyable
         && playerCharacterReference.GlobalGameData.VisitedScenes
                                    .Contains(mapLocation.FlyLocation.Scene);

        /// <summary>
        /// Set the UI as if the cursor is over no location.
        /// </summary>
        private void SetUIAsNoLocationSelected()
        {
            canPlayerFlyToCurrentLocation = false;

            LocationName.SetValue("");

            FlyTip.Show(false);
            MapEncounterInfo.Show(false);
        }

        /// <summary>
        /// Called when it starts receiving input.
        /// </summary>
        public void OnStateEnter() => allowNavigation = true;

        /// <summary>
        /// Called when it stops receiving input.
        /// </summary>
        public void OnStateExit() => allowNavigation = false;

        /// <summary>
        /// Move the cursor.
        /// </summary>
        public void OnNavigation(InputAction.CallbackContext context) =>
            holdingNavigation = context switch
            {
                { phase: InputActionPhase.Started } => true,
                { phase: InputActionPhase.Canceled } => false,
                _ => holdingNavigation
            };

        /// <summary>
        /// Navigation when holding.
        /// </summary>
        private void Update()
        {
            if (!allowNavigation) return;

            lastNavigationInput = inputManager.GetCurrentNavigationVector();

            if (!holdingNavigation) return;

            Vector3 newCursorPosition = Cursor.position;
            newCursorPosition += new Vector3(lastNavigationInput.x, lastNavigationInput.y) * CursorSpeed;
            newCursorPosition.x = Mathf.Clamp(newCursorPosition.x, CursorXLimits.x, CursorXLimits.y);
            newCursorPosition.y = Mathf.Clamp(newCursorPosition.y, CursorYLimits.x, CursorYLimits.y);
            Cursor.position = newCursorPosition;
        }

        /// <summary>
        /// Fly to the location if it can fly.
        /// </summary>
        public void OnSelect(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started || !canPlayerFlyToCurrentLocation) return;

            audioManager.PlayAudio(SelectAudio);

            DialogManager.ShowChoiceMenu(new List<string>
                                         {
                                             "Common/True",
                                             "Common/False"
                                         },
                                         choice =>
                                         {
                                             if (choice == 0) CoroutineRunner.RunRoutine(FlyToCurrentLocation());
                                         },
                                         onBackCallback: () =>
                                                         {
                                                         },
                                         showDialog: true,
                                         localizationKey: "Map/Fly/ChoiceDialog",
                                         modifiers: CurrentLocation.FlyLocation.Scene.LocalizableNameKey);
        }

        /// <summary>
        /// Fly to the current location.
        /// </summary>
        private IEnumerator FlyToCurrentLocation()
        {
            SceneLocation location = CurrentLocation.FlyLocation;
            yield return DialogManager.CloseMenus();
            yield return playerTeleporter.TeleportPlayer(location);
            globalGridManager.StopAllActors = false;
        }

        /// <summary>
        /// Close the map.
        /// </summary>
        public void OnBack(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            CoroutineRunner.RunRoutine(CloseMap());
        }

        /// <summary>
        /// Close the map.
        /// </summary>
        private IEnumerator CloseMap()
        {
            audioManager.PlayAudio(SelectAudio);

            inputManager.ReleaseInput(this);
            inputManager.BlockInput();

            DialogManager.ShowLoadingIcon();
            yield return TransitionManager.BlackScreenFadeInRoutine(.1f);

            yield return mapSceneLauncher.CloseMap();
        }

        /// <summary>
        /// This scene will use the UI input.
        /// </summary>
        /// <returns></returns>
        public InputType GetInputType() => InputType.UI;

        /// <summary>
        /// Debug name for input.
        /// </summary>
        public string GetDebugName() => name;

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
    }
}