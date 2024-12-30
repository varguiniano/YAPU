using System;
using System.Collections.Generic;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Scenarios;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Bags;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Natures;
using Varguiniano.YAPU.Runtime.World;
using Varguiniano.YAPU.Runtime.World.Encounters;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Interface that defines a launcher for monster battles.
    /// </summary>
    public interface IBattleLauncher
    {
        /// <summary>
        /// Flag to know if there is already a battle in progress.
        /// </summary>
        bool BattleInProgress { get; }

        /// <summary>
        /// Launch a single wild encounter for the player.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="encounter">Encounter to launch.</param>
        /// <param name="level">Level to launch the encounter at.</param>
        /// <param name="nature"></param>
        /// <param name="sceneInfo">Information of the scene the encounter is triggered in.</param>
        /// <param name="weather">Starting weather.</param>
        /// <param name="encounterType">Type of encounter triggered.</param>
        /// <param name="endCallback">Callback for when the battle is over.</param>
        void LaunchSingleWildEncounter(PlayerCharacter playerCharacter,
                                       WildEncounter encounter,
                                       byte level,
                                       Nature nature,
                                       SceneInfo sceneInfo,
                                       Weather weather,
                                       EncounterType encounterType,
                                       Action<BattleResultParameters> endCallback);

        /// <summary>
        /// Launch a single wild encounter for the player.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="wildRoster">Roster of the wild monster.</param>
        /// <param name="sceneInfo">Information of the scene the encounter is triggered in.</param>
        /// <param name="weather">Starting weather.</param>
        /// <param name="encounterType">Type of encounter triggered.</param>
        /// <param name="endCallback">Callback for when the battle is over.</param>
        void LaunchSingleWildEncounter(PlayerCharacter playerCharacter,
                                       Roster wildRoster,
                                       SceneInfo sceneInfo,
                                       Weather weather,
                                       EncounterType encounterType,
                                       Action<BattleResultParameters> endCallback);

        /// <summary>
        /// Launch a single wild encounter for the player.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="wildRoster">Roster of the wild monster.</param>
        /// <param name="sceneInfo">Information of the scene the encounter is triggered in.</param>
        /// <param name="weather">Starting weather.</param>
        /// <param name="encounterType">Type of encounter triggered.</param>
        /// <param name="endCallback">Callback for when the battle is over.</param>
        void LaunchDoubleWildEncounter(PlayerCharacter playerCharacter,
                                       List<Roster> wildRoster,
                                       SceneInfo sceneInfo,
                                       Weather weather,
                                       EncounterType encounterType,
                                       Action<BattleResultParameters> endCallback);

        /// <summary>
        /// Launch a single trainer encounter for the player.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="trainerRoster">Roster of the trainer to battle.</param>
        /// <param name="trainerBag">Bag of the trainer to battle.</param>
        /// <param name="characterData">Character data of the trainer to battle.</param>
        /// <param name="sceneInfo">Information of the scene the encounter is triggered in.</param>
        /// <param name="weather">Starting weather.</param>
        /// <param name="encounterType">Type of encounter triggered.</param>
        /// <param name="afterBattleDialog">Localization key for the dialog when the trainer is defeated.</param>
        /// <param name="respawnPlayerIfLose">Respawn the player if they lose?</param>
        /// <param name="endCallback">Callback for when the battle is over.</param>
        public void LaunchSingleTrainerEncounter(PlayerCharacter playerCharacter,
                                                 Roster trainerRoster,
                                                 Bag trainerBag,
                                                 CharacterData characterData,
                                                 SceneInfo sceneInfo,
                                                 Weather weather,
                                                 EncounterType encounterType,
                                                 string afterBattleDialog,
                                                 bool respawnPlayerIfLose,
                                                 Action<BattleResultParameters> endCallback);

        /// <summary>
        /// Launch a battle.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="template">Object containing the battle parameters.</param>
        /// <param name="endCallback">Callback for when the battle is over.</param>
        void LaunchBattle(PlayerCharacter playerCharacter,
                          BattleTemplate template,
                          Action<BattleResultParameters> endCallback);

        /// <summary>
        /// Launch a battle.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="battleType">Type of battle to launch.</param>
        /// <param name="enemyType">Enemy type in this battle.</param>
        /// <param name="rosters">Rosters to use.</param>
        /// <param name="characters">Characters that will fight in this battle.</param>
        /// <param name="playerControlsFirstRoster">Flag to know if the player controls the first roster or it's controller by an AI.</param>
        /// <param name="ais">AIs that will control each roster of the battle.</param>
        /// <param name="bags">Bags to use in this battle.</param>
        /// <param name="isFightAvailable">Is the fight menu available?</param>
        /// <param name="isMonstersMenuAvailable">Is the monster's menu available?</param>
        /// <param name="isBagAvailable">Is the bag available to the trainers?</param>
        /// <param name="weather">Starting battle weather.</param>
        /// <param name="encounterType">Type of encounter this battle is.</param>
        /// <param name="endCallback">Callback for when the battle is over.</param>
        /// <param name="scenario">Scenario to use in this battle.</param>
        /// <param name="enemyTrainersAfterBattleDialogKeys">Dialog to be said by the enemy trainers after tha battle.</param>
        /// <param name="respawnPlayerIfLose">Respawn the player if they lose?</param>
        void LaunchBattle(PlayerCharacter playerCharacter,
                          BattleType battleType,
                          EnemyType enemyType,
                          Roster[] rosters,
                          CharacterData[] characters,
                          bool playerControlsFirstRoster,
                          BattleAI[] ais,
                          Bag[] bags,
                          bool isFightAvailable,
                          bool isMonstersMenuAvailable,
                          bool isBagAvailable,
                          BattleScenario scenario,
                          Weather weather,
                          EncounterType encounterType,
                          Action<BattleResultParameters> endCallback,
                          string[] enemyTrainersAfterBattleDialogKeys = null,
                          bool respawnPlayerIfLose = true);

        /// <summary>
        /// Retrieve the battle parameters.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The battle parameters.</returns>
        BattleParameters RegisterBattleManager(IBattleManager battleManager);

        /// <summary>
        /// Reset the battle launcher.
        /// </summary>
        void Reset();

        /// <summary>
        /// Raised when a battle has been launched.
        /// </summary>
        void SubscribeToBattleLaunched(Action subscriber);

        /// <summary>
        /// Raised when a battle has been launched.
        /// </summary>
        void UnsubscribeFromBattleLaunched(Action subscriber);

        /// <summary>
        /// Raised when a battle has ended.
        /// </summary>
        void SubscribeToBattleToUnload(Action subscriber);

        /// <summary>
        /// Raised when a battle has ended.
        /// </summary>
        void UnsubscribeFromBattleToUnload(Action subscriber);

        /// <summary>
        /// Raised when a battle has ended.
        /// </summary>
        void SubscribeToBattleEnded(Action<BattleResultParameters> subscriber);

        /// <summary>
        /// Raised when a battle has ended.
        /// </summary>
        void UnsubscribeFromBattleEnded(Action<BattleResultParameters> subscriber);
    }
}