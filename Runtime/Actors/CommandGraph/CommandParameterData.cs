using System;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.Monster.Trade;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.Quests;
using Varguiniano.YAPU.Runtime.Saves;
using Varguiniano.YAPU.Runtime.UI.Map;
using WhateverDevs.Core.Runtime.Configuration;
using WhateverDevs.Localization.Runtime;
using CharacterController = Varguiniano.YAPU.Runtime.Characters.CharacterController;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph
{
    /// <summary>
    /// Class that holds all the parameters that are passed to each command in the graph.
    /// Each command may add its own parameters to the ICommandParameter[] array.
    /// </summary>
    [Serializable]
    public class CommandParameterData
    {
        /// <summary>
        /// Graph that is running the command.
        /// </summary>
        public ActorCommandGraph Graph;

        /// <summary>
        /// Game object owner of this command.
        /// </summary>
        public GameObject Owner;

        /// <summary>
        /// Actor running the command, if applicable.
        /// </summary>
        public Actor Actor;

        /// <summary>
        /// Player direction, if available.
        /// </summary>
        public CharacterController.Direction PlayerDirection;

        /// <summary>
        /// Reference to the player character, in case it is a player interaction.
        /// </summary>
        public PlayerCharacter PlayerCharacter;

        /// <summary>
        /// Reference to the global game data.
        /// </summary>
        public GlobalGameData GlobalGameData;

        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        public YAPUSettings YAPUSettings;

        /// <summary>
        /// Reference to the monster database.
        /// </summary>
        public MonsterDatabaseInstance MonsterDatabase;

        /// <summary>
        /// Reference to the battle launcher.
        /// </summary>
        public IBattleLauncher BattleLauncher;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        public ILocalizer Localizer;

        /// <summary>
        /// Reference to the configuration manager.
        /// </summary>
        public IConfigurationManager ConfigurationManager;

        /// <summary>
        /// Reference to the input manager.
        /// </summary>
        public IInputManager InputManager;

        /// <summary>
        /// Reference to the teleporter.
        /// </summary>
        public PlayerTeleporter Teleporter;

        /// <summary>
        /// Reference to the savegame manager.
        /// </summary>
        public SavegameManager SavegameManager;

        /// <summary>
        /// Reference to the map scene launcher.
        /// </summary>
        public MapSceneLauncher MapSceneLauncher;

        /// <summary>
        /// Reference to the time manager.
        /// </summary>
        public TimeManager TimeManager;

        /// <summary>
        /// Reference to the quest manager.
        /// </summary>
        public QuestManager QuestManager;

        /// <summary>
        /// Reference to the trade manager.
        /// </summary>
        public TradeManager TradeManager;

        /// <summary>
        /// Is the player using a move on the actor?
        /// </summary>
        public bool UsingMove;

        /// <summary>
        /// Is the player using a move on the actor?
        /// </summary>
        public MonsterInstance MoveUser;

        /// <summary>
        /// Move being used.
        /// </summary>
        public Move Move;

        /// <summary>
        /// Is this command running inside a loop?
        /// </summary>
        public bool IsRunningOnLoop;

        /// <summary>
        /// Extra parameters that can be passed to the command.
        /// </summary>
        public ICommandParameter[] ExtraParams;

        /// <summary>
        /// Callback with result params.
        /// </summary>
        public Action<CommandCallbackParams> Callback;

        /// <summary>
        /// Base constructor.
        /// </summary>
        /// <param name="owner">Game object owner of this command.</param>
        /// <param name="actor">Actor running the command, if applicable.</param>
        /// <param name="playerDirection">Player direction, if available.</param>
        /// <param name="playerCharacter">Reference to the player character, in case it is a player interaction.</param>
        /// <param name="globalGameData">Reference to the global game data.</param>
        /// <param name="yapuSettings">Reference to the YAPU settings.</param>
        /// <param name="monsterDatabase">Reference to the monster database.</param>
        /// <param name="battleLauncher">Reference to the battle launcher.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="configurationManager">Reference to the configuration manager.</param>
        /// <param name="inputManager">Reference to the input manager.</param>
        /// <param name="teleporter">Reference to the teleporter.</param>
        /// <param name="savegameManager">Reference to the savegame manager.</param>
        /// <param name="mapSceneLauncher">Reference to the map scene launcher.</param>
        /// <param name="timeManager">Reference to the time manager.</param>
        /// <param name="questManager">Reference to the quest manager.</param>
        /// <param name="tradeManager">Reference to the trade manager.</param>
        /// <param name="callback">Callback with result params.</param>
        /// <param name="usingMove">Is the player using a move on the actor?</param>
        /// <param name="moveUser">Is the player using a move on the actor?</param>
        /// <param name="move">Move being used.</param>
        /// <param name="isRunningOnLoop">Is this command running inside a loop?</param>
        /// <param name="extraParams">Extra parameters that can be passed to the command.</param>
        public CommandParameterData(GameObject owner,
                                    Actor actor,
                                    CharacterController.Direction playerDirection,
                                    PlayerCharacter playerCharacter,
                                    GlobalGameData globalGameData,
                                    YAPUSettings yapuSettings,
                                    MonsterDatabaseInstance monsterDatabase,
                                    IBattleLauncher battleLauncher,
                                    ILocalizer localizer,
                                    IConfigurationManager configurationManager,
                                    IInputManager inputManager,
                                    PlayerTeleporter teleporter,
                                    SavegameManager savegameManager,
                                    MapSceneLauncher mapSceneLauncher,
                                    TimeManager timeManager,
                                    QuestManager questManager,
                                    TradeManager tradeManager,
                                    Action<CommandCallbackParams> callback,
                                    bool usingMove = false,
                                    MonsterInstance moveUser = null,
                                    Move move = null,
                                    bool isRunningOnLoop = false,
                                    params ICommandParameter[] extraParams)
        {
            Owner = owner;
            Actor = actor;
            PlayerDirection = playerDirection;
            PlayerCharacter = playerCharacter;
            GlobalGameData = globalGameData;
            YAPUSettings = yapuSettings;
            MonsterDatabase = monsterDatabase;
            BattleLauncher = battleLauncher;
            Localizer = localizer;
            ConfigurationManager = configurationManager;
            InputManager = inputManager;
            Teleporter = teleporter;
            SavegameManager = savegameManager;
            MapSceneLauncher = mapSceneLauncher;
            TimeManager = timeManager;
            QuestManager = questManager;
            TradeManager = tradeManager;
            UsingMove = usingMove;
            MoveUser = moveUser;
            Move = move;
            IsRunningOnLoop = isRunningOnLoop;
            ExtraParams = extraParams;
            Callback = callback;
        }
    }
}