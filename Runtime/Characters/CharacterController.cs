using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.World;
using Varguiniano.YAPU.Runtime.World.Encounters;
using Varguiniano.YAPU.Runtime.World.Tiles;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using WhateverDevs.Core.Editor.Utils;
#endif

namespace Varguiniano.YAPU.Runtime.Characters
{
    /// <summary>
    /// Controller for a character that exists in the world.
    /// </summary>
    public class CharacterController : WhateverBehaviour<CharacterController>, ISceneMember
    {
        /// <summary>
        /// Character mode.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        [OnValueChanged(nameof(OnModeChanged))]
        public CharacterMode Mode;

        /// <summary>
        /// Character to display.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [ShowIf("@Mode == CharacterMode.Character")]
        [OnValueChanged(nameof(OnCharacterChanged))]
        [SerializeField]
        private CharacterData Character;

        /// <summary>
        /// Character to display.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [ShowIf("@Mode == CharacterMode.Monster")]
        [InfoBox("Only the first monster in the roster will be seen in the world.",
                 InfoMessageType.Warning,
                 VisibleIf = "@MonsterRoster != null && MonsterRoster.RosterSize > 1")]
        [InfoBox("Roster must have one monster.",
                 InfoMessageType.Error,
                 VisibleIf = "@MonsterRoster != null && MonsterRoster.RosterSize == 0")]
        [OnValueChanged(nameof(OnMonsterChanged))]
        public Roster MonsterRoster;

        /// <summary>
        /// Is this character the player?
        /// </summary>
        [FoldoutGroup("Configuration")]
        public bool IsPlayer;

        /// <summary>
        /// Initialize on enable?
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private bool InitOnEnable = true;

        /// <summary>
        /// Speed of this character in seconds/tile.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [Tooltip("Seconds/tile")]
        [SerializeField]
        private float Speed = .25f;

        /// <summary>
        /// Normal sorting order for the shadow.
        /// </summary>
        [FoldoutGroup("Configuration/Sort Order")]
        [SerializeField]
        private int NormalShadowSortOrder = 5;

        /// <summary>
        /// Normal sorting order for the body.
        /// </summary>
        [FoldoutGroup("Configuration/Sort Order")]
        [SerializeField]
        private int NormalBodySortOrder = 10;

        /// <summary>
        /// Normal sorting order for the head.
        /// </summary>
        [FoldoutGroup("Configuration/Sort Order")]
        [SerializeField]
        private int NormalHeadSortOrder = 15;

        /// <summary>
        /// Normal sorting order for the shadow.
        /// </summary>
        [FoldoutGroup("Configuration/Sort Order")]
        [SerializeField]
        private int OverBridgeShadowSortOrder = 23;

        /// <summary>
        /// Over bridge sorting order for the body.
        /// </summary>
        [FoldoutGroup("Configuration/Sort Order")]
        public int OverBridgeBodySortOrder = 30;

        /// <summary>
        /// Over bridge sorting order for the head.
        /// </summary>
        [FoldoutGroup("Configuration/Sort Order")]
        [SerializeField]
        private int OverBridgeHeadSortOrder = 35;

        /// <summary>
        /// Audio for jumping.
        /// </summary>
        [FoldoutGroup("Configuration/Audio")]
        [SerializeField]
        private bool MakeSound;

        /// <summary>
        /// Audio for jumping.
        /// </summary>
        [FoldoutGroup("Configuration/Audio")]
        [SerializeField]
        private AudioReference JumpAudio;

        /// <summary>
        /// Sort order to use when hiding the graphics.
        /// </summary>
        [FoldoutGroup("Configuration/Animations")]
        [SerializeField]
        private int HideSortOrder = -100;

        /// <summary>
        /// Sort order to use when falling from the sky.
        /// </summary>
        [FoldoutGroup("Configuration/Animations")]
        [SerializeField]
        private int FallAnimationSortOrder = 100;

        /// <summary>
        /// Y position when it starts falling from the sky.
        /// </summary>
        [FoldoutGroup("Configuration/Animations")]
        [SerializeField]
        private float FallAnimationYPosition = 15;

        /// <summary>
        /// Audio to play when falling.
        /// </summary>
        [FoldoutGroup("Configuration/Animations")]
        [SerializeField]
        private AudioReference FallAudio;

        /// <summary>
        /// Audio to play when falling and hitting the floor.
        /// </summary>
        [FoldoutGroup("Configuration/Animations")]
        [SerializeField]
        private AudioReference FloorHitAudio;

        /// <summary>
        /// Reference to the attached SpriteRenderers.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private List<SpriteRenderer> SpriteRenderers;

        /// <summary>
        /// Reference to the renderers for the shadow.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private List<SpriteRenderer> ShadowRenderers;

        /// <summary>
        /// Reference to the renderers for the body.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private List<SpriteRenderer> BodyRenderers;

        /// <summary>
        /// Reference to the renderers for the head.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private List<SpriteRenderer> HeadRenderers;

        /// <summary>
        /// Reference to the body's pivot transform.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform BodyPivot;

        /// <summary>
        /// Current direction of the character.
        /// </summary>
        [ReadOnly]
        [ShowInInspector]
        public Direction CurrentDirection;

        /// <summary>
        /// Is the character running?
        /// </summary>
        public bool IsRunning;

        /// <summary>
        /// Is the character biking?
        /// </summary>
        public bool IsBiking;

        /// <summary>
        /// Is the character fishing?
        /// </summary>
        public bool IsFishing;

        /// <summary>
        /// Flag to know if the character is swimming.
        /// </summary>
        public bool IsSwimming;

        /// <summary>
        /// Flag to know if the character is moving.
        /// </summary>
        public bool IsMoving { get; private set; }

        /// <summary>
        /// Is the character over a bridge?
        /// </summary>
        [ReadOnly]
        [ShowInInspector]
        public bool IsOverBridge { get; private set; }

        /// <summary>
        /// Event raised whenever the character moves or changes direction.
        /// </summary>
        private readonly List<ICharacterMovementSubscriber> movementSubscribers = new();

        /// <summary>
        /// Flag to know if the character is changing from one tile to another.
        /// </summary>
        private bool isChangingTiles;

        /// <summary>
        /// Speed multiplier when running.
        /// </summary>
        private float RunningSpeedMultiplier => IsRunning && !IsBiking && !IsSwimming ? .33f : 1;

        /// <summary>
        /// Speed multiplier when biking.
        /// </summary>
        private float BikingSpeedMultiplier => IsBiking && !IsSwimming ? .15f : 1;

        /// <summary>
        /// Speed multiplier when swimming.
        /// </summary>
        private float SwimmingSpeedMultiplier => IsSwimming ? .66f : 1;

        /// <summary>
        /// Speed multiplier when jumping.
        /// </summary>
        private const float JumpMultiplier = 1.5f;

        /// <summary>
        /// Characters sort order.
        /// </summary>
        public int SortOrder => SpriteRenderers[0].sortingOrder;

        /// <summary>
        /// Reference to the grid this character is in.
        /// </summary>
        [HideInInspector]
        public GridController CurrentGrid;

        /// <summary>
        /// Reference to the global grid manager.
        /// </summary>
        [Inject]
        private GlobalGridManager globalGridManager;

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
        /// Reference to the YAPU settings.
        /// </summary>
        [Inject]
        private YAPUSettings yapuSettings;

        /// <summary>
        /// Localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Cached reference to the attached transform.
        /// </summary>
        public Transform Transform
        {
            get
            {
                if (transformReference == null) transformReference = transform;
                return transformReference;
            }
        }

        /// <summary>
        /// Backfield for Transform.
        /// </summary>
        private Transform transformReference;

        /// <summary>
        /// Foot it moved last time.
        /// </summary>
        private int foot;

        /// <summary>
        /// Variable storing the original position of the body pivot.
        /// </summary>
        private float originalBodyPivotYPosition;

        /// <summary>
        /// Initialize if set on enable.
        /// </summary>
        private void OnEnable()
        {
            if (InitOnEnable) Initialize();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize()
        {
            CurrentGrid = FindObjectsByType<GridController>(FindObjectsSortMode.None)
               .First(grid => grid.gameObject.scene == gameObject.scene);

            Vector3Int position = Transform.position.ToInts();

            CurrentGrid.CharacterAboutToEnterTileAsync(this, position);
            CurrentGrid.CharacterEnterTileAsync(this, position);

            StartCoroutine(CurrentGrid.CharacterEnterTile(this,
                                                          position,
                                                          _ =>
                                                          {
                                                          }));

            if (IsPlayer) CurrentGrid.PlayerEnterGrid(GetCachedComponent<PlayerCharacter>());

            UpdateSortingOrderToCurrentTile();
        }

        /// <summary>
        /// Unregister from the grid.
        /// </summary>
        private void OnDisable()
        {
            CurrentGrid.RemoveCharacterFromGridWithoutLeftCallbacks(this);

            if (IsPlayer) CurrentGrid.PlayerExitGrid(GetCachedComponent<PlayerCharacter>());

            Transform.DOKill();
        }

        /// <summary>
        /// Called when the mode is changed.
        /// </summary>
        private void OnModeChanged()
        {
            foreach (SpriteRenderer shadowRenderer in ShadowRenderers)
                shadowRenderer.enabled = Mode != CharacterMode.Invisible;

            if (Mode == CharacterMode.Invisible)
                LookAt(Direction.Down, useBikingSprite: IsBiking);
            else
            {
                OnCharacterChanged();
                OnMonsterChanged();
            }
        }

        /// <summary>
        /// Called when the character is changed.
        /// </summary>
        private void OnCharacterChanged()
        {
            if (GetCharacterData() == null) return;

            LookAt(Direction.Down, useBikingSprite: IsBiking);
        }

        /// <summary>
        /// Called when the monster is changed.
        /// </summary>
        public void OnMonsterChanged()
        {
            if (GetMonsterData() == null) return;

            LookAt(Direction.Down, useBikingSprite: IsBiking);
        }

        /// <summary>
        /// Subscribe to the movement.
        /// </summary>
        /// <param name="subscriber">Subscriber.</param>
        public void SubscribeToMovement(ICharacterMovementSubscriber subscriber) => movementSubscribers.Add(subscriber);

        /// <summary>
        /// Unsubscribe from the movement.
        /// </summary>
        /// <param name="unSubscriber">UnSubscriber.</param>
        public void UnSubscribeFromMovement(ICharacterMovementSubscriber unSubscriber)
        {
            if (movementSubscribers.Contains(unSubscriber)) movementSubscribers.Remove(unSubscriber);
        }

        /// <summary>
        /// Trigger the move event.
        /// </summary>
        private IEnumerator TriggerMoveEvent()
        {
            using IEnumerator<IEnumerator> triggerMoveEvent = movementSubscribers
                                                             .Select(subscriber => subscriber.OnCharacterMoved(this))
                                                             .GetEnumerator();

            yield return triggerMoveEvent;
        }

        /// <summary>
        /// Look at the given direction.
        /// </summary>
        /// <param name="direction">Direction to look at.</param>
        /// <param name="useRunningSprite">Use the running sprite?</param>
        /// <param name="useBikingSprite">Use the biking sprite?</param>
        [Button("Look at")]
        public void LookAt(Direction direction, bool useRunningSprite = false, bool useBikingSprite = false)
        {
            if (direction == Direction.None) return;

            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            ChangeSprite(GetSpriteContainer()
                            .GetLooking(direction, IsSwimming, useBikingSprite, useRunningSprite, IsFishing));

            CurrentDirection = direction;

            StartCoroutine(TriggerMoveEvent());
        }

        /// <summary>
        /// Move in the given direction.
        /// </summary>
        /// <param name="direction">Direction to move towards.</param>
        /// <param name="tiles">Number of tiles to move in that direction.</param>
        [Button("Move")]
        [HideInEditorMode]
        private void TestMove(Direction direction, int tiles = 1) => StartCoroutine(Move(direction, tiles));

        /// <summary>
        /// Enter the water in the given direction.
        /// </summary>
        /// <param name="direction">Direction to move towards.</param>
        [Button("Enter Water")]
        [HideInEditorMode]
        private void TestEnterWater(Direction direction) => StartCoroutine(EnterWater(direction));

        /// <summary>
        /// Climb a waterfall.
        /// </summary>
        [Button("Climb Waterfall")]
        [HideInEditorMode]
        private void TestClimbWaterfall() => StartCoroutine(ClimbWaterfall());

        /// <summary>
        /// Move in the given direction.
        /// </summary>
        /// <param name="direction">Direction to move towards.</param>
        /// <param name="tiles">Number of tiles to move in that direction.</param>
        /// <param name="waitForNextTileToBeFree">Wait for the next tile to be free?</param>
        public IEnumerator Move(Direction direction, int tiles, bool waitForNextTileToBeFree = false)
        {
            for (int i = 0; i < tiles; ++i)
                yield return Move(direction, waitForNextTileToBeFree: waitForNextTileToBeFree);

            LookAt(direction, useBikingSprite: IsBiking);
        }

        /// <summary>
        /// Check if the character can enter water in the given direction.
        /// </summary>
        /// <param name="direction">Direction to check.</param>
        /// <returns>True if it can.</returns>
        public bool CanEnterWater(Direction direction) =>
            CanMoveToNextTile(direction,
                              true,
                              false,
                              out TileType currentType,
                              out TileType targetType,
                              out GameObject _)
         && currentType
         == TileType.Walkable
         && targetType == TileType.Water;

        /// <summary>
        /// Enter the water in the given direction.
        /// </summary>
        /// <param name="direction">Direction to enter water to.</param>
        public IEnumerator EnterWater(Direction direction)
        {
            IsBiking = false;
            yield return Move(direction, true);
        }

        /// <summary>
        /// Check if the character can enter water in the given direction.
        /// </summary>
        /// <returns>True if it can.</returns>
        public bool CanClimbWaterfall() =>
            CurrentDirection == Direction.Up
         && CanMoveToNextTile(Direction.Up,
                              false,
                              true,
                              out TileType currentType,
                              out TileType targetType,
                              out GameObject _)
         && currentType == TileType.Water
         && targetType == TileType.Waterfall;

        /// <summary>
        /// Climb a waterfall.
        /// </summary>
        /// <returns></returns>
        public IEnumerator ClimbWaterfall()
        {
            yield return Move(Direction.Up, climbWaterfall: true);
        }

        /// <summary>
        /// Check if the character can move to the next tile in the given direction.
        /// </summary>
        /// <param name="direction">Direction to look into.</param>
        /// <param name="enterWater">Is the character entering water?</param>
        /// <param name="climbWaterfall">Is the character climbing a waterfall?</param>
        /// <param name="currentType">Type of the current tile.</param>
        /// <param name="targetType">Type of the target tile.</param>
        /// <param name="targetObject">Object in the target tile that would be in the layer directly below the player. Can be null.</param>
        /// <returns>True if it can.</returns>
        public bool CanMoveToNextTile(Direction direction,
                                      bool enterWater,
                                      bool climbWaterfall,
                                      out TileType currentType,
                                      out TileType targetType,
                                      out GameObject targetObject)
        {
            Vector3Int currentPosition = Transform.position.ToInts();

            BlockMovementDirections blocker = CurrentGrid
                                             .GetGameObjectOfTileDirectlyBelowSortOrder(currentPosition, SortOrder)
                                            ?.GetComponent<BlockMovementDirections>();

            if (blocker != null && blocker.BlockedDirections.Contains(direction))
            {
                Logger.Warn("The current tile at " + currentPosition + "blocks movement " + direction + ".");
                currentType = TileType.NonExistent;
                targetType = TileType.NonExistent;
                targetObject = null;
                return false;
            }

            currentType =
                CurrentGrid.GetTypeOfTileDirectlyBelowSortOrder(currentPosition, SortOrder);

            TileType topCurrentType =
                CurrentGrid.GetTypeOfTileDirectlyBelowSortOrder(currentPosition, OverBridgeBodySortOrder);

            Vector3Int targetPosition = MoveOneInDirection(currentPosition, direction);

            // Get the target grid and get the tile types.
            if (!FindTargetGrid(targetPosition, out GridController targetGrid))
            {
                targetType = TileType.NonExistent;
                targetObject = null;
                return false;
            }

            targetType =
                targetGrid.GetTypeOfTileDirectlyBelowSortOrder(targetPosition, SortOrder);

            TileType topTargetType =
                targetGrid.GetTypeOfTileDirectlyBelowSortOrder(targetPosition, OverBridgeBodySortOrder);

            targetObject = targetGrid.GetGameObjectOfTileDirectlyBelowSortOrder(targetPosition, SortOrder);

            // First check all the common restrictions, then make sure that it is precisely entering water and not on any other situation.
            return CheckCommonTypeMovementRestrictions(targetGrid,
                                                       targetPosition,
                                                       currentPosition,
                                                       currentType,
                                                       targetType,
                                                       topCurrentType,
                                                       topTargetType,
                                                       enterWater,
                                                       isWaterFalling: climbWaterfall,
                                                       logWarnings: false);
        }

        /// <summary>
        /// Hide the graphics.
        /// </summary>
        public void HideGraphics() => UpdateAllSortOrdersToFixedValue(HideSortOrder);

        /// <summary>
        /// Prepare the character to fall from the sky.
        /// </summary>
        public void PrepareToFallFromSky()
        {
            UpdateAllSortOrdersToFixedValue(FallAnimationSortOrder);

            Vector3 bodyPivotLocalPosition = BodyPivot.localPosition;

            originalBodyPivotYPosition = bodyPivotLocalPosition.y;
            bodyPivotLocalPosition.y = FallAnimationYPosition;

            BodyPivot.localPosition = bodyPivotLocalPosition;
        }

        /// <summary>
        /// Make a fall from the sky animation.
        /// </summary>
        public IEnumerator FallFromSky()
        {
            audioManager.PlayAudio(FallAudio);

            bool playedHit = false;

            Tween tween = BodyPivot.DOLocalMoveY(originalBodyPivotYPosition, 1.5f).SetEase(Ease.InExpo);

            tween.OnUpdate(() =>
                           {
                               if (playedHit || !(tween.ElapsedPercentage() >= .9f)) return;
                               audioManager.PlayAudio(FloorHitAudio);
                               audioManager.StopAudio(FallAudio);
                               UpdateSortingOrderToCurrentTile();
                               playedHit = true;
                           });

            yield return tween.WaitForCompletion();
        }

        /// <summary>
        /// Move in the given direction.
        /// </summary>
        /// <param name="direction">Direction to move towards.</param>
        /// <param name="enterWater">Is the character entering water?</param>
        /// <param name="climbWaterfall">Is the character climbing a waterfall?</param>
        /// <param name="forceKeepMoving">Force the character to keep moving.</param>
        /// <param name="animateWhirlwind">Animate the player like they are in a whirlwind?</param>
        /// <param name="speedMultiplier">Apply an external multiplier to the movement?</param>
        /// <param name="runningAndOthersAffectSpeed">Flag to mark if running, swimming, biking... affect speed.</param>
        /// <param name="waitForNextTileToBeFree">Wait for the next tile to be free?</param>
        /// <param name="playerCallback">Callback for the player actor to give it information like if it should trigger an encounter.</param>
        public IEnumerator Move(Direction direction,
                                bool enterWater = false,
                                bool climbWaterfall = false,
                                bool forceKeepMoving = false,
                                bool animateWhirlwind = false,
                                float speedMultiplier = 1,
                                bool runningAndOthersAffectSpeed = true,
                                bool waitForNextTileToBeFree = false,
                                Action<bool, EncounterType> playerCallback = null)
        {
            if (direction == Direction.None) yield break;

            if (IsMoving && !forceKeepMoving)
            {
                Logger.Warn("Already moving! Will wait for movement to stop.");
                yield return new WaitWhile(() => IsMoving);
            }

            LookAt(direction, useBikingSprite: IsBiking);

            Vector3 currentPosition = Transform.position;

            BlockMovementDirections blocker = CurrentGrid
                                             .GetGameObjectOfTileDirectlyBelowSortOrder(currentPosition.ToInts(),
                                                  SortOrder)
                                            ?.GetComponent<BlockMovementDirections>();

            if (blocker != null && blocker.BlockedDirections.Contains(direction))
            {
                Logger.Warn("The current tile at " + currentPosition + "blocks movement " + direction + ".");
                yield break;
            }

            List<Sprite>
                flipbook = GetSpriteContainer().GetWalking(direction, IsSwimming, IsBiking, IsRunning);

            Vector3Int targetPosition = MoveOneInDirection(currentPosition.ToInts(), direction);

            yield return MoveToTargetTile(direction,
                                          currentPosition.ToInts(),
                                          targetPosition,
                                          flipbook,
                                          false,
                                          enterWater,
                                          climbWaterfall,
                                          false,
                                          false,
                                          animateWhirlwind: animateWhirlwind,
                                          speedMultiplier: speedMultiplier,
                                          runningAndOthersAffectSpeed: runningAndOthersAffectSpeed,
                                          waitForNextTileToBeFree: waitForNextTileToBeFree,
                                          playerCallback: playerCallback);
        }

        /// <summary>
        /// Have the character dash to the target tile.
        /// </summary>
        /// <param name="direction">Direction to dash into.</param>
        /// <param name="targetPosition">Target position.</param>
        /// <param name="playerCallback">Callback for the player actor to give it information like if it should trigger an encounter.</param>
        public IEnumerator DashToTargetTile(Direction direction,
                                            Vector3Int targetPosition,
                                            Action<bool, EncounterType> playerCallback = null)
        {
            List<Sprite>
                flipbook = GetSpriteContainer().GetWalking(direction, false, false, false);

            Vector3Int currentPosition = transform.position.ToInts();

            int steps = 0;

            Vector3Int stepPosition = currentPosition;

            do
            {
                stepPosition = MoveOneInDirection(stepPosition, direction);

                steps++;
            }
            while (stepPosition != targetPosition);

            yield return MoveToTargetTile(direction,
                                          currentPosition,
                                          targetPosition,
                                          flipbook,
                                          false,
                                          false,
                                          false,
                                          false,
                                          false,
                                          speedMultiplier: steps * .33333f,
                                          runningAndOthersAffectSpeed: false,
                                          playerCallback: playerCallback);
        }

        /// <summary>
        /// Move to the target tile.
        /// </summary>
        /// <param name="direction">Direction we are taking.</param>
        /// <param name="currentPosition">Our current position.</param>
        /// <param name="targetPosition">The target position.</param>
        /// <param name="flipbook">Flipbook to use.</param>
        /// <param name="jump">Are we jumping?</param>
        /// <param name="enterWater">Is the character entering water?</param>
        /// <param name="climbWaterfall">Is the character climbing a waterfall?</param>
        /// <param name="isWaterFalling">Is the character going through a waterfall?</param>
        /// <param name="isSliding">Is the character sliding.</param>
        /// <param name="animateSteps">Animate the steps when moving?</param>
        /// <param name="animateWhirlwind">Animate the movement like if the player was in a whirlwind?</param>
        /// <param name="speedMultiplier">Multiplier for the animation speed.</param>
        /// <param name="runningAndOthersAffectSpeed">Flag to mark if running, swimming, biking... affect speed.</param>
        /// <param name="waitForNextTileToBeFree">Wait for the next tile to be free?</param>
        /// <param name="playerCallback">Callback for the player actor to give it information like if it should trigger an encounter.</param>
        // Every coroutine call causes some frames lag, so this is going to be a monster of a method.
        // ReSharper disable once CyclomaticComplexity
        // ReSharper disable once FunctionComplexityOverflow
        private IEnumerator MoveToTargetTile(Direction direction,
                                             Vector3Int currentPosition,
                                             Vector3Int targetPosition,
                                             IReadOnlyList<Sprite> flipbook,
                                             bool jump,
                                             bool enterWater,
                                             bool climbWaterfall,
                                             bool isWaterFalling,
                                             bool isSliding,
                                             bool animateSteps = true,
                                             bool animateWhirlwind = false,
                                             float speedMultiplier = 1,
                                             bool runningAndOthersAffectSpeed = true,
                                             bool waitForNextTileToBeFree = false,
                                             Action<bool, EncounterType> playerCallback = null)
        {
            IsMoving = true;

            // Get the target grid and get the tile types.
            if (!FindTargetGrid(targetPosition, out GridController targetGrid))
            {
                IsMoving = false;
                yield break;
            }

            TileType currentType =
                CurrentGrid.GetTypeOfTileDirectlyBelowSortOrder(currentPosition, SortOrder);

            TileType topCurrentType =
                CurrentGrid.GetTypeOfTileDirectlyBelowSortOrder(currentPosition, OverBridgeBodySortOrder);

            TileType targetType =
                targetGrid.GetTypeOfTileDirectlyBelowSortOrder(targetPosition, SortOrder);

            TileType topTargetType =
                targetGrid.GetTypeOfTileDirectlyBelowSortOrder(targetPosition, OverBridgeBodySortOrder);

            // Calculate and perform if it should jump.
            if (targetType == TileType.Jumpable && !jump && !IsSwimming)
            {
                OneWayJumpableTile jumpableData = targetGrid
                                                 .GetAttachedTileObjectOnPositionDirectlyBelowPlayer(targetPosition,
                                                      SortOrder)
                                                ?.GetComponent<OneWayJumpableTile>();

                if (jumpableData != null && jumpableData.JumpDirection == direction)
                {
                    Vector3Int jumpTarget = MoveOneInDirection(targetPosition, direction);

                    Logger.Info("Next tile is jumpable and direction matches, trying to jump to " + jumpTarget + ".");

                    yield return MoveToTargetTile(direction,
                                                  currentPosition,
                                                  jumpTarget,
                                                  flipbook,
                                                  true,
                                                  false,
                                                  false,
                                                  false,
                                                  false,
                                                  animateSteps,
                                                  animateWhirlwind,
                                                  2,
                                                  true,
                                                  waitForNextTileToBeFree,
                                                  playerCallback);

                    IsMoving = false;
                    yield break;
                }
            }

            // Calculate and perform waterfall if it should.
            if (!isWaterFalling && targetType == TileType.Waterfall)
            {
                WaterfallEdge edge = targetGrid
                                    .GetAttachedTileObjectOnPositionDirectlyBelowPlayer(targetPosition,
                                         SortOrder)
                                   ?.GetComponent<WaterfallEdge>();

                if (edge != null)
                    if ((edge.EdgeType == EdgeType.Top && direction == Direction.Down)
                     || (edge.EdgeType == EdgeType.Bottom && direction == Direction.Up && climbWaterfall))
                    {
                        Vector3Int waterfallTarget = targetPosition;
                        TileType newTargetType;
                        int steps = 0;

                        do
                        {
                            waterfallTarget = MoveOneInDirection(waterfallTarget, direction);

                            newTargetType = targetGrid.GetTypeOfTileDirectlyBelowSortOrder(waterfallTarget, SortOrder);

                            steps++;
                        }
                        while (newTargetType == TileType.Waterfall);

                        yield return MoveToTargetTile(direction,
                                                      currentPosition,
                                                      waterfallTarget,
                                                      flipbook,
                                                      false,
                                                      false,
                                                      false,
                                                      true,
                                                      false,
                                                      animateSteps,
                                                      animateWhirlwind,
                                                      steps,
                                                      true,
                                                      waitForNextTileToBeFree,
                                                      playerCallback);

                        IsMoving = false;
                        yield break;
                    }
            }

            // Calculate and slide if it should.
            if (!isSliding && targetType == TileType.Slippery)
            {
                Vector3Int slideTarget = targetPosition;
                TileType newTargetType;
                int steps = 0;

                do
                {
                    Vector3Int candidateSlideTarget = MoveOneInDirection(slideTarget, direction);

                    TileType candidateTargetType =
                        targetGrid.GetTypeOfTileDirectlyBelowSortOrder(candidateSlideTarget, SortOrder);

                    if (candidateTargetType == TileType.NonWalkable
                     || !targetGrid.IsTileFree(candidateSlideTarget, true))
                        break;

                    slideTarget = candidateSlideTarget;
                    newTargetType = candidateTargetType;

                    steps++;
                }
                while (newTargetType == TileType.Slippery);

                // Only slide if the target is more than a tile away.
                if (slideTarget != targetPosition)
                {
                    List<Sprite> slideFlipbook = GetSpriteContainer().GetWalking(direction, false, false, true);

                    // We have to manually animate the sprite so it looks like the character is sliding.
                    ChangeSprite(slideFlipbook[foot]);

                    yield return MoveToTargetTile(direction,
                                                  currentPosition,
                                                  slideTarget,
                                                  slideFlipbook,
                                                  false,
                                                  false,
                                                  false,
                                                  false,
                                                  true,
                                                  false,
                                                  animateWhirlwind,
                                                  steps * .5f,
                                                  false,
                                                  waitForNextTileToBeFree,
                                                  playerCallback);

                    LookAt(direction, useBikingSprite: IsBiking);

                    IsMoving = false;
                    yield break;
                }
            }

            // Check movement restrictions.
            bool move = CheckCommonTypeMovementRestrictions(targetGrid,
                                                            targetPosition,
                                                            currentPosition,
                                                            currentType,
                                                            targetType,
                                                            topCurrentType,
                                                            topTargetType,
                                                            enterWater);

            if (!move)
            {
                if (waitForNextTileToBeFree)
                    while (!move)
                    {
                        yield return new WaitForSeconds(.1f);

                        move = CheckCommonTypeMovementRestrictions(targetGrid,
                                                                   targetPosition,
                                                                   currentPosition,
                                                                   currentType,
                                                                   targetType,
                                                                   topCurrentType,
                                                                   topTargetType,
                                                                   enterWater);
                    }
                else
                {
                    IsMoving = false;
                    yield break;
                }
            }

            // Pre movement calls.
            foot = foot == 0 ? 1 : 0;

            if (topTargetType == TileType.BridgeEntrance) UpdateBodySortingOrderToTile(topTargetType);

            if (targetType != TileType.Bridge && topTargetType == TileType.Bridge)
                UpdateHeadSortingOrderToBelowBridge();

            CurrentGrid.CharacterAboutToLeaveTileAsync(this, currentPosition);
            targetGrid.CharacterAboutToEnterTileAsync(this, targetPosition);

            bool triggerWilds = true;

            // These calls generate a noticeable frame delay with continuous movement.
            if (CurrentGrid.HasBlockingInteraction(currentPosition))
                yield return CurrentGrid.CharacterAboutToLeaveTile(this,
                                                                   currentPosition,
                                                                   keepTriggering => triggerWilds &= keepTriggering);

            if (targetGrid.HasBlockingInteraction(targetPosition))
                yield return targetGrid.CharacterAboutToEnterTile(this,
                                                                  targetPosition,
                                                                  (keepMoving, keepTriggering) =>
                                                                  {
                                                                      move &= keepMoving;
                                                                      triggerWilds &= keepTriggering;
                                                                  });

            if (!move)
            {
                IsMoving = false;
                yield break;
            }

            // Actually move.
            isChangingTiles = true;

            IsSwimming = targetType == TileType.Water;

            bool jumpMultiplierApplies = false;

            if (jump || enterWater || (currentType == TileType.Water && targetType == TileType.Walkable))
            {
                if (MakeSound) audioManager.PlayAudio(JumpAudio);

                BodyPivot.DOLocalJump(BodyPivot.localPosition, 1, 1, Speed * speedMultiplier * JumpMultiplier);

                jumpMultiplierApplies = true;
            }

            float duration = Speed
                           * speedMultiplier
                           * (runningAndOthersAffectSpeed
                                  ? jumpMultiplierApplies
                                        ? JumpMultiplier
                                        : RunningSpeedMultiplier
                                        * BikingSpeedMultiplier
                                        * SwimmingSpeedMultiplier
                                  : 1);

            TweenerCore<Vector3, Vector3, VectorOptions> tween = Transform
                                                                .DOMove(targetPosition, duration)
                                                                .SetEase(Ease.Linear)
                                                                .OnComplete(() => isChangingTiles = false);

            if (animateSteps || animateWhirlwind)
                tween.OnUpdate(() =>
                               {
                                   float elapsed = tween.ElapsedPercentage();

                                   if (animateWhirlwind)
                                       switch (elapsed)
                                       {
                                           case <= .25f:
                                               LookAt(Direction.Down, false, IsBiking);
                                               break;
                                           case <= .5f:
                                               LookAt(Direction.Left, false, IsBiking);
                                               break;
                                           case <= .75f:
                                               LookAt(Direction.Up, false, IsBiking);
                                               break;
                                           default:
                                               LookAt(Direction.Right, false, IsBiking);
                                               break;
                                       }
                                   else
                                       switch (elapsed)
                                       {
                                           case > .75f:
                                               LookAt(direction, IsRunning, IsBiking);
                                               break;
                                           case < .5f:
                                               ChangeSprite(flipbook[foot]);
                                               break;
                                       }
                               });

            yield return new WaitWhile(() => isChangingTiles);

            // Post movement calls.
            if (targetType == TileType.Walkable && topTargetType != TileType.BridgeEntrance)
                UpdateBodySortingOrderToTile(targetType);

            if (topCurrentType == TileType.Bridge && targetType != TileType.Bridge && topTargetType != TileType.Bridge)
                UpdateHeadSortingOrderToOverBridge();

            if (gameObject.scene != targetGrid.gameObject.scene)
                sceneManager.MoveObjectToScene(gameObject, targetGrid.gameObject.scene.name);

            CurrentGrid.CharacterLeftTileAsync(this, currentPosition);
            targetGrid.CharacterEnterTileAsync(this, targetPosition);

            // These calls generate a noticeable frame delay with continuous movement.
            if (CurrentGrid.HasBlockingInteraction(currentPosition))
                yield return CurrentGrid.CharacterLeftTile(this,
                                                           currentPosition,
                                                           keepTriggering => triggerWilds &= keepTriggering);

            if (targetGrid.HasBlockingInteraction(targetPosition))
                yield return targetGrid.CharacterEnterTile(this,
                                                           targetPosition,
                                                           keepTriggering => triggerWilds &= keepTriggering);

            if (IsPlayer && targetGrid != CurrentGrid)
            {
                CurrentGrid.PlayerExitGrid(GetCachedComponent<PlayerCharacter>());
                targetGrid.PlayerEnterGrid(GetCachedComponent<PlayerCharacter>());
            }

            CurrentGrid = targetGrid;

            // Trigger wild encounters.
            if (!IsPlayer || !triggerWilds)
            {
                IsMoving = false;
                yield break;
            }

            EncounterTile encounterTile = targetGrid
                                         .GetAttachedTileObjectOnPositionDirectlyBelowPlayer(targetPosition,
                                              SortOrder)
                                          // ReSharper disable once WrongIndentSize
                                       ?
                                      .GetComponent<EncounterTile>();

            EncounterType encounterType;

            if (encounterTile == null)
            {
                if (targetGrid.SceneInfo.IsDungeon)
                    encounterType = EncounterType.Cave;
                else
                {
                    IsMoving = false;
                    yield break;
                }
            }
            else
                encounterType = encounterTile.EncounterType;

            float chance = yapuSettings.TileEncounterChance[encounterType];

            if (yapuSettings.EncountersWithModifiableChances.Contains(encounterType))
                chance *= GetCachedComponent<PlayerCharacter>()
                         .PlayerRoster[0]
                         .OnCalculateEncounterChance(GetCachedComponent<PlayerCharacter>(), encounterType);

            // Is player on a bike?
            // https://sha.wn.zone/p/pokemon-encounter-rate
            float roll = Random.value / (IsBiking ? .8f : 1f);

            if (roll < chance) playerCallback?.Invoke(true, encounterType);

            IsMoving = false;

            yield return TriggerMoveEvent();
        }

        /// <summary>
        /// Find the target grid of the movement.
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="targetGrid"></param>
        /// <returns></returns>
        public bool FindTargetGrid(Vector3Int targetPosition, out GridController targetGrid)
        {
            targetGrid = null;

            if (!CurrentGrid.HasTile(targetPosition))
            {
                Logger.Warn("Current grid has no tile on the position "
                          + targetPosition
                          + ". Checking other loaded grids...");

                targetGrid = globalGridManager.FindGridWithTileInPosition(targetPosition);

                if (targetGrid != null) return true;

                Logger.Warn("No grid was found with a tile in the position "
                          + targetPosition
                          + ".");

                return false;
            }

            targetGrid = CurrentGrid;
            return true;
        }

        /// <summary>
        /// Check common movement restrictions related to the tile type.
        /// </summary>
        /// <param name="targetGrid">Target grid to move to.</param>
        /// <param name="currentPosition">Current position moving from.</param>
        /// <param name="targetPosition">Target position to move to.</param>
        /// <param name="currentType">Current type of tile.</param>
        /// <param name="targetType">Target type of tile.</param>
        /// <param name="topCurrentType">Current type on top.</param>
        /// <param name="topTargetType">Target type on top.</param>
        /// <param name="enterWater">Is the character entering water?</param>
        /// <param name="isWaterFalling">Is the character going through a waterfall?</param>
        /// <param name="canExitWater">Can the character exit water?</param>
        /// <param name="logWarnings">Should warnings be logged?</param>
        /// <param name="recalculateTypes">Do the types need recalculation?</param>
        /// <param name="ignorePlayer">Ignore the player and treat them as if they are not an obstacle.</param>
        // ReSharper disable once CyclomaticComplexity
        public bool CheckCommonTypeMovementRestrictions(GridController targetGrid,
                                                        Vector3Int targetPosition,
                                                        Vector3Int currentPosition,
                                                        TileType currentType = TileType.NonExistent,
                                                        TileType targetType = TileType.NonExistent,
                                                        TileType topCurrentType = TileType.NonExistent,
                                                        TileType topTargetType = TileType.NonExistent,
                                                        bool enterWater = false,
                                                        bool isWaterFalling = false,
                                                        bool canExitWater = true,
                                                        bool logWarnings = true,
                                                        bool recalculateTypes = false,
                                                        bool ignorePlayer = false)
        {
            if (recalculateTypes)
            {
                currentType =
                    targetGrid.GetTypeOfTileDirectlyBelowSortOrder(currentPosition, SortOrder);

                topCurrentType =
                    targetGrid.GetTypeOfTileDirectlyBelowSortOrder(currentPosition, OverBridgeBodySortOrder);

                targetType =
                    targetGrid.GetTypeOfTileDirectlyBelowSortOrder(targetPosition, SortOrder);

                topTargetType =
                    targetGrid.GetTypeOfTileDirectlyBelowSortOrder(targetPosition, OverBridgeBodySortOrder);
            }

            if (!targetGrid.IsTileFree(targetPosition, ignorePlayer))
            {
                if (logWarnings)
                    Logger.Warn("Target tile "
                              + targetPosition
                              + " already has someone or something on it.");

                return false;
            }

            if (IsSwimming && currentType == TileType.Water && targetType == TileType.Water) return true;

            if (IsBiking && targetType == TileType.WalkableNotBikable) return false;

            // ReSharper disable twice ConvertIfStatementToSwitchStatement
            if (currentType == TileType.Bridge && targetType is TileType.Walkable or TileType.WalkableNotBikable)
            {
                if (logWarnings) Logger.Warn("Can't jump from bridge.");

                return false;
            }

            if (currentType is TileType.Walkable or TileType.WalkableNotBikable
             && targetType == TileType.Water
             && enterWater)
            {
                if (logWarnings) Logger.Info("Character entering water.");

                return true;
            }

            if (targetType == TileType.Waterfall && isWaterFalling)
            {
                if (logWarnings) Logger.Info("Character going through waterfall.");

                return true;
            }

            if (targetType == TileType.Slippery)
            {
                if (logWarnings) Logger.Info("Character is sliding.");

                return true;
            }

            if (currentType == TileType.Water
             && targetType is TileType.Walkable or TileType.WalkableNotBikable
             && !canExitWater)
                return false;

            if (currentType is TileType.Walkable or TileType.WalkableNotBikable
             && topCurrentType != TileType.Bridge
             && topTargetType == TileType.BridgeEntrance)
            {
                Logger.Info("Entering bridge.");
                IsOverBridge = true;
            }
            else if (currentType == TileType.BridgeEntrance && targetType == TileType.Bridge)
                Logger.Info("Entered bridge.");
            else if (currentType == TileType.BridgeEntrance && targetType == TileType.BridgeEntrance)
                Logger.Info("Walking on bridge edge.");
            else if (currentType == TileType.Bridge && targetType == TileType.Bridge)
                Logger.Info("Walking through bridge.");
            else if (currentType == TileType.Bridge && targetType == TileType.BridgeEntrance)
                Logger.Info("Exiting bridge.");
            else if (currentType == TileType.BridgeEntrance
                  && targetType is TileType.Walkable or TileType.WalkableNotBikable)
            {
                Logger.Info("Exited bridge.");
                IsOverBridge = false;
            }
            else if (targetType != TileType.Walkable && targetType != TileType.WalkableNotBikable)
            {
                if (logWarnings)
                    Logger.Warn("Target tile "
                              + targetPosition
                              + " is not of walkable type. Its type is "
                              + targetType
                              + ".");

                return false;
            }

            return true;
        }

        /// <summary>
        /// Place the character on top a bridge that is in the same position.
        /// </summary>
        public void PlaceOnTopOfBridge()
        {
            UpdateBodySortingOrderToTile(TileType.BridgeEntrance);
            IsOverBridge = true;
        }

        /// <summary>
        /// Update the body sorting order to the current tile.
        /// </summary>
        public void UpdateSortingOrderToCurrentTile()
        {
            Vector3Int position = Transform.position.ToInts();

            TileType tileType = CurrentGrid.GetTypeOfTileDirectlyBelowSortOrder(position, SortOrder);

            TileType topCurrenType = CurrentGrid.GetTypeOfTileDirectlyBelowSortOrder(position, OverBridgeBodySortOrder);

            IsSwimming = tileType == TileType.Water;

            UpdateBodySortingOrderToTile(tileType);

            if (topCurrenType == TileType.Bridge)
                UpdateHeadSortingOrderToBelowBridge();
            else
                UpdateHeadSortingOrderToOverBridge();

            LookAt(CurrentDirection, useBikingSprite: IsBiking);
        }

        /// <summary>
        /// Update the sort order of the sprites to the tile type.
        /// </summary>
        private void UpdateBodySortingOrderToTile(TileType tileType)
        {
            if (tileType is TileType.Bridge or TileType.BridgeEntrance)
            {
                foreach (SpriteRenderer shadowRenderer in ShadowRenderers)
                    shadowRenderer.sortingOrder = OverBridgeShadowSortOrder;

                foreach (SpriteRenderer bodyRenderer in BodyRenderers)
                    bodyRenderer.sortingOrder = OverBridgeBodySortOrder;
            }
            else
            {
                foreach (SpriteRenderer shadowRenderer in ShadowRenderers)
                    shadowRenderer.sortingOrder = NormalShadowSortOrder;

                foreach (SpriteRenderer bodyRenderer in BodyRenderers) bodyRenderer.sortingOrder = NormalBodySortOrder;
            }
        }

        /// <summary>
        /// Update the head sort order to below bridge.
        /// </summary>
        private void UpdateHeadSortingOrderToBelowBridge()
        {
            foreach (SpriteRenderer headRenderer in HeadRenderers) headRenderer.sortingOrder = NormalHeadSortOrder;
        }

        /// <summary>
        /// Update the head sort order to over bridge.
        /// </summary>
        private void UpdateHeadSortingOrderToOverBridge()
        {
            foreach (SpriteRenderer headRenderer in HeadRenderers) headRenderer.sortingOrder = OverBridgeHeadSortOrder;
        }

        /// <summary>
        /// Update all the sort orders to the given value.
        /// </summary>
        /// <param name="sort">Value to update to.</param>
        private void UpdateAllSortOrdersToFixedValue(int sort)
        {
            foreach (SpriteRenderer shadowRenderer in ShadowRenderers) shadowRenderer.sortingOrder = sort;

            foreach (SpriteRenderer bodyRenderer in BodyRenderers) bodyRenderer.sortingOrder = sort;

            foreach (SpriteRenderer headRenderer in HeadRenderers) headRenderer.sortingOrder = sort;
        }

        /// <summary>
        /// Move a vector 1 tile in a direction.
        /// </summary>
        /// <param name="original">Original vector.</param>
        /// <param name="direction">Direction to move in.</param>
        /// <returns>A new moved vector.</returns>
        public static Vector3Int MoveOneInDirection(Vector3Int original, Direction direction)
        {
            Vector3Int targetPosition = original;

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (direction)
            {
                case Direction.Down:
                    targetPosition.y--;
                    break;
                case Direction.Up:
                    targetPosition.y++;
                    break;
                case Direction.Left:
                    targetPosition.x--;
                    break;
                case Direction.Right:
                    targetPosition.x++;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            return targetPosition;
        }

        /// <summary>
        /// Change the character's sprite.
        /// </summary>
        /// <param name="sprite">Sprite to set.</param>
        public void ChangeSprite(Sprite sprite)
        {
            foreach (SpriteRenderer spriteRenderer in SpriteRenderers)
            {
                if (spriteRenderer.sprite == sprite) return;
                spriteRenderer.sprite = sprite;
            }
        }

        /// <summary>
        /// Enable or disable the characters shadow.
        /// </summary>
        /// <param name="enable">Enable or disable?</param>
        public void EnableShadow(bool enable)
        {
            foreach (SpriteRenderer shadowRenderer in ShadowRenderers) shadowRenderer.enabled = enable;
        }

        /// <summary>
        /// Get the localization key of the name.
        /// </summary>
        /// <returns></returns>
        public string GetLocalizedName() =>
            Mode switch
            {
                CharacterMode.Character => localizer[GetCharacterData().LocalizableName],
                CharacterMode.Monster => GetMonsterData().GetNameOrNickName(localizer),
                CharacterMode.Invisible => "InvisibleCharacter",
                _ => throw new ArgumentOutOfRangeException()
            };

        /// <summary>
        /// Get the character data of this character.
        /// </summary>
        public CharacterData GetCharacterData() => Mode != CharacterMode.Character ? null : Character;

        /// <summary>
        /// Get the monster data.
        /// </summary>
        public MonsterInstance GetMonsterData() =>
            Mode != CharacterMode.Monster || MonsterRoster == null ? null : MonsterRoster[0];

        /// <summary>
        /// Get the container for the world sprites.
        /// </summary>
        /// <returns>An IWorldSpriteContainer object.</returns>
        private IWorldDataContainer GetSpriteContainer() =>
            Mode switch
            {
                CharacterMode.Character => GetCharacterData(),
                CharacterMode.Monster => GetMonsterData(),
                #if UNITY_EDITOR
                CharacterMode.Invisible when !Application.isPlaying => AssetManagementUtils
                                                                      .FindAssetsByType<YAPUSettings>()
                                                                      .First()
                                                                      .EmptySpriteContainer,
                #endif
                CharacterMode.Invisible => yapuSettings.EmptySpriteContainer,
                _ => throw new ArgumentOutOfRangeException()
            };

        /// <summary>
        /// Update the name of the object to match the character data.
        /// Only to be used in editor.
        /// </summary>
        [Button]
        [PropertyOrder(-1)]
        private void UpdateName() =>
            name = Mode switch
            {
                CharacterMode.Character => GetCharacterData().name,
                CharacterMode.Monster => GetMonsterData().Species.name,
                CharacterMode.Invisible => "InvisibleCharacter",
                _ => throw new ArgumentOutOfRangeException()
            };

        /// <summary>
        /// Enumerations of the mode this character can be.
        /// </summary>
        public enum CharacterMode
        {
            Character,
            Monster,
            Invisible
        }

        /// <summary>
        /// Directions the character can move in.
        /// </summary>
        public enum Direction
        {
            None,
            Down,
            Up,
            Left,
            Right
        }
    }
}