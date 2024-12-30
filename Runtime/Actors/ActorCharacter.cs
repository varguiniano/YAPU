using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.World;
using Varguiniano.YAPU.Runtime.World.Tiles;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Localization.Runtime;
using CharacterController = Varguiniano.YAPU.Runtime.Characters.CharacterController;

namespace Varguiniano.YAPU.Runtime.Actors
{
    /// <summary>
    /// Actor controller for character actors.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class ActorCharacter : Actor, IPlayerMovementSubscriber, ICharacterMovementSubscriber, ICommandParameter
    {
        /// <summary>
        /// Normally actors that subscribe to the grid will block movement.
        /// </summary>
        public override bool BlocksMovement => true;

        /// <summary>
        /// Can this character spot the player and walk to them?
        /// </summary>
        [FoldoutGroup("Spotting")]
        public bool CanSpotPlayer;

        /// <summary>
        /// Distance at which the player can be spotted.
        /// </summary>
        [FoldoutGroup("Spotting")]
        [SerializeField]
        [ShowIf(nameof(CanSpotPlayer))]
        private byte SpotRange;

        /// <summary>
        /// Commands that are triggered when it spots the player.
        /// </summary>
        [FoldoutGroup("Spotting")]
        [ShowIf(nameof(CanSpotPlayer))]
        [SerializeField]
        [SerializeReference]
        [InfoBox("These commands when the player is spotted by this character.")]
        protected ActorCommandGraph SpotPlayerCommandGraph;

        /// <summary>
        /// Cache of the tiles this character can spot.
        /// </summary>
        [FoldoutGroup("Spotting")]
        [ShowIf(nameof(CanSpotPlayer))]
        [ShowInInspector]
        [ReadOnly]
        [HideInEditorMode]
        private List<Vector3Int> spotRangeCache = new();

        /// <summary>
        /// Reference to the popup.
        /// </summary>
        [FoldoutGroup("References")]
        public CharacterPopUpController Popup;

        /// <summary>
        /// Reference to the attached character controller.
        /// </summary>
        public CharacterController CharacterController => GetCachedComponent<CharacterController>();

        /// <summary>
        /// Reference to the player.
        /// </summary>
        private PlayerCharacter player;

        /// <summary>
        /// Flag to know if the character is trying to spot the player.
        /// </summary>
        private bool tryingToSpot;

        /// <summary>
        /// Subscribe to player events to spot the player if it can.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!CanSpotPlayer) return;
            UpdateSpotRange();
            CharacterController.SubscribeToMovement(this);
            GlobalGridManager.SubscribeToPlayerMovement(this);
        }

        /// <summary>
        /// Unsubscribe from the actor character.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            CharacterController.UnSubscribeFromMovement(this);
            GlobalGridManager.UnSubscribeFromPlayerMovement(this);
        }

        /// <summary>
        /// Called when the character moves.
        /// </summary>
        /// <param name="characterControllerReference">Reference to the character.</param>
        public IEnumerator OnCharacterMoved(CharacterController characterControllerReference)
        {
            UpdateSpotRange();

            if (CanSpotPlayer) yield return TrySpot();
        }

        /// <summary>
        /// Called each time the player moves.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="finished">Called when the callback is finished.</param>
        public IEnumerator PlayerMoved(PlayerCharacter playerCharacter, Action finished)
        {
            player = playerCharacter;

            if (CanSpotPlayer)
                yield return TrySpot();
            else // Only those that spot need to keep subscribed.
                GlobalGridManager.UnSubscribeFromPlayerMovement(this);

            finished.Invoke();
        }

        /// <summary>
        /// Get the current grid this actor is in.
        /// </summary>
        /// <returns>The grid controller.</returns>
        public override GridController GetCurrentGrid() => CharacterController.CurrentGrid;

        /// <summary>
        /// Called when the player interacts with this element.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="playerDirection">Direction the player is at.</param>
        public override IEnumerator Interact(PlayerCharacter playerCharacter,
                                             CharacterController.Direction playerDirection)
        {
            yield return new WaitWhile(() => tryingToSpot);

            yield return base.Interact(playerCharacter, playerDirection);
        }

        /// <summary>
        /// Check if the player has been spotted and trigger the spot commands.
        /// <param name="changeLooping">Callback use to modify the looping value.</param>
        /// </summary>
        private IEnumerator CheckIfThePlayerIsSpottedAndTriggerCommands(Action<bool> changeLooping)
        {
            if (GlobalGridManager.PlayerSpotAttempt)
                yield return new WaitWhile(() => GlobalGridManager.PlayerSpotAttempt);

            GlobalGridManager.PlayerSpotAttempt = true;

            if (!CanSpotPlayer || RunningCommands || player == null || !spotRangeCache.Contains(player.Position))
            {
                GlobalGridManager.PlayerSpotAttempt = false;
                yield break;
            }

            // Recheck but this time taking obstacles into account.
            UpdateSpotRange(true);

            if (!spotRangeCache.Contains(player.Position))
            {
                GlobalGridManager.PlayerSpotAttempt = false;
                yield break;
            }

            RunningCommands = true;
            player.LockInput();

            yield return WaitAFrame;

            if (player.CharacterController.IsMoving)
                yield return new WaitWhile(() => player.CharacterController.IsMoving);

            if (!spotRangeCache.Contains(player.Position))
            {
                RunningCommands = false;
                player.LockInput(false);
                GlobalGridManager.PlayerSpotAttempt = false;
                yield break;
            }

            GlobalGridManager.StopAllActors = true;
            InputManager.BlockInput();

            yield return SpotPlayerCommandGraph.Run(new CommandParameterData(gameObject,
                                                                             this,
                                                                             CharacterController.CurrentDirection,
                                                                             player,
                                                                             GlobalGameData,
                                                                             Settings,
                                                                             Database,
                                                                             BattleLauncher,
                                                                             Localizer,
                                                                             ConfigurationManager,
                                                                             InputManager,
                                                                             PlayerTeleporter,
                                                                             SavegameManager,
                                                                             MapSceneLauncher,
                                                                             TimeManager,
                                                                             QuestManager,
                                                                             TradeManager,
                                                                             commandCallbackParams =>
                                                                             {
                                                                                 if (commandCallbackParams
                                                                                 .UpdateLooping)
                                                                                     changeLooping
                                                                                        .Invoke(commandCallbackParams
                                                                                            .NewLooping);
                                                                             }));

            InputManager.BlockInput(false);
            GlobalGridManager.StopAllActors = false;

            player.LockInput(false);
            RunningCommands = false;
            GlobalGridManager.PlayerSpotAttempt = false;
        }

        /// <summary>
        /// Try to spot the player.
        /// </summary>
        private IEnumerator TrySpot()
        {
            if (!CanSpotPlayer) yield break;

            if (tryingToSpot) yield break;

            tryingToSpot = true;

            bool wasLooping = ShouldLoop;

            ShouldLoop = false;

            yield return CheckIfThePlayerIsSpottedAndTriggerCommands(change =>
                                                                     {
                                                                         wasLooping = change;
                                                                     });

            ShouldLoop = wasLooping;

            tryingToSpot = false;
        }

        /// <summary>
        /// Update the range of spotted tiles.
        /// </summary>
        private void UpdateSpotRange(bool checkObstacles = false)
        {
            if (!CanSpotPlayer) return;
            spotRangeCache = GetSpotRangeTiles(checkObstacles);
        }

        /// <summary>
        /// Get the tiles inside the spot range of this actor.
        /// </summary>
        private List<Vector3Int> GetSpotRangeTiles(bool checkObstacles)
        {
            List<Vector3Int> tiles = new();

            Vector3Int previousCandidate = CharacterController.Transform.position.ToInts();
            Vector3Int candidate = previousCandidate;

            GridController previousGrid = CharacterController.CurrentGrid;

            for (int i = 1; i <= SpotRange; ++i)
            {
                candidate = CharacterController.MoveOneInDirection(candidate, CharacterController.CurrentDirection);

                GridController targetGrid = null;

                if (checkObstacles)
                {
                    if (!CharacterController.FindTargetGrid(candidate, out targetGrid)) break;

                    if (!CharacterController.CheckCommonTypeMovementRestrictions(targetGrid,
                            candidate,
                            previousCandidate,
                            canExitWater: false,
                            logWarnings: false,
                            recalculateTypes: true,
                            ignorePlayer: true))
                        break;

                    BlockMovementDirections blocker = previousGrid
                                                     .GetGameObjectOfTileDirectlyBelowSortOrder(previousCandidate,
                                                          CharacterController.SortOrder)
                                                    ?.GetComponent<BlockMovementDirections>();

                    if (blocker != null && blocker.BlockedDirections.Contains(CharacterController.CurrentDirection))
                        break;
                }

                tiles.Add(candidate);

                previousCandidate = candidate;
                previousGrid = targetGrid;
            }

            return tiles;
        }

        /// <summary>
        /// Get the localized name of this object.
        /// </summary>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <returns>The localized string.</returns>
        public string GetLocalizedName(ILocalizer localizer) => CharacterController.GetLocalizedName();
    }
}