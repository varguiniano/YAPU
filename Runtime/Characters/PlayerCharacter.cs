using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using Varguiniano.YAPU.Runtime.Actors;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.Monster.Breeding;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Bags;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Other;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Natures;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.Quests;
using Varguiniano.YAPU.Runtime.Rendering.RenderFX;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.UI.Map;
using Varguiniano.YAPU.Runtime.World;
using Varguiniano.YAPU.Runtime.World.Encounters;
using Varguiniano.YAPU.Runtime.World.OutOfBattleWeathers;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Configuration;
using WhateverDevs.Core.Runtime.DependencyInjection;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;
using Random = UnityEngine.Random;

namespace Varguiniano.YAPU.Runtime.Characters
{
    /// <summary>
    /// Controller in charge of receiving the player input and performing the actions,
    /// like moving the character, interacting with stuff or opening the menu.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerCharacter : WhateverBehaviour<PlayerCharacter>, IInputReceiver, IPlayerDataReceiver
    {
        /// <summary>
        /// Reference to the FX controller.
        /// </summary>
        public PlayerCharacterFX FX;

        /// <summary>
        /// Reference to the player post processing.
        /// </summary>
        public PlayerCharacterPostProcessing PostProcessing;

        /// <summary>
        /// Reference to the surf move.
        /// </summary>
        [SerializeField]
        private Surf Surf;

        /// <summary>
        /// Reference to the Waterfall move.
        /// </summary>
        [SerializeField]
        private Waterfall Waterfall;

        /// <summary>
        /// Reference to the select audio.
        /// </summary>
        [SerializeField]
        private AudioReference SelectAudio;

        /// <summary>
        /// Reference to the attached CharacterController.
        /// </summary>
        public CharacterController CharacterController => GetCachedComponent<CharacterController>();

        /// <summary>
        /// Access to the scene the player is in.
        /// </summary>
        public SceneInfo Scene => CharacterController.CurrentGrid.SceneInfo;

        /// <summary>
        /// Access to the region the player is in.
        /// </summary>
        public Region Region => Scene.Region;

        /// <summary>
        /// Scene location object of the player's current location.
        /// </summary>
        public SceneLocation CurrentLocation =>
            new()
            {
                Scene = Scene.Asset,
                Location = new Vector2Int(Position.x,
                                          Position.y)
            };

        /// <summary>
        /// Reference to the player roster.
        /// </summary>
        [HideInInspector]
        [Inject]
        public Roster PlayerRoster;

        /// <summary>
        /// Reference to the player storage.
        /// </summary>
        [HideInInspector]
        [Inject]
        public MonsterStorage PlayerStorage;

        /// <summary>
        /// Reference to the player bag.
        /// </summary>
        [HideInInspector]
        [Inject]
        public Bag PlayerBag;

        /// <summary>
        /// Reference to the player dex.
        /// </summary>
        [HideInInspector]
        [Inject]
        public Dex PlayerDex;

        /// <summary>
        /// This actor's position.
        /// </summary>
        public Vector3Int Position => transform.position.ToInts();

        /// <summary>
        /// Flag to mark if the player can move heavy boulders.
        /// </summary>
        public bool Strong;

        /// <summary>
        /// Flag to mark if the player is using flash.
        /// </summary>
        public bool Flash;

        /// <summary>
        /// Current weather.
        /// </summary>
        [FoldoutGroup("Weather")]
        [ReadOnly]
        public OutOfBattleWeather CurrentWeather;

        /// <summary>
        /// List of weathers the player has already cleared, for example using defog.
        /// Only works until they teleport.
        /// </summary>
        [FoldoutGroup("Weather")]
        [ReadOnly]
        public List<OutOfBattleWeather> ClearedWeathers;

        /// <summary>
        /// Is the current weather cleared?
        /// </summary>
        public bool IsCurrentWeatherCleared => ClearedWeathers.Contains(CurrentWeather);

        /// <summary>
        /// Counter for repel effect.
        /// > 0  will repel wild monsters.
        /// </summary>
        [ShowInInspector]
        [ReadOnly]
        private uint repelCounter;

        /// <summary>
        /// Last item used for repelling.
        /// </summary>
        private Repel lastRepelItem;

        /// <summary>
        /// Reference to the input manager.
        /// </summary>
        [Inject]
        private IInputManager inputManager;

        /// <summary>
        /// Reference to the battle launcher.
        /// </summary>
        [Inject]
        private IBattleLauncher battleLauncher;

        /// <summary>
        /// Reference to the global grid manager.
        /// </summary>
        [Inject]
        [HideInInspector]
        public GlobalGridManager GlobalGridManager;

        /// <summary>
        /// Reference to the global game data.
        /// </summary>
        [Inject]
        [HideInInspector]
        public GlobalGameData GlobalGameData;

        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        [Inject]
        public YAPUSettings YAPUSettings;

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
        /// Reference to the teleporter.
        /// </summary>
        [Inject]
        private PlayerTeleporter teleporter;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Reference to the map scene launcher.
        /// </summary>
        [Inject]
        private MapSceneLauncher mapSceneLauncher;

        /// <summary>
        /// Reference to the hatching manager.
        /// </summary>
        [HideInInspector]
        [Inject]
        public HatchingManager HatchingManager;

        /// <summary>
        /// Reference to the time manager.
        /// </summary>
        [Inject]
        private TimeManager timeManager;

        /// <summary>
        /// Reference to the quest manager.
        /// </summary>
        [Inject]
        private QuestManager questManager;

        /// <summary>
        /// Flag to know when the movement is being held.
        /// </summary>
        public bool HoldingMovement => inputManager.IsHoldingPlayerMovement();

        /// <summary>
        /// Backfield for HoldingMovement.
        /// </summary>
        private bool holdingMovement;

        /// <summary>
        /// Routine in charge of managing holding the movement.
        /// </summary>
        private Coroutine movementRoutine;

        /// <summary>
        /// Last direction the player used.
        /// </summary>
        [HideInEditorMode]
        [ShowInInspector]
        [ReadOnly]
        private CharacterController.Direction lastUsedDirection = CharacterController.Direction.None;

        /// <summary>
        /// Last input the player used.
        /// </summary>
        [HideInEditorMode]
        [ShowInInspector]
        [ReadOnly]
        private Vector2 lastInput = Vector2.zero;

        /// <summary>
        /// Flag to know if it is already interacting.
        /// </summary>
        [HideInEditorMode]
        [ShowInInspector]
        [ReadOnly]
        private bool interacting;

        /// <summary>
        /// Flag to know when an entire move cycle is being performed.
        /// </summary>
        [HideInEditorMode]
        [ShowInInspector]
        [ReadOnly]
        private bool performingMovement;

        /// <summary>
        /// Cached copy of the gameplay configuration.
        /// </summary>
        private GameplayConfiguration gameplayConfiguration;

        /// <summary>
        /// Request input as soon as it is enabled.
        /// </summary>
        private void OnEnable()
        {
            if (!configurationManager.GetConfiguration(out gameplayConfiguration))
            {
                Logger.Error("Error retrieving the gameplay configuration!");
                return;
            }

            CharacterController.LookAt(CharacterController.Direction.Down,
                                       useBikingSprite: CharacterController.IsBiking);

            LockInput(); // Start with input locked until the teleporter says otherwise.
            RequestInput(null);

            // BUG: This shouldn't be necessary, why is the input manager getting stuck otherwise?
            battleLauncher.SubscribeToBattleLaunched(UnregisterFromInput);
            battleLauncher.SubscribeToBattleEnded(RequestInput);

            timeManager.OnDayEnded += OnDayEnded;

            PostProcessing.Init(this);
        }

        /// <summary>
        /// Unregister on disable.
        /// </summary>
        private void OnDisable()
        {
            // Clean up stuff that weather might have instantiated.
            if (CoroutineRunner.Instance != null)
                CoroutineRunner.RunRoutine(UpdateWeather(null, isDestroyingCharacter: true));

            battleLauncher.UnsubscribeFromBattleLaunched(UnregisterFromInput);
            battleLauncher.UnsubscribeFromBattleEnded(RequestInput);

            timeManager.OnDayEnded -= OnDayEnded;

            UnregisterFromInput();
        }

        /// <summary>
        /// Request input to the input manager.
        /// </summary>
        private void RequestInput(BattleResultParameters battleResultParameters) => inputManager.RequestInput(this);

        /// <summary>
        /// Unregister input manager.
        /// </summary>
        private void UnregisterFromInput() => inputManager.ReleaseInput(this);

        /// <summary>
        /// Called when it starts receiving input.
        /// </summary>
        public void OnStateEnter()
        {
            if (!configurationManager.GetConfiguration(out gameplayConfiguration))
            {
                Logger.Error("Error retrieving the gameplay configuration!");
                return;
            }

            if (gameplayConfiguration.RunningMode == GameplayConfiguration.RunMode.Hold
             && CharacterController.IsRunning)
                CharacterController.IsRunning = false;

            movementRoutine ??= StartCoroutine(MovementHoldRoutine());
        }

        /// <summary>
        /// Called when it stops receiving input.
        /// </summary>
        public void OnStateExit()
        {
        }

        /// <summary>
        /// Lock or unlock the player movement.
        /// </summary>
        /// <param name="lockMovement">Lock the movement?</param>
        public void LockInput(bool lockMovement = true) => interacting = lockMovement;

        /// <summary>
        /// Called to move the player character.
        /// </summary>
        public void OnMovement(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Canceled) return;

            lastInput = context.ReadValue<Vector2>();

            lastUsedDirection = InputToDirection(lastInput.x, lastInput.y);

            if (lastUsedDirection != CharacterController.Direction.None
             && context.phase == InputActionPhase.Started
             && !interacting
             && !CharacterController.IsMoving)
                CharacterController.LookAt(lastUsedDirection, useBikingSprite: CharacterController.IsBiking);
        }

        /// <summary>
        /// Routine to perform movement when the stick is held.
        /// This method has to be huge because every time you call yield return Unity skips a frame.
        /// Separating the movement into smaller methods would make the player move slower.
        /// </summary>
        private IEnumerator MovementHoldRoutine()
        {
            yield return new WaitWhile(() => interacting);

            yield return GlobalGridManager.PlayerMoved(this);

            while (true)
            {
                // This is checked before and after moving because there was a frame perfect chance of getting the player frozen when exiting a door after teleporting.
                if (CharacterController.IsMoving) yield return new WaitWhile(() => CharacterController.IsMoving);

                if (HoldingMovement)
                {
                    performingMovement = true;

                    lastInput = inputManager.GetCurrentMovementVector();

                    lastUsedDirection = InputToDirection(lastInput.x, lastInput.y);

                    CharacterController.LookAt(lastUsedDirection, useBikingSprite: CharacterController.IsBiking);

                    if (Strong) yield return TriggerPush();

                    Vector3Int targetPosition =
                        CharacterController.MoveOneInDirection(CharacterController.Transform.position.ToInts(),
                                                               CharacterController.CurrentDirection);

                    // Get the target grid and get the tile types.
                    if (!CharacterController.FindTargetGrid(targetPosition, out GridController targetGrid)) yield break;

                    List<TriggerActor> interactables =
                        targetGrid.GetListOfActorTypesInPosition<TriggerActor>(targetPosition);

                    bool dontMove = false;

                    // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                    foreach (TriggerActor interactable in interactables)
                        if (interactable.HasAboutToEnterTriggers)
                            yield return interactable.PlayerAboutToEnter(this,
                                                                         CharacterController.CurrentDirection.Invert(),
                                                                         stop => dontMove = stop);

                    bool triggerEncounter = false;
                    EncounterType encounterType = EncounterType.Other;

                    if (!dontMove)
                    {
                        yield return CharacterController.Move(lastUsedDirection,
                                                              playerCallback: (trigger, triggerEncounterType) =>
                                                                              {
                                                                                  triggerEncounter = trigger;
                                                                                  encounterType = triggerEncounterType;
                                                                              });

                        GlobalGameData.StepsTaken++;

                        FriendshipStepTick();

                        uint stepsPerEggCycle = YAPUSettings.StepsPerEggCycle;

                        foreach (MonsterInstance monsterInstance in PlayerRoster)
                            if (monsterInstance is {IsNullEntry: false, EggData: {IsEgg: false}})
                                stepsPerEggCycle =
                                    (uint) (stepsPerEggCycle * monsterInstance.ModifyStepsNeededForEggCycle());

                        if (GlobalGameData.StepsTaken % stepsPerEggCycle == 0) yield return EggCycle();

                        // This is a twisted way of triggering encounters but calling the trigger encounter routine from the moving routine made both of them get stuck.
                        // Perhaps from the original being called here then calling another class then this one, looks like a Unity bug.
                        // Also, no triggering encounters if the are triggers on that tile.
                        if (triggerEncounter && interactables.Count == 0) yield return TriggerEncounter(encounterType);

                        if (repelCounter > 0) yield return RepelStepTick();

                        yield return GlobalGridManager.PlayerMoved(this);
                    }

                    if (!HoldingMovement)
                        CharacterController.LookAt(lastUsedDirection, useBikingSprite: CharacterController.IsBiking);

                    // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                    foreach (TriggerActor interactable in interactables)
                        if (interactable.HasEnteredTriggers)
                            yield return interactable.PlayerEntered(this, CharacterController.CurrentDirection);

                    // We do this twice because if you are frame perfect you can release the movement while triggers are running and get the sprite in a weird state while running.
                    if (!HoldingMovement)
                        CharacterController.LookAt(lastUsedDirection, useBikingSprite: CharacterController.IsBiking);

                    performingMovement = false;
                }

                if (CharacterController.IsMoving)
                    yield return new WaitWhile(() => CharacterController.IsMoving);
                else if (interacting)
                    yield return new WaitWhile(() => interacting);
                else
                    yield return WaitAFrame;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        /// <summary>
        /// Depending on the config, enable running while this is pressed or enable/disable when pressed.
        /// </summary>
        public void OnRun(InputAction.CallbackContext context)
        {
            switch (gameplayConfiguration.RunningMode)
            {
                case GameplayConfiguration.RunMode.Toggle:

                    if (context.phase != InputActionPhase.Started) return;

                    CharacterController.IsRunning = !CharacterController.IsRunning;

                    break;
                case GameplayConfiguration.RunMode.Hold:

                    // ReSharper disable once ConvertSwitchStatementToSwitchExpression
                    // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                    switch (context.phase)
                    {
                        case InputActionPhase.Started:
                            CharacterController.IsRunning = true;
                            break;
                        case InputActionPhase.Canceled:
                            CharacterController.IsRunning = false;
                            break;
                    }

                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Called to interact with the world.
        /// </summary>
        public void OnInteract(InputAction.CallbackContext context)
        {
            if (!interacting) StartCoroutine(Interact());
        }

        /// <summary>
        /// Interact with a tile.
        /// </summary>
        private IEnumerator Interact()
        {
            interacting = true;

            yield return new WaitWhile(() => CharacterController.IsMoving);

            List<IPlayerInteractable> onTopInteractables =
                CharacterController.CurrentGrid.GetListOfPlayerInteractableActorsInPositionWhenOnTop(Position);

            foreach (IPlayerInteractable interactable in onTopInteractables)
                yield return interactable.Interact(this, CharacterController.CurrentDirection.Invert());

            Vector3Int targetPosition =
                CharacterController.MoveOneInDirection(CharacterController.Transform.position.ToInts(),
                                                       CharacterController.CurrentDirection);

            Logger.Info("Attempting to interact with tile: " + targetPosition + ".");

            // Get the target grid and get the tile types.
            if (!CharacterController.FindTargetGrid(targetPosition, out GridController targetGrid))
            {
                interacting = false;
                yield break;
            }

            List<IPlayerInteractable> interactables =
                targetGrid.GetListOfPlayerInteractableActorsInPosition(targetPosition);

            foreach (IPlayerInteractable interactable in interactables)
                yield return interactable.Interact(this, CharacterController.CurrentDirection.Invert());

            if (CharacterController.CanEnterWater(CharacterController.CurrentDirection)
             && PlayerRoster.AnyHasMove(Surf))
            {
                int choice = -1;

                audioManager.PlayAudio(SelectAudio);

                DialogManager.ShowChoiceMenu(new List<string>
                                             {
                                                 "Common/True",
                                                 "Common/False"
                                             },
                                             option => choice = option,
                                             onBackCallback: () => choice = 1,
                                             showDialog: true,
                                             localizationKey: "Moves/Surf/OutOfBattleUseChoice");

                yield return new WaitWhile(() => choice == -1);

                if (choice == 0)
                    foreach (MonsterInstance candidate in PlayerRoster)
                        if (candidate.KnowsMove(Surf))
                        {
                            yield return Surf.UseOutOfBattle(this, candidate, localizer, mapSceneLauncher);
                            break; // We only need to use it once.
                        }
            }

            if (CharacterController.CanClimbWaterfall()
             && PlayerRoster.AnyHasMove(Waterfall))
            {
                int choice = -1;

                audioManager.PlayAudio(SelectAudio);

                DialogManager.ShowChoiceMenu(new List<string>()
                                             {
                                                 "Common/True",
                                                 "Common/False"
                                             },
                                             option => choice = option,
                                             onBackCallback: () => choice = 1,
                                             showDialog: true,
                                             localizationKey: "Moves/Waterfall/OutOfBattleUseChoice");

                yield return new WaitWhile(() => choice == -1);

                if (choice == 0)
                    foreach (MonsterInstance candidate in PlayerRoster)
                        if (candidate.KnowsMove(Waterfall))
                        {
                            yield return Waterfall.UseOutOfBattle(this, candidate, localizer, mapSceneLauncher);
                            break; // We only need to use it once.
                        }
            }

            interacting = false;
        }

        /// <summary>
        /// If strong push objects before trying to move.
        /// </summary>
        private IEnumerator TriggerPush()
        {
            Vector3Int targetPosition =
                CharacterController.MoveOneInDirection(CharacterController.Transform.position.ToInts(),
                                                       CharacterController.CurrentDirection);

            // Get the target grid and get the tile types.
            if (!CharacterController.FindTargetGrid(targetPosition, out GridController targetGrid)) yield break;

            List<PushableActor> interactables = targetGrid.GetListOfActorTypesInPosition<PushableActor>(targetPosition);

            foreach (PushableActor interactable in interactables)
                yield return interactable.Push(this, CharacterController.CurrentDirection.Invert());
        }

        /// <summary>
        /// Check if the player can use a move on its current location.
        /// </summary>
        /// <param name="monster">Monster using the move.</param>
        /// <param name="move">The move being used.</param>
        /// <param name="targets">Available targets in that position</param>
        /// <returns>True if they can.</returns>
        public bool CanUseMove(MonsterInstance monster, Move move, out List<ActorMoveTarget> targets)
        {
            targets = new List<ActorMoveTarget>();

            List<ActorMoveTarget> onTopInteractables =
                CharacterController.CurrentGrid.GetListOfActorTypesInPosition<ActorMoveTarget>(Position);

            targets.AddRange(onTopInteractables.Where(interactable =>
                                                          interactable.InteractsWhenOnTop()
                                                       && interactable.IsMoveCompatible(move)));

            Vector3Int targetPosition =
                CharacterController.MoveOneInDirection(CharacterController.Transform.position.ToInts(),
                                                       CharacterController.CurrentDirection);

            // Get the target grid and get the tile types.
            if (!CharacterController.FindTargetGrid(targetPosition, out GridController targetGrid)) return false;

            List<ActorMoveTarget> interactables =
                targetGrid.GetListOfActorTypesInPosition<ActorMoveTarget>(targetPosition);

            targets.AddRange(interactables.Where(interactable =>
                                                     !interactable.InteractsWhenOnTop()
                                                  && interactable.IsMoveCompatible(move)));

            return targets.Count > 0;
        }

        /// <summary>
        /// Use a move out of battle.
        /// </summary>
        /// <param name="monster">Monster using the move.</param>
        /// <param name="move">The move being used.</param>
        /// <param name="targets">Targets to use the move on.</param>
        public IEnumerator UseMove(MonsterInstance monster, Move move, List<ActorMoveTarget> targets)
        {
            interacting = true;

            foreach (ActorMoveTarget target in targets)
                yield return target.UseMove(this, CharacterController.CurrentDirection.Invert(), monster, move);

            interacting = false;
        }

        /// <summary>
        /// Called to open the menu.
        /// </summary>
        public void OnMenu(InputAction.CallbackContext context)
        {
            // Get input only when key goes down.
            if (context.phase != InputActionPhase.Started) return;

            if (interacting) return;

            StartCoroutine(OpenMenu());
        }

        /// <summary>
        /// Called to open the menu.
        /// </summary>
        private IEnumerator OpenMenu()
        {
            interacting = true;

            GlobalGridManager.StopAllActors = true;

            yield return WaitAFrame;

            yield return new WaitWhile(() => CharacterController.IsMoving);

            List<bool> onButtons = new()
                                   {
                                       GlobalGameData.HasDex && PlayerRoster.RosterSize > 0, // Dex
                                       PlayerRoster.RosterSize > 0, // Mons
                                       PlayerRoster.RosterSize > 0, // Bag
                                       true, // Map
                                       questManager.GetAllQuests().Count > 0, // Quests
                                       true, // Profile
                                       true, // Options
                                       true // Exit
                                   };

            DialogManager.ShowGameMenu(onButtons,
                                       this,
                                       () =>
                                       {
                                           if (!configurationManager.GetConfiguration(out gameplayConfiguration))
                                           {
                                               Logger.Error("Error retrieving the gameplay configuration!");
                                               return;
                                           }

                                           GlobalGridManager.StopAllActors = false;

                                           interacting = false;
                                       });
        }

        /// <summary>
        /// Open the registered items if there are any when the player presses the extra key.
        /// </summary>
        public void OnExtra(InputAction.CallbackContext context)
        {
            // Get input only when key goes down.
            if (context.phase != InputActionPhase.Started) return;

            if (interacting || CharacterController.IsMoving) return;

            audioManager.PlayAudio(SelectAudio);

            DialogManager.ShowQuickAccessItems(this);
        }

        /// <summary>
        /// Trigger events when the player is about to enter a tile.
        /// </summary>
        /// <param name="targetGrid">Target grid of that tile.</param>
        /// <param name="targetPosition">Target position in that tile.</param>
        /// <param name="callback">Callback stating if movement should be blocked and the interactables found in that tile.</param>
        public IEnumerator TriggerPlayerAboutToEnterTile(GridController targetGrid,
                                                         Vector3Int targetPosition,
                                                         Action<bool, List<TriggerActor>> callback)
        {
            List<TriggerActor> interactables =
                targetGrid.GetListOfActorTypesInPosition<TriggerActor>(targetPosition);

            bool dontMove = false;

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (TriggerActor interactable in interactables)
                if (interactable.HasAboutToEnterTriggers)
                    yield return interactable.PlayerAboutToEnter(this,
                                                                 CharacterController.CurrentDirection.Invert(),
                                                                 stop => dontMove = stop);

            callback.Invoke(dontMove, interactables);
        }

        /// <summary>
        /// Trigger events when the player has entered a tile.
        /// </summary>
        /// <param name="interactables">Interactables on that tile.</param>
        public IEnumerator TriggerPlayerEnteredTile(List<TriggerActor> interactables)
        {
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (TriggerActor interactable in interactables)
                if (interactable.HasEnteredTriggers)
                    yield return interactable.PlayerEntered(this, CharacterController.CurrentDirection);
        }

        /// <summary>
        /// Trigger a wild encounter.
        /// </summary>
        /// <param name="encounterType">Type of encounter to trigger.</param>
        private IEnumerator TriggerEncounter(EncounterType encounterType)
        {
            LockInput();

            SceneInfo sceneInfo = CharacterController.CurrentGrid.SceneInfo;

            Logger.Info("Attempting to trigger encounter of type "
                      + encounterType
                      + " on scene "
                      + sceneInfo.Scene.SceneName
                      + ".");

            List<WildEncounter> possibleEncounters =
                sceneInfo.GetWildEncounters(encounterType, timeManager.DayMoment, CurrentWeather);

            PlayerRoster[0].ModifyPossibleEncounters(ref possibleEncounters, sceneInfo, encounterType);

            if (possibleEncounters.Count == 0)
            {
                Logger.Info("No available encounters.");

                LockInput(false);
                yield break;
            }

            WildEncounter encounter = possibleEncounters.Last();

            // The total chances are calculated by the sum of all weights.
            float roll = Random.Range(0, possibleEncounters.Sum(wildEncounter => wildEncounter.Weight));
            float accumChance = 0;

            Logger.Info("Wild encounter roll: " + roll);

            foreach (WildEncounter encounterCandidate in possibleEncounters)
            {
                if (roll < encounterCandidate.Weight + accumChance)
                {
                    encounter = encounterCandidate;
                    break;
                }

                accumChance += encounterCandidate.Weight;
            }

            (byte minimumLevel, byte maximumLevel) = PlayerRoster[0]
               .ModifyEncounterLevels(encounterType, encounter.MinLevel, encounter.MaxLevel);

            byte level = (byte) Random.Range(minimumLevel, maximumLevel + 1);

            Nature encounterNature = PlayerRoster[0].ModifyEncounterNature(encounterType);

            if (repelCounter > 0 && PlayerRoster.RosterSize > 0 && level < PlayerRoster[0].StatData.Level)
            {
                Logger.Info("Repel prevented an encounter.");
                LockInput(false);
                yield break;
            }

            if (PlayerRoster[0].ShouldPreventEncounter(encounterType, level))
            {
                Logger.Info("Party lead prevented an encounter.");
                LockInput(false);
                yield break;
            }

            GlobalGridManager.StopAllActors = true;

            // Fix running position.
            CharacterController.LookAt(CharacterController.CurrentDirection,
                                       useBikingSprite: CharacterController.IsBiking);

            battleLauncher.LaunchSingleWildEncounter(this,
                                                     encounter,
                                                     level,
                                                     encounterNature,
                                                     sceneInfo,
                                                     null,
                                                     encounterType,
                                                     result =>
                                                     {
                                                         if (result.PlayerWon)
                                                             CharacterController.CurrentGrid
                                                                .PlayerEnterGrid(this, true);

                                                         Logger.Info("Battle ended.");

                                                         GlobalGridManager.StopAllActors = false;

                                                         LockInput(false);
                                                     });

            yield return new WaitWhile(() => battleLauncher.BattleInProgress);
        }

        /// <summary>
        /// Tick a friendship boost when taking certain steps.
        /// </summary>
        private void FriendshipStepTick()
        {
            if (GlobalGameData.StepsTaken % YAPUSettings.FriendshipTickSteps != 0) return;

            if (Random.value > YAPUSettings.FriendshipTickChance) return;

            foreach (MonsterInstance monster in PlayerRoster)
                if (monster is {IsNullEntry: false})
                    monster.ChangeFriendship(monster.Friendship < 200 ? 2 : 1, Scene.Asset, localizer);
        }

        /// <summary>
        /// Egg cycle triggered each 256 steps.
        /// </summary>
        private IEnumerator EggCycle()
        {
            Logger.Info("Triggering egg cycle.");

            foreach (MonsterInstance monster in PlayerRoster)
                if (monster is {IsNullEntry: false, EggData: {IsEgg: true}})
                    monster.EggData.EggCyclesLeft--;

            yield return HatchingManager.TriggerHatchingAfterCycle(PlayerRoster.RosterData);

            CharacterController.LookAt(CharacterController.CurrentDirection,
                                       useBikingSprite: CharacterController.IsBiking);
        }

        /// <summary>
        /// Tick the repel counter every step.
        /// </summary>
        private IEnumerator RepelStepTick()
        {
            if (repelCounter == 0) yield break;

            repelCounter--;

            if (repelCounter != 0) yield break;

            yield return DialogManager.ShowDialogAndWait("Items/Repel/WoreOff");

            if (lastRepelItem == null || !PlayerBag.Contains(lastRepelItem)) yield break;

            int choice = -1;

            DialogManager.ShowChoiceMenu(new List<string>
                                         {
                                             "Common/True",
                                             "Common/False"
                                         },
                                         playerChoice => choice = playerChoice,
                                         onBackCallback: () => choice = 1,
                                         showDialog: true,
                                         localizationKey: "Items/Repel/UseAnother");

            yield return new WaitWhile(() => choice == -1);

            if (choice != 0) yield break;

            yield return AddRepelSteps(lastRepelItem);

            PlayerBag.ChangeItemAmount(lastRepelItem, -1);
        }

        /// <summary>
        /// Method called each time the day ends.
        /// </summary>
        private void OnDayEnded() => PlayerRoster.TriggerVirusSpread();

        /// <summary>
        /// Apply the FX of the given scene.
        /// </summary>
        /// <param name="sceneInfo">Scene to apply the modifiers from.</param>
        public void ApplySceneFX(SceneInfo sceneInfo)
        {
            if (sceneInfo.IsDungeon && sceneInfo.IsDark)
                PostProcessing.EnableDarkCave(Flash);
            else
                PostProcessing.DisableDarkCave();
        }

        /// <summary>
        /// Clear the current weather.
        /// </summary>
        public IEnumerator ClearWeather()
        {
            if (CurrentWeather != null) ClearedWeathers.Add(CurrentWeather);
            yield return UpdateWeather(null);
        }

        /// <summary>
        /// Update the current weather.
        /// </summary>
        /// <param name="weather">New weather.</param>
        /// <param name="force">Force updating even if it is the same?</param>
        /// <param name="isDestroyingCharacter">Is the character being destroyed?</param>
        /// <param name="ignoredCleared">Ignore the cleared weathers and set anyway?</param>
        public IEnumerator UpdateWeather(OutOfBattleWeather weather,
                                         bool force = false,
                                         bool isDestroyingCharacter = false,
                                         bool ignoredCleared = false)
        {
            if (weather == CurrentWeather && !force) yield break;

            if (ClearedWeathers.Contains(weather))
            {
                if (ignoredCleared)
                    ClearedWeathers.Remove(weather);
                else
                    weather = null;
            }

            if (CurrentWeather != null) yield return CurrentWeather.EndWeather(this, isDestroyingCharacter);

            CurrentWeather = weather;

            if (CurrentWeather != null) yield return CurrentWeather.StartWeather(this);
        }

        /// <summary>
        /// fish on the tile in front if able.
        /// </summary>
        /// <param name="level">Fishing level. Encounters depend on this.</param>
        public IEnumerator Fish(int level)
        {
            audioManager.PlayAudio(SelectAudio);

            if (!Scene.CanFishHere
             || !CharacterController.CanMoveToNextTile(CharacterController.CurrentDirection,
                                                       true,
                                                       false,
                                                       out TileType _,
                                                       out TileType targetType,
                                                       out GameObject _)
             || targetType != TileType.Water)
            {
                yield return DialogManager.ShowDialogAndWait("Fishing/CantFishHere");
                yield break;
            }

            CharacterController.IsFishing = true;

            CharacterController.LookAt(CharacterController.CurrentDirection,
                                       useBikingSprite: CharacterController.IsBiking);

            StringBuilder fishingDots = new();

            for (int i = 0; i < Random.Range(10, 40); i++) fishingDots.Append(". ");

            DialogManager.ShowDialog(fishingDots.ToString(), acceptInput: false, typewriterSpeed: .02f);

            yield return DialogManager.WaitForTypewriter;

            DialogManager.NextDialog();

            audioManager.PlayAudio(SelectAudio);

            yield return DialogManager.ShowDialogAndWait("Fishing/MonsterBit");

            CharacterController.IsFishing = false;

            CharacterController.LookAt(CharacterController.CurrentDirection,
                                       useBikingSprite: CharacterController.IsBiking);

            yield return TriggerEncounter(level switch
            {
                0 => EncounterType.FishingOldRod, 1 => EncounterType.FishingGoodRod, _ => EncounterType.FishingSuperRod
            });
        }

        /// <summary>
        /// Use flash to light the current dungeon.
        /// </summary>
        public IEnumerator UseFlash()
        {
            Flash = true;
            yield return PostProcessing.LitDarkCave();
        }

        /// <summary>
        /// Escape the current dungeon.
        /// </summary>
        public IEnumerator EscapeDungeon()
        {
            if (!Scene.IsDungeon)
            {
                Logger.Error("Current scene is not a dungeon.");
                yield break;
            }

            yield return teleporter.TeleportPlayer(Scene.EscapeRopePosition);
        }

        /// <summary>
        /// Teleport back to the last healing location.
        /// </summary>
        public IEnumerator TeleportBackToLastHeal()
        {
            yield return teleporter.TeleportPlayer(GlobalGameData.LastHealLocation);
        }

        /// <summary>
        /// Climb to the target position.
        /// This method is pretty much based on the movement hold routine.
        /// TODO: Merge both?
        /// </summary>
        /// <param name="targetPosition">Target position.</param>
        public IEnumerator ClimbTo(Vector3Int targetPosition)
        {
            if (CharacterController.IsMoving) yield return new WaitWhile(() => CharacterController.IsMoving);

            if (!CharacterController.FindTargetGrid(targetPosition, out GridController targetGrid)) yield break;

            List<TriggerActor> interactables =
                targetGrid.GetListOfActorTypesInPosition<TriggerActor>(targetPosition);

            bool dontMove = false;

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (TriggerActor interactable in interactables)
                if (interactable.HasAboutToEnterTriggers)
                    yield return interactable.PlayerAboutToEnter(this,
                                                                 CharacterController.CurrentDirection.Invert(),
                                                                 stop => dontMove = stop);

            bool triggerEncounter = false;
            EncounterType encounterType = EncounterType.Other;

            if (!dontMove)
            {
                FX.ShowClimbingParticles();

                yield return CharacterController.DashToTargetTile(CharacterController.CurrentDirection,
                                                                  targetPosition,
                                                                  (trigger, triggerEncounterType) =>
                                                                  {
                                                                      triggerEncounter = trigger;
                                                                      encounterType = triggerEncounterType;
                                                                  });

                FX.ShowClimbingParticles(false);

                GlobalGameData.StepsTaken++;

                FriendshipStepTick();

                // This is a twisted way of triggering encounters but calling the trigger encounter routine from the moving routine made both of them get stuck.
                // Perhaps from the original being called here then calling another class then this one, looks like a Unity bug.
                if (triggerEncounter) yield return TriggerEncounter(encounterType);

                if (repelCounter > 0) yield return RepelStepTick();

                yield return GlobalGridManager.PlayerMoved(this);
            }

            if (!HoldingMovement)
                CharacterController.LookAt(lastUsedDirection, useBikingSprite: CharacterController.IsBiking);

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (TriggerActor interactable in interactables)
                if (interactable.HasEnteredTriggers)
                    yield return interactable.PlayerEntered(this, CharacterController.CurrentDirection);

            // We do this twice because if you are frame perfect you can release the movement while triggers are running and get the sprite in a weird state while running.
            if (!HoldingMovement)
                CharacterController.LookAt(lastUsedDirection, useBikingSprite: CharacterController.IsBiking);
        }

        /// <summary>
        /// Add an amount of steps to the repel counter.
        /// </summary>
        /// <param name="usedItem">Item used to add that amount.</param>
        public IEnumerator AddRepelSteps(Repel usedItem)
        {
            audioManager.PlayAudio(usedItem.RepelSound);

            yield return DialogManager.ShowDialogAndWait("Items/Repel/Used");

            repelCounter += usedItem.RepelAmount;
            lastRepelItem = usedItem;
        }

        /// <summary>
        /// Set the information about repels.
        /// Only to be used on object resetting circumstances, like teleporting.
        /// </summary>
        /// <param name="steps">Steps to set.</param>
        /// <param name="item">Last item to set.</param>
        public void SetRepelInfo(uint steps, Repel item)
        {
            repelCounter = steps;
            lastRepelItem = item;
        }

        /// <summary>
        /// Retrieve the information about repels.
        /// </summary>
        /// <returns>A tuple with the steps left and the last item used.</returns>
        public (uint, Repel) GetRepelInfo() => (repelCounter, lastRepelItem);

        /// <summary>
        /// Type of input.
        /// </summary>
        public InputType GetInputType() => InputType.Main;

        /// <summary>
        /// Input debug name.
        /// </summary>
        public string GetDebugName() => "Player";

        /// <summary>
        /// Translate the input of two axi to a direction.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>A direction to go into.</returns>
        private CharacterController.Direction InputToDirection(float x, float y)
        {
            if (Mathf.Abs(x) > Mathf.Abs(y))
                switch (x)
                {
                    case < 0: return CharacterController.Direction.Left;
                    case > 0: return CharacterController.Direction.Right;
                }
            else
                switch (y)
                {
                    case < 0: return CharacterController.Direction.Down;
                    case > 0: return CharacterController.Direction.Up;
                }

            Logger.Warn("Asked to translate input which is all 0s!");

            return CharacterController.Direction.None;
        }

        #region Unused input callbacks

        public void OnNavigation(InputAction.CallbackContext context)
        {
        }

        public void OnSelect(InputAction.CallbackContext context)
        {
        }

        public void OnBack(InputAction.CallbackContext context)
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
        /// Player character factory for dependency injection.
        /// </summary>
        public class Factory : GameObjectFactory<PlayerCharacter>
        {
        }
    }
}