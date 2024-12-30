using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.Monster.Trade;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.Quests;
using Varguiniano.YAPU.Runtime.Saves;
using Varguiniano.YAPU.Runtime.UI.Map;
using Varguiniano.YAPU.Runtime.World;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Configuration;
using WhateverDevs.Localization.Runtime;
using Zenject;
using CharacterController = Varguiniano.YAPU.Runtime.Characters.CharacterController;

namespace Varguiniano.YAPU.Runtime.Actors
{
    /// <summary>
    /// Base class for all actors the player can find in the world and interact with.
    /// </summary>
    public abstract class Actor : WhateverBehaviour<Actor>, IPlayerInteractable, IPlayerDataReceiver
    {
        /// <summary>
        /// Property to know if this actor blocks character movement.
        /// </summary>
        // This is actually used, it's a false positive by Rider.
        // ReSharper disable once UnusedMemberInSuper.Global
        public abstract bool BlocksMovement { get; }

        /// <summary>
        /// On enable commands.
        /// </summary>
        [SerializeField]
        [SerializeReference]
        [InfoBox("These commands run on enable.")]
        protected ActorCommandGraph OnEnableCommandGraph;

        /// <summary>
        /// Run the loop on enable?
        /// </summary>
        [FoldoutGroup("Loop")]
        [SerializeField]
        private bool RunLoopOnEnable;

        /// <summary>
        /// Commands that are run on loop.
        /// </summary>
        [FoldoutGroup("Loop")]
        [SerializeField]
        [SerializeReference]
        [InfoBox("These commands run on loop while no other commands are running."
               + " The loop can be started and stopped via a command.")]
        protected ActorCommandGraph LoopCommandGraph;

        /// <summary>
        /// Flag to know if the actor is looping.
        /// </summary>
        [FoldoutGroup("Loop")]
        [ReadOnly]
        [HideInEditorMode]
        [ShowInInspector]
        public bool IsLooping => ShouldLoop && !GlobalGridManager.StopAllActors;

        /// <summary>
        /// Flag to mark that the player interacts with this actor when it's on top of it, not in front.
        /// </summary>
        [FoldoutGroup("Other")]
        [SerializeField]
        [SerializeReference]
        [InfoBox("Flag to mark that the player interacts with this actor when it's on top of it, not in front.")]
        private bool PlayerInteractsWhenOnTop;

        /// <summary>
        /// Commands that are triggered when the player interacts.
        /// </summary>
        [FoldoutGroup("Other")]
        [SerializeField]
        [SerializeReference]
        [InfoBox("These commands run when the player interacts with this actor.")]
        protected ActorCommandGraph InteractCommandGraph;

        /// <summary>
        /// Commands that are triggered on demand.
        /// There can be several graphs of commands to run.
        /// </summary>
        [FoldoutGroup("Other/OnDemand")]
        [SerializeField]
        [SerializeReference]
        [InfoBox("These commands can only be run via another command.")]
        public ActorCommandGraph OnDemandCommandGraph0;

        /// <summary>
        /// Commands that are triggered on demand.
        /// There can be several graphs of commands to run.
        /// </summary>
        [FoldoutGroup("Other/OnDemand")]
        [SerializeField]
        [SerializeReference]
        [InfoBox("These commands can only be run via another command.")]
        public ActorCommandGraph OnDemandCommandGraph1;

        /// <summary>
        /// Commands that are triggered on demand.
        /// There can be several graphs of commands to run.
        /// </summary>
        [FoldoutGroup("Other/OnDemand")]
        [SerializeField]
        [SerializeReference]
        [InfoBox("These commands can only be run via another command.")]
        public ActorCommandGraph OnDemandCommandGraph2;

        /// <summary>
        /// Commands that are triggered on demand.
        /// There can be several graphs of commands to run.
        /// </summary>
        [FoldoutGroup("Other/OnDemand")]
        [SerializeField]
        [SerializeReference]
        [InfoBox("These commands can only be run via another command.")]
        public ActorCommandGraph OnDemandCommandGraph3;

        /// <summary>
        /// Flag to know if the actor is running any command.
        /// </summary>
        [HideInInspector]
        public bool RunningCommands;

        /// <summary>
        /// Reference to this actor's temporal variables storage.
        /// </summary>
        public TemporalVariables TemporalVariables =>
            TryGetCachedComponent(out TemporalVariables temporalVariables)
                ? temporalVariables
                : gameObject.AddComponent<TemporalVariables>();

        /// <summary>
        /// This actor's position.
        /// </summary>
        public Vector3Int Position => transform.position.ToInts();

        /// <summary>
        /// Reference to the battle launcher.
        /// </summary>
        [Inject]
        protected IBattleLauncher BattleLauncher;

        /// <summary>
        /// Reference to the global grid manager.
        /// </summary>
        [Inject]
        [HideInInspector]
        public GlobalGridManager GlobalGridManager;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        protected ILocalizer Localizer;

        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        [Inject]
        protected YAPUSettings Settings;

        /// <summary>
        /// Reference to the database.
        /// </summary>
        [Inject]
        protected MonsterDatabaseInstance Database;

        /// <summary>
        /// Reference to the input manager.
        /// </summary>
        [Inject]
        protected IInputManager InputManager;

        /// <summary>
        /// Reference to the player teleporter.
        /// </summary>
        [Inject]
        protected PlayerTeleporter PlayerTeleporter;

        /// <summary>
        /// Reference to the configuration manager.
        /// </summary>
        [Inject]
        protected IConfigurationManager ConfigurationManager;

        /// <summary>
        /// Reference to the global game data.
        /// </summary>
        [Inject]
        protected GlobalGameData GlobalGameData;

        /// <summary>
        /// Reference to the savegame manager.
        /// </summary>
        [Inject]
        protected SavegameManager SavegameManager;

        /// <summary>
        /// Reference to the map scene launcher.
        /// </summary>
        [Inject]
        protected MapSceneLauncher MapSceneLauncher;

        /// <summary>
        /// Reference to the time manager.
        /// </summary>
        [Inject]
        protected TimeManager TimeManager;

        /// <summary>
        /// Reference to the player's quest manager.
        /// </summary>
        [Inject]
        protected QuestManager QuestManager;

        /// <summary>
        /// Reference to the trade manager.
        /// </summary>
        [Inject]
        protected TradeManager TradeManager;

        /// <summary>
        /// Flag to keep looping.
        /// </summary>
        private bool runLooping;

        /// <summary>
        /// Flag to know when the loop is running.
        /// </summary>
        public bool LoopRunning;

        /// <summary>
        /// Flag to know if the actor is looping.
        /// </summary>
        [HideInInspector]
        public bool ShouldLoop;

        /// <summary>
        /// Flag to know if the actor is processing stuff.
        /// </summary>
        protected bool Processing => RunningCommands || GlobalGridManager.StopAllActors;

        /// <summary>
        /// Start tick routine.
        /// </summary>
        protected virtual void OnEnable()
        {
            runLooping = true;

            // TODO: Remove.
            //StartCoroutine(Loop());
            StartCoroutine(RunLoopGraph());

            if (RunLoopOnEnable) ShouldLoop = true;

            // TODO: Remove.
            //StartCoroutine(RunOnEnableCommands());
            StartCoroutine(RunOnEnableCommandGraph());
        }

        /// <summary>
        /// Run the on enable commands.
        /// </summary>
        private IEnumerator RunOnEnableCommandGraph()
        {
            RunningCommands = true;

            bool wasLooping = ShouldLoop;

            ShouldLoop = false;

            if (OnEnableCommandGraph != null)
                yield return OnEnableCommandGraph.Run(new CommandParameterData(gameObject,
                                                                               this,
                                                                               CharacterController.Direction.None,
                                                                               null,
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
                                                                               callbackParams =>
                                                                               {
                                                                                   if (callbackParams.UpdateLooping)
                                                                                       wasLooping =
                                                                                           callbackParams.NewLooping;
                                                                               }));

            RunningCommands = false;

            ShouldLoop = wasLooping;
        }

        /// <summary>
        /// Disable ticking.
        /// </summary>
        protected virtual void OnDisable() => runLooping = false;

        /// <summary>
        /// Run the loop graph in a loop.
        /// </summary>
        private IEnumerator RunLoopGraph()
        {
            if (LoopCommandGraph == null || LoopCommandGraph.IsGraphEmpty) yield break;

            while (runLooping)
                yield return LoopCommandGraph.Run(new CommandParameterData(gameObject,
                                                                           this,
                                                                           CharacterController.Direction.None,
                                                                           null,
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
                                                                           _ =>
                                                                           {
                                                                           },
                                                                           isRunningOnLoop: true));
        }

        /// <summary>
        /// Function to check if the loop should hold.
        /// </summary>
        internal bool ShouldContinueLoop() => IsLooping && !Processing;

        /// <summary>
        /// Flag to mark that the player interacts with this actor when it's on top of it, not in front.
        /// </summary>
        public bool InteractsWhenOnTop() => PlayerInteractsWhenOnTop;

        /// <summary>
        /// Called when the player interacts with this element.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="playerDirection">Direction the player is at.</param>
        public virtual IEnumerator Interact(PlayerCharacter playerCharacter,
                                            CharacterController.Direction playerDirection)
        {
            if (Processing) yield break;

            Vector3Int interactPosition = Position;

            RunningCommands = true;

            bool wasLooping = ShouldLoop;

            ShouldLoop = false;

            yield return new WaitWhile(() => LoopRunning);

            if (Position == interactPosition)
            {
                GlobalGridManager.StopAllActors = true;

                if (InteractCommandGraph != null)
                    yield return InteractCommandGraph.Run(new CommandParameterData(gameObject,
                                                              this,
                                                              playerDirection,
                                                              playerCharacter,
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
                                                              callbackParams =>
                                                              {
                                                                  if (callbackParams.UpdateLooping)
                                                                      wasLooping =
                                                                          callbackParams.NewLooping;
                                                              }));

                GlobalGridManager.StopAllActors = false;
            }

            RunningCommands = false;

            ShouldLoop = wasLooping;
        }

        /// <summary>
        /// Run the on demand commands.
        /// </summary>
        /// <param name="index">On demand graph to run.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="playerDirection">Player direction, if available.</param>
        /// <param name="callback">Callback with result params.</param>
        public IEnumerator RunOnDemandCommandGraphs(int index,
                                                    PlayerCharacter playerCharacter,
                                                    CharacterController.Direction playerDirection,
                                                    Action<CommandCallbackParams> callback = null)
        {
            RunningCommands = true;

            bool wasLooping = ShouldLoop;

            ShouldLoop = false;

            ActorCommandGraph graphToRun = index switch
            {
                1 => OnDemandCommandGraph1,
                2 => OnDemandCommandGraph2,
                3 => OnDemandCommandGraph3,
                _ => OnDemandCommandGraph0
            };

            yield return graphToRun
               .Run(new CommandParameterData(gameObject,
                                             this,
                                             playerDirection,
                                             playerCharacter,
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
                                             callbackParams =>
                                             {
                                                 if (callbackParams.UpdateLooping)
                                                     wasLooping =
                                                         callbackParams.NewLooping;

                                                 callback?.Invoke(callbackParams);
                                             }));

            RunningCommands = false;

            ShouldLoop = wasLooping;
        }

        /// <summary>
        /// Get the grid this actor is in.
        /// </summary>
        public abstract GridController GetCurrentGrid();

        /// <summary>
        /// Run the on demand commands.
        /// </summary>
        [Button("Run on demand commands")]
        [HideInEditorMode]
        private void TestRunOnDemandCommands(int index) =>
            StartCoroutine(RunOnDemandCommandGraphs(index, null, CharacterController.Direction.None));

        #if UNITY_EDITOR

        /// <summary>
        /// Is this actor linked to a prefab?
        /// </summary>
        private bool IsLinkedToPrefab =>
            PrefabUtility.GetCorrespondingObjectFromSource(gameObject) != null
         || PrefabUtility.GetPrefabInstanceHandle(gameObject) != null;

        /// <summary>
        /// Method to unpack the prefab the actor used as template so the graphs are not accidentally modified.
        /// </summary>
        [Button(ButtonSizes.Medium)]
        [PropertyOrder(-2)]
        [InfoBox("This Actor is linked to a prefab. It is recommended to unpack it so you don't accidentally modify the graphs in the prefab.",
                 InfoMessageType.Warning)]
        [ShowIf(nameof(IsLinkedToPrefab))]
        [PropertySpace(0, 30)]
        private void UnpackPrefab() =>
            PrefabUtility.UnpackPrefabInstance(gameObject,
                                               PrefabUnpackMode.Completely,
                                               InteractionMode.UserAction);

        /// <summary>
        /// Snap the actor to the grid.
        /// Only to be used in editor.
        /// </summary>
        [Button]
        [PropertyOrder(-1)]
        private void SnapToGrid() => transform.position = new Vector3(Position.x, Position.y, 0);

        #endif
    }
}