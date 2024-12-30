using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Scenarios;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.Monster.Evolution;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Bags;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Natures;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.World;
using Varguiniano.YAPU.Runtime.World.Encounters;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Scriptable in charge of launching a battle and unloading it when finished.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Battle/Launcher", fileName = "BattleLauncher")]
    public class BattleLauncher : WhateverScriptable<BattleLauncher>, IBattleLauncher, IPlayerDataReceiver
    {
        /// <summary>
        /// Reference to the battle scene.
        /// </summary>
        [FoldoutGroup("References")]
        [PropertyOrder(2)]
        public SceneReference BattleScene;

        /// <summary>
        /// List of music for wild battles.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [PropertyOrder(1)]
        public List<AudioReference> WildBattleMusic;

        /// <summary>
        /// List of music for trainer battles.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [PropertyOrder(1)]
        public List<AudioReference> TrainerBattleMusic;

        /// <summary>
        /// Default battle AI.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private BattleAI DefaultAI;

        /// <summary>
        /// Results of the last battle.
        /// </summary>
        [FoldoutGroup("Last battle")]
        [ReadOnly]
        public BattleResultParameters LastBattleResult;

        /// <summary>
        /// Flag to know if there is already a battle in progress.
        /// </summary>
        [ShowInInspector]
        [ReadOnly]
        public bool BattleInProgress { get; private set; }

        /// <summary>
        /// Raised when a battle has been launched.
        /// </summary>
        public Action OnBattleLaunched;

        /// <summary>
        /// Called when a battle is going to be unloaded.
        /// </summary>
        public Action OnBattleToUnload;

        /// <summary>
        /// Raised when a battle has ended.
        /// </summary>
        public Action<BattleResultParameters> OnBattleEnded;

        /// <summary>
        /// Reference to the scene manager.
        /// </summary>
        [Inject]
        private ISceneManager sceneManager;

        /// <summary>
        /// Reference to the player roster.
        /// </summary>
        [Inject]
        private Roster playerRoster;

        /// <summary>
        /// Reference to the player character.
        /// </summary>
        [Inject]
        private CharacterData playerCharacterData;

        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        [Inject]
        private YAPUSettings settings;

        /// <summary>
        /// Reference to the monster database.
        /// </summary>
        [Inject]
        private MonsterDatabaseInstance database;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Reference to the audio manager.
        /// </summary>
        [Inject]
        private IAudioManager audioManager;

        /// <summary>
        /// Reference to the global game data.
        /// </summary>
        [Inject]
        private GlobalGameData globalGameData;

        /// <summary>
        /// Reference to the player teleporter.
        /// </summary>
        [Inject]
        private PlayerTeleporter playerTeleporter;

        /// <summary>
        /// Reference to the evolution manager.
        /// </summary>
        [Inject]
        private EvolutionManager evolutionManager;

        /// <summary>
        /// Reference to the end battle callback.
        /// </summary>
        private Action<BattleResultParameters> endCallbackReference;

        /// <summary>
        /// Parameters for this battle.
        /// </summary>
        private BattleParameters battleParameters;

        /// <summary>
        /// Reference to the battle manager.
        /// </summary>
        private IBattleManager battleManagerReference;

        /// <summary>
        /// Reference to the player character.
        /// </summary>
        private PlayerCharacter playerCharacterReference;

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
        public void LaunchSingleWildEncounter(PlayerCharacter playerCharacter,
                                              WildEncounter encounter,
                                              byte level,
                                              Nature nature,
                                              SceneInfo sceneInfo,
                                              Weather weather,
                                              EncounterType encounterType,
                                              Action<BattleResultParameters> endCallback)
        {
            Form form = encounter.FormCalculator.GetEncounterForm(sceneInfo, encounterType);

            if (form.HasShinyVersion)
            {
                float shinnyRoll = UnityEngine.Random.value;

                Logger.Info("Shiny roll: " + shinnyRoll + " of " + settings.WildShinyChance + ".");

                if (shinnyRoll <= settings.WildShinyChance) form = form.ShinyVersion;
            }

            Item heldItem = null;

            float itemRoll = UnityEngine.Random.value;

            float accumulatedChance = 0;

            foreach (KeyValuePair<Item, float> wildHeldItemChanceSlot in encounter.Monster[form].WildHeldItems)
            {
                accumulatedChance += wildHeldItemChanceSlot.Value;

                if (itemRoll > accumulatedChance) continue;

                heldItem = wildHeldItemChanceSlot.Key;
                break;
            }

            Roster roster = CreateInstance<Roster>();

            roster.Settings = settings;
            roster.Database = database;

            roster.AddMonster(encounter.Monster, form, level, nature: nature, heldItem: heldItem);

            LaunchSingleWildEncounter(playerCharacter, roster, sceneInfo, weather, encounterType, endCallback);
        }

        /// <summary>
        /// Launch a single wild encounter for the player.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="wildRoster">Roster of the wild monster.</param>
        /// <param name="sceneInfo">Information of the scene the encounter is triggered in.</param>
        /// <param name="weather">Starting weather.</param>
        /// <param name="encounterType">Type of encounter triggered.</param>
        /// <param name="endCallback">Callback for when the battle is over.</param>
        public void LaunchSingleWildEncounter(PlayerCharacter playerCharacter,
                                              Roster wildRoster,
                                              SceneInfo sceneInfo,
                                              Weather weather,
                                              EncounterType encounterType,
                                              Action<BattleResultParameters> endCallback) =>
            LaunchBattle(playerCharacter,
                         BattleType.SingleBattle,
                         EnemyType.Wild,
                         new[] {wildRoster},
                         new[] {playerCharacterData},
                         true,
                         new[] {DefaultAI},
                         new Bag[] { },
                         true,
                         true,
                         true,
                         sceneInfo.BattleScenariosPerEncounter[encounterType],
                         weather,
                         encounterType,
                         endCallback);

        /// <summary>
        /// Launch a single wild encounter for the player.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="wildRoster">Roster of the wild monster.</param>
        /// <param name="sceneInfo">Information of the scene the encounter is triggered in.</param>
        /// <param name="weather">Starting weather.</param>
        /// <param name="encounterType">Type of encounter triggered.</param>
        /// <param name="endCallback">Callback for when the battle is over.</param>
        public void LaunchDoubleWildEncounter(PlayerCharacter playerCharacter,
                                              List<Roster> wildRoster,
                                              SceneInfo sceneInfo,
                                              Weather weather,
                                              EncounterType encounterType,
                                              Action<BattleResultParameters> endCallback) =>
            LaunchBattle(playerCharacter,
                         BattleType.DoubleBattle,
                         EnemyType.Wild,
                         wildRoster.ToArray(),
                         new[] {playerCharacterData},
                         true,
                         new[] {DefaultAI, DefaultAI},
                         new Bag[] { },
                         true,
                         true,
                         true,
                         sceneInfo.BattleScenariosPerEncounter[encounterType],
                         weather,
                         encounterType,
                         endCallback);

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
                                                 Action<BattleResultParameters> endCallback) =>
            LaunchBattle(playerCharacter,
                         BattleType.SingleBattle,
                         EnemyType.Trainer,
                         new[] {trainerRoster},
                         new[] {playerCharacterData, characterData},
                         true,
                         new[] {DefaultAI},
                         new[] {trainerBag},
                         true,
                         true,
                         true,
                         sceneInfo.BattleScenariosPerEncounter[encounterType],
                         weather,
                         encounterType,
                         endCallback,
                         new[] {afterBattleDialog},
                         respawnPlayerIfLose);

        /// <summary>
        /// Launch a battle.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="template">Object containing the battle parameters.</param>
        /// <param name="endCallback">Callback for when the battle is over.</param>
        [Button]
        [HideInEditorMode]
        public void LaunchBattle(PlayerCharacter playerCharacter,
                                 BattleTemplate template,
                                 Action<BattleResultParameters> endCallback) =>
            LaunchBattle(playerCharacter,
                         template.BattleType,
                         template.EnemyType,
                         template.Rosters,
                         template.Characters,
                         template.PlayerControlsFirstRoster,
                         template.AIs,
                         template.Bags,
                         template.IsFightAvailable,
                         template.IsMonstersMenuAvailable,
                         template.IsBagAvailable,
                         template.Scenario,
                         template.Weather,
                         template.EncounterType,
                         endCallback,
                         template.EnemyTrainersAfterBattleDialogs,
                         template.RespawnPlayerIfLose);

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
        /// <param name="endCallback">Callback for when the battle is over.</param>
        /// <param name="scenario">Scenario to use in this battle.</param>
        /// <param name="weather">Starting weather.</param>
        /// <param name="encounterType">Type of encounter this battle is.</param>
        /// <param name="enemyTrainersAfterBattleDialogKeys">Dialog to be said by the enemy trainers after tha battle.</param>
        /// <param name="respawnPlayerIfLose">Respawn the player if they lose?</param>
        public void LaunchBattle(PlayerCharacter playerCharacter,
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
                                 bool respawnPlayerIfLose = true)
        {
            playerCharacterReference = playerCharacter;

            if (!BattleInProgress)
                CoroutineRunner.RunRoutine(LaunchBattleRoutine(battleType,
                                                               enemyType,
                                                               rosters,
                                                               characters,
                                                               playerControlsFirstRoster,
                                                               ais,
                                                               bags,
                                                               isFightAvailable,
                                                               isMonstersMenuAvailable,
                                                               isBagAvailable,
                                                               scenario,
                                                               weather,
                                                               encounterType,
                                                               endCallback,
                                                               enemyTrainersAfterBattleDialogKeys,
                                                               playerCharacter,
                                                               respawnPlayerIfLose));
        }

        /// <summary>
        /// Retrieve the battle parameters for this battle.
        /// </summary>
        /// <returns></returns>
        public BattleParameters RegisterBattleManager(IBattleManager battleManager)
        {
            battleManagerReference = battleManager;

            battleManagerReference.SubscribeToBattleFinished(BattleFinished);

            battleManagerReference.SubscribeToBattleInitialized(OnBattleInitialized);

            return !BattleInProgress ? null : battleParameters;
        }

        /// <summary>
        /// Reset the battle launcher.
        /// </summary>
        public void Reset()
        {
            LastBattleResult = null;
            BattleInProgress = false;
            endCallbackReference = null;
            battleManagerReference = null;
        }

        /// <summary>
        /// Raised when a battle has been launched.
        /// </summary>
        public void SubscribeToBattleLaunched(Action subscriber) => OnBattleLaunched += subscriber;

        /// <summary>
        /// Raised when a battle has been launched.
        /// </summary>
        public void UnsubscribeFromBattleLaunched(Action subscriber) => OnBattleLaunched -= subscriber;

        /// <summary>
        /// Raised when a battle has ended.
        /// </summary>
        public void SubscribeToBattleToUnload(Action subscriber) => OnBattleToUnload += subscriber;

        /// <summary>
        /// Raised when a battle has ended.
        /// </summary>
        public void UnsubscribeFromBattleToUnload(Action subscriber) => OnBattleToUnload -= subscriber;

        /// <summary>
        /// Raised when a battle has ended.
        /// </summary>
        public void SubscribeToBattleEnded(Action<BattleResultParameters> subscriber) => OnBattleEnded += subscriber;

        /// <summary>
        /// Raised when a battle has ended.
        /// </summary>
        public void UnsubscribeFromBattleEnded(Action<BattleResultParameters> subscriber) =>
            OnBattleEnded -= subscriber;

        /// <summary>
        /// Routine to launch the battle.
        /// </summary>
        private IEnumerator LaunchBattleRoutine(BattleType battleType,
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
                                                string[] enemyTrainersAfterBattleDialogKeys,
                                                PlayerCharacter playerCharacter,
                                                bool respawnPlayerIfLose)
        {
            yield return WaitAFrame;

            TransitionManager.BlackScreenFadeIn();

            audioManager.StopAllAudios();

            audioManager.PlayAudio(enemyType == EnemyType.Wild
                                       ? WildBattleMusic.Random()
                                       : TrainerBattleMusic.Random(),
                                   true);

            BattleInProgress = true;

            yield return new WaitForSeconds(2);

            battleParameters = new BattleParameters
                               {
                                   BattleType = battleType,
                                   EnemyType = enemyType,
                                   Rosters = rosters,
                                   Characters = characters,
                                   PlayerControlsFirstRoster = playerControlsFirstRoster,
                                   AIs = ais,
                                   Bags = bags,
                                   IsFightAvailable = isFightAvailable,
                                   IsMonstersMenuAvailable = isMonstersMenuAvailable,
                                   IsBagAvailable = isBagAvailable,
                                   EnemyTrainersAfterBattleDialogKeys =
                                       enemyTrainersAfterBattleDialogKeys,
                                   Scenario = scenario,
                                   Weather = weather,
                                   EncounterType = encounterType,
                                   PlayerCharacter = playerCharacter,
                                   RespawnPlayerOnLoose = respawnPlayerIfLose
                               };

            endCallbackReference = endCallback;

            sceneManager.LoadScene(BattleScene, null, _ => OnBattleLaunched?.Invoke());
        }

        /// <summary>
        /// Called when the battle has been initialized.
        /// </summary>
        private static void OnBattleInitialized() => TransitionManager.BlackScreenFadeOut();

        /// <summary>
        /// Method called when the battle has finished.
        /// </summary>
        /// <param name="result">Result of the battle.</param>
        private void BattleFinished(BattleResultParameters result) =>
            CoroutineRunner.RunRoutine(BattleFinishedRoutine(result));

        /// <summary>
        /// Method called when the battle has finished.
        /// </summary>
        /// <param name="result">Result of the battle.</param>
        private IEnumerator BattleFinishedRoutine(BattleResultParameters result)
        {
            LastBattleResult = result;

            yield return TransitionManager.BlackScreenFadeInRoutine();

            OnBattleToUnload?.Invoke();

            if (result.PlayerRoster != null) playerRoster.LoadFromBattlers(result.PlayerRoster, settings);

            bool unloaded = false;

            sceneManager.UnloadScene(BattleScene,
                                     null,
                                     _ => unloaded = true);

            foreach (MonsterInstance monster in playerRoster)
            {
                if (monster == null || monster.IsNullEntry) continue;
                yield return monster.AfterBattle(localizer);
            }

            yield return evolutionManager.TriggerEvolutionsAfterLevelUp(playerRoster.RosterData.ToList(),
                                                                        playerCharacterReference,
                                                                        false);

            yield return evolutionManager.TriggerEvolutionAfterBattleThroughExtraData(playerRoster.RosterData.ToList(),
                false,
                playerCharacterReference);

            if (result.CapturedMonster is {IsNullEntry: false})
            {
                Logger.Info("Captured " + result.CapturedMonster.GetNameOrNickName(localizer));

                result.CapturedMonster.RebuildOriginData(playerCharacterReference.CharacterController.CurrentGrid
                                                            .SceneInfo.Asset,
                                                         OriginData.Type.Caught,
                                                         playerCharacterReference.CharacterController.GetCharacterData()
                                                            .LocalizableName,
                                                         settings,
                                                         localizer);

                result.CapturedMonster.CurrentTrainer = playerCharacterData.LocalizableName;

                yield return DialogManager.ShowNewMonsterDialog(playerCharacterReference,
                                                                result.CapturedMonster,
                                                                true,
                                                                true);
            }

            yield return new WaitUntil(() => unloaded);

            audioManager.StopAllAudios();

            bool shouldRespawn = !result.PlayerWon && result.RespawnIfLose;

            if (!shouldRespawn) yield return TransitionManager.BlackScreenFadeOutRoutine();

            BattleInProgress = false;

            endCallbackReference?.Invoke(result);
            OnBattleEnded?.Invoke(result);

            if (!shouldRespawn) yield break;
            yield return playerTeleporter.TeleportPlayer(globalGameData.LastHealLocation);
            playerRoster.CompletelyHeal();
        }
    }
}