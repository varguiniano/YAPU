using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.Battle.PlayerControl;
using Varguiniano.YAPU.Runtime.Battle.Random;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Experience;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Bags;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.Player;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Configuration;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Manager that handles a monster battle.
    /// </summary>
    [RequireComponent(typeof(BattleStateMachine))]
    [RequireComponent(typeof(BattleManagerBattlersModule))]
    [RequireComponent(typeof(BattleManagerBattlerSwitchModule))]
    [RequireComponent(typeof(BattleManagerBattlerStatsModule))]
    [RequireComponent(typeof(BattleManagerHealthModule))]
    [RequireComponent(typeof(BattleManagerMovesModule))]
    [RequireComponent(typeof(BattleManagerItemsModule))]
    [RequireComponent(typeof(BattleManagerCaptureModule))]
    [RequireComponent(typeof(BattleManagerRostersModule))]
    [RequireComponent(typeof(BattleManagerAIModule))]
    [RequireComponent(typeof(BattleManagerScenariosModule))]
    [RequireComponent(typeof(BattleManagerStatusesModule))]
    [RequireComponent(typeof(BattleManagerCharactersModule))]
    [RequireComponent(typeof(BattleManagerAnimationModule))]
    [RequireComponent(typeof(BattleManagerAudioModule))]
    [RequireComponent(typeof(BattleManagerMegaModule))]
    public class BattleManager : WhateverBehaviour<BattleManager>, IBattleManager, IPlayerDataReceiver
    {
        #region Modules

        /// <summary>
        /// Battlers module.
        /// </summary>
        internal BattleManagerBattlersModule Battlers => GetCachedComponent<BattleManagerBattlersModule>();

        /// <summary>
        /// Battler switch module.
        /// </summary>
        internal BattleManagerBattlerSwitchModule BattleManagerBattlerSwitch =>
            GetCachedComponent<BattleManagerBattlerSwitchModule>();

        /// <summary>
        /// Battler stats module.
        /// </summary>
        internal BattleManagerBattlerStatsModule BattlerStats => GetCachedComponent<BattleManagerBattlerStatsModule>();

        /// <summary>
        /// Battler health module.
        /// </summary>
        internal BattleManagerHealthModule BattlerHealth => GetCachedComponent<BattleManagerHealthModule>();

        /// <summary>
        /// Moves module.
        /// </summary>
        internal BattleManagerMovesModule Moves => GetCachedComponent<BattleManagerMovesModule>();

        /// <summary>
        /// Items module.
        /// </summary>
        internal BattleManagerItemsModule Items => GetCachedComponent<BattleManagerItemsModule>();

        /// <summary>
        /// Capture module.
        /// </summary>
        internal BattleManagerCaptureModule Capture => GetCachedComponent<BattleManagerCaptureModule>();

        /// <summary>
        /// Rosters module.
        /// </summary>
        internal BattleManagerRostersModule Rosters => GetCachedComponent<BattleManagerRostersModule>();

        /// <summary>
        /// AI module.
        /// </summary>
        internal BattleManagerAIModule AI => GetCachedComponent<BattleManagerAIModule>();

        /// <summary>
        /// Scenarios module.
        /// </summary>
        internal BattleManagerScenariosModule Scenario => GetCachedComponent<BattleManagerScenariosModule>();

        /// <summary>
        /// Statuses module.
        /// </summary>
        internal BattleManagerStatusesModule Statuses => GetCachedComponent<BattleManagerStatusesModule>();

        /// <summary>
        /// Characters module.
        /// </summary>
        internal BattleManagerCharactersModule Characters => GetCachedComponent<BattleManagerCharactersModule>();

        /// <summary>
        /// Animation module.
        /// </summary>
        internal BattleManagerAnimationModule Animation => GetCachedComponent<BattleManagerAnimationModule>();

        /// <summary>
        /// Audio module.
        /// </summary>
        internal BattleManagerAudioModule Audio => GetCachedComponent<BattleManagerAudioModule>();

        /// <summary>
        /// Mega forms module.
        /// </summary>
        internal BattleManagerMegaModule Megas => GetCachedComponent<BattleManagerMegaModule>();

        /// <summary>
        /// Provider for random numbers generation in battle.
        /// </summary>
        internal IBattleRandomNumbersProvider RandomProvider => GetCachedComponent<IBattleRandomNumbersProvider>();

        #endregion

        #region References

        /// <summary>
        /// Reference to the enemy roster indicators.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        internal BattleRosterIndicator[] EnemyRosterIndicators;

        /// <summary>
        /// Reference to the ally roster indicator.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        internal BattleRosterIndicator AllyRosterIndicator;

        /// <summary>
        /// Reference to the battle camera.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        internal BattleCamera BattleCamera;

        /// <summary>
        /// Reference to the ally sprites.
        /// </summary>
        [FoldoutGroup("References/Monsters")]
        [SerializeField]
        internal BattleMonsterSprite[] AllyBattlerSprites;

        /// <summary>
        /// Reference to the enemy sprites.
        /// </summary>
        [FoldoutGroup("References/Monsters")]
        [SerializeField]
        internal BattleMonsterSprite[] EnemyBattlerSprites;

        /// <summary>
        /// Reference to the ally panels.
        /// </summary>
        [FoldoutGroup("References/Monsters")]
        [SerializeField]
        internal MonsterPanel[] AllyPanels;

        /// <summary>
        /// Reference to the enemy panels.
        /// </summary>
        [FoldoutGroup("References/Monsters")]
        [SerializeField]
        internal MonsterPanel[] EnemyPanels;

        /// <summary>
        /// Reference to the ally sprites.
        /// </summary>
        [FoldoutGroup("References/Trainers")]
        [SerializeField]
        internal BattleTrainerSprite[] AllyTrainerSprites;

        /// <summary>
        /// Reference to the enemy sprites.
        /// </summary>
        [FoldoutGroup("References/Trainers")]
        [SerializeField]
        internal BattleTrainerSprite[] EnemyTrainerSprites;

        /// <summary>
        /// Reference to the PlayerControlManager.
        /// </summary>
        internal IPlayerControlManager PlayerControlManager;

        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        [Inject]
        internal YAPUSettings YAPUSettings;

        /// <summary>
        /// Reference to the player settings.
        /// </summary>
        [Inject]
        internal PlayerSettings PlayerSettings;

        /// <summary>
        /// Reference to the monster database.
        /// </summary>
        [Inject]
        internal MonsterDatabaseInstance MonsterDatabase;

        /// <summary>
        /// Reference to the Dex.
        /// </summary>
        [Inject]
        internal Dex Dex;

        /// <summary>
        /// Reference to the experience lookup table.
        /// </summary>
        [Inject]
        internal ExperienceLookupTable ExperienceLookupTable;

        /// <summary>
        /// Reference to the global game data.
        /// </summary>
        [Inject]
        internal GlobalGameData GlobalGameData;

        /// <summary>
        /// Reference to the time manager.
        /// </summary>
        [Inject]
        internal TimeManager TimeManager;

        /// <summary>
        /// Reference to the battle configuration.
        /// </summary>
        internal BattleConfiguration Configuration;

        /// <summary>
        /// Get the monster sprite of the given type and index.
        /// </summary>
        /// <param name="type">Type of battler to check.</param>
        /// <param name="index">Index of the roster.</param>
        /// <returns>The reference to the monster sprite.</returns>
        public BattleMonsterSprite GetMonsterSprite(BattlerType type, int index) =>
            type switch
            {
                BattlerType.Ally => AllyBattlerSprites[index],
                BattlerType.Enemy => EnemyBattlerSprites[index],
                _ => throw new UnsupportedBattlerException(type)
            };

        /// <summary>
        /// Get the monster sprite for a battler.
        /// This may fail if the monster is not fighting.
        /// </summary>
        /// <param name="battler">Battler to check.</param>
        /// <returns>The reference to their sprite.</returns>
        public BattleMonsterSprite GetMonsterSprite(Battler battler)
        {
            (BattlerType type, int index) = Battlers.GetTypeAndIndexOfBattler(battler);
            return GetMonsterSprite(type, index);
        }

        /// <summary>
        /// Get the monster sprite of the given type and index.
        /// </summary>
        /// <param name="tuple">tuple with the type and index.</param>
        /// <returns>The reference to the monster sprite.</returns>
        public BattleMonsterSprite GetMonsterSprite((BattlerType Type, int Index) tuple) =>
            GetMonsterSprite(tuple.Item1, tuple.Item2);

        #endregion

        #region Configuration

        /// <summary>
        /// Type of this battle.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [ReadOnly]
        public BattleType BattleType;

        /// <summary>
        /// Enemy type in this battle.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [ReadOnly]
        public EnemyType EnemyType;

        #endregion

        #region Initialization

        /// <summary>
        /// Receive the battle launcher injection.
        /// </summary>
        /// <param name="battleLauncher">Reference to the battle launcher.</param>
        /// <param name="localizerReference">Reference to the localizer.</param>
        /// <param name="configurationManager">Reference to the configuration manager.</param>
        /// <param name="playerRosterReference">Reference to the player roster.</param>
        /// <param name="playerControlManagerReference">Reference to the player control manager.</param>
        /// <param name="audioManagerReference">Reference to the audio manager.</param>
        [Inject]
        public void Construct(IBattleLauncher battleLauncher,
                              ILocalizer localizerReference,
                              IConfigurationManager configurationManager,
                              Roster playerRosterReference,
                              IPlayerControlManager playerControlManagerReference,
                              IAudioManager audioManagerReference)
        {
            Localizer = localizerReference;
            Rosters.PlayerRoster = playerRosterReference;
            PlayerControlManager = playerControlManagerReference;
            AudioManager = audioManagerReference;

            if (!configurationManager.GetConfiguration(out Configuration))
            {
                Logger.Error("Could not retrieve battle configuration!");
                return;
            }

            StartCoroutine(InitializeBattle(battleLauncher));
        }

        /// <summary>
        /// Initialize the battle.
        /// </summary>
        /// <param name="battleLauncher">Reference to the battle launcher.</param>
        /// <exception cref="NotImplementedException">Thrown when the battle type and rosters sent are not compatible/supported yet.</exception>
        private IEnumerator InitializeBattle(IBattleLauncher battleLauncher)
        {
            yield return WaitAFrame;

            Battlers.ShrinkAndHideBattlers();

            BattleParameters parameters = battleLauncher.RegisterBattleManager(this);

            if (parameters == null)
            {
                Logger.Error("Battle parameters are null!");
                yield break;
            }

            Logger.Info("Retrieved battle parameters.");

            InitVariables(parameters);

            BattleCamera.FocusOnEnemy(BattleSpeed);

            Rosters.PrepareRosters(parameters);

            Characters.SetupCharacters(parameters);

            AI.SetupAIs(parameters);

            Items.PrepareBags(parameters);

            Battlers.SetAllMonsterSprites();

            Battlers.SetEnemyPanelsAndSprites();

            Battlers.HideAllyIfNotAlliedDoubleBattle();

            Logger.Info("Battle initialized.");

            initializedCallback?.Invoke();

            // Camera needs two frames to catch up.
            yield return WaitAFrame;
            yield return WaitAFrame;

            BattlerStats.RaiseFriendshipOnBattleStart();

            yield return WaitUntilCameraStopped;

            yield return Animation.BattleStartAnimation();

            // If not overriden and an out of battle weather is in place, use that.
            yield return Scenario.SetWeather(parameters.Weather == null
                                          && !PlayerCharacter.IsCurrentWeatherCleared
                                          && PlayerCharacter.CurrentWeather != null
                                                 ? PlayerCharacter.CurrentWeather.InBattleWeather
                                                 : parameters.Weather,
                                             -1);

            yield return BattleManagerBattlerSwitch.SendBattlersIn(BattlerType.Ally);

            yield return Statuses.OnBattleStartedCallbacks();

            GetCachedComponent<BattleStateMachine>().StartBattleLoop();
        }

        /// <summary>
        /// Init variables before starting the battle.
        /// </summary>
        private void InitVariables(BattleParameters parameters)
        {
            BattleType = parameters.BattleType;
            EnemyType = parameters.EnemyType;

            IsBattleOver = false;
            AI.PlayerControlsFirstRoster = parameters.PlayerControlsFirstRoster;
            IsFightAvailable = parameters.IsFightAvailable;
            IsMonstersMenuAvailable = parameters.IsMonstersMenuAvailable;
            IsBagAvailable = parameters.IsBagAvailable;
            EnemyTrainersAfterBattleDialogKeys = parameters.EnemyTrainersAfterBattleDialogKeys;
            PlayerCharacter = parameters.PlayerCharacter;

            Scenario.SetScenario(parameters.Scenario, parameters.EncounterType, parameters.PlayerCharacter.Scene.Asset);

            respawnPlayerIfLose = parameters.RespawnPlayerOnLoose;

            WaitUntilCameraStopped = new WaitUntil(CameraIsStopped);
        }

        #endregion

        #region CleanUp

        /// <summary>
        /// Finish the battle and raise the result event.
        /// </summary>
        public void FinishBattle()
        {
            BattleResultParameters battleResultParameters = new()
                                                            {
                                                                BattleType = BattleType,
                                                                PlayerWon = !PlayerLost,
                                                                CapturedMonster = Capture.CapturedMonster,
                                                                RespawnIfLose = respawnPlayerIfLose
                                                            };

            if (AI.PlayerControlsFirstRoster)
            {
                List<Battler> roster = new()
                                       {
                                           null,
                                           null,
                                           null,
                                           null,
                                           null,
                                           null
                                       };

                List<Battler> firstRoster = Rosters.GetRoster(BattlerType.Ally, 0);

                for (int i = 0; i < Rosters.FirstRosterIndexes.Length; i++)
                {
                    int index = Rosters.FirstRosterIndexes[i];
                    roster[index] = firstRoster.Count > i ? firstRoster[i] : default;
                }

                battleResultParameters.PlayerRoster = roster;
            }

            battleFinished?.Invoke(battleResultParameters);
        }

        #endregion

        #region Events And Variables

        /// <summary>
        /// Callback when the battle has initialized.
        /// </summary>
        private Action initializedCallback;

        /// <summary>
        /// Callback when the battle has finished.
        /// </summary>
        private Action<BattleResultParameters> battleFinished;

        /// <summary>
        /// Reference to the audio manager.
        /// </summary>
        internal IAudioManager AudioManager;

        /// <summary>
        /// Is the fight menu available?
        /// </summary>
        [FoldoutGroup("Variables")]
        [ReadOnly]
        public bool IsFightAvailable;

        /// <summary>
        /// Is the monsters menu available?
        /// </summary>
        [FoldoutGroup("Variables")]
        [ReadOnly]
        public bool IsMonstersMenuAvailable;

        /// <summary>
        /// Is the bag available to the trainers?
        /// </summary>
        [FoldoutGroup("Variables")]
        [ReadOnly]
        public bool IsBagAvailable;

        /// <summary>
        /// Public access to the action order for this turn.
        /// </summary>
        public Queue<Battler> CurrentTurnActionOrder;

        /// <summary>
        /// Public access tot he actions for this turn.
        /// </summary>
        public SerializableDictionary<Battler, BattleAction> CurrentTurnActions { get; internal set; }
        
        /// <summary>
        /// Priority brackets for this turn, each battler will have a number indicating that bracket.
        /// </summary>
        public SerializableDictionary<Battler, int> CurrentTurnPriorityBrackets { get; internal set; }

        /// <summary>
        /// Flag to mark that the battle is over.
        /// </summary>
        [FoldoutGroup("Variables")]
        [ReadOnly]
        [PropertyOrder(-1)]
        public bool IsBattleOver;

        /// <summary>
        /// Count of turns that have passed.
        /// </summary>
        [FoldoutGroup("Variables")]
        [ReadOnly]
        [ShowInInspector]
        public int TurnCounter { get; private set; }

        /// <summary>
        /// Times the player tried to run away.
        /// </summary>
        [FoldoutGroup("Variables")]
        [ReadOnly]
        public uint RunAwayAttempts;

        /// <summary>
        /// Flag to mark that someone run away.
        /// We actually don't care who was to finish the battle.
        /// </summary>
        [FoldoutGroup("Variables")]
        [ReadOnly]
        public bool DidRunAway;

        /// <summary>
        /// Speed at which the battle should animate.
        /// It should be between 1 and a reasonable number.
        /// </summary>
        [FoldoutGroup("Variables")]
        [ReadOnly]
        [ShowInInspector]
        public byte BattleSpeed => Configuration.BattleAnimationSpeedUp;

        /// <summary>
        /// Dialog to be said by the enemy trainers after the battle.
        /// </summary>
        [FoldoutGroup("Variables")]
        [ReadOnly]
        public string[] EnemyTrainersAfterBattleDialogKeys;

        /// <summary>
        /// Reference to the player character.
        /// </summary>
        [HideInInspector]
        public PlayerCharacter PlayerCharacter;

        /// <summary>
        /// Respawn the player if they lose?
        /// </summary>
        private bool respawnPlayerIfLose;

        /// <summary>
        /// Flag to know if the player can run from this battle.
        /// </summary>
        public bool CanPlayerRun =>
            EnemyType == EnemyType.Wild
         && Battlers.GetBattlersFighting(BattlerType.Enemy)
                    .All(battler => !battler.OriginData.IsAlpha);

        /// <summary>
        /// Flag to know if the player lost.
        /// </summary>
        internal bool PlayerLost => Rosters.AreAllBattlersOfTypeFainted(BattlerType.Ally);

        /// <summary>
        /// Wait until the camera has stopped.
        /// </summary>
        internal WaitUntil WaitUntilCameraStopped;

        /// <summary>
        /// Register a callback for the initialization of the battle manager.
        /// </summary>
        /// <param name="callback">Callback called when its initialized.</param>
        public void SubscribeToBattleInitialized(Action callback) => initializedCallback = callback;

        /// <summary>
        /// Subscribe to the event of the battle has finished.
        /// </summary>
        /// <param name="callback">Callback when finished.</param>
        public void SubscribeToBattleFinished(Action<BattleResultParameters> callback) => battleFinished = callback;

        /// <summary>
        /// Change the turn number.
        /// </summary>
        public void TickTurn() => TurnCounter++;

        #endregion

        #region Battlers

        /// <summary>
        /// Exception saying that the given battler type is not supported.
        /// </summary>
        internal class UnsupportedBattlerException : ArgumentOutOfRangeException
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="type">Type that was sent.</param>
            public UnsupportedBattlerException(BattlerType type) : base(nameof(type),
                                                                        type,
                                                                        "Battler type not supported.")
            {
            }
        }

        #endregion

        #region Items

        /// <summary>
        /// Reference to the player bag.
        /// </summary>
        [Inject]
        internal Bag PlayerBag;

        #endregion

        #region Dialogs

        /// <summary>
        /// Method to check if the camera is stopped in a stable position.
        /// </summary>
        /// <returns></returns>
        private bool CameraIsStopped() => !BattleCamera.CamerasMoving;

        #endregion

        #region Localization

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        internal ILocalizer Localizer;

        #endregion
    }
}