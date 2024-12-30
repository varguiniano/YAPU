using Proxima;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.Quests;
using Varguiniano.YAPU.Runtime.World;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Characters
{
    /// <summary>
    /// Class that allows a console to run commands related to the player character.
    /// </summary>
    public class PlayerCharacterCommands : WhateverBehaviour<PlayerCharacterCommands>, IPlayerDataReceiver
    {
        /// <summary>
        /// Current instance of this object,
        /// we need to check and save this since the player will not always be on scene.
        /// </summary>
        private static PlayerCharacterCommands instance;

        /// <summary>
        /// Flag to check if the commands have been registered to Proxima.
        /// </summary>
        private static bool registeredCommands;

        /// <summary>
        /// Reference to the world database.
        /// </summary>
        [Inject]
        private WorldDatabase worldDatabase;

        /// <summary>
        /// Reference to the quest manager.
        /// </summary>
        [Inject]
        private QuestManager questManager;

        /// <summary>
        /// Register commands if they are not and assign the instance.
        /// </summary>
        private void Awake()
        {
            instance = this;

            if (registeredCommands) return;

            ProximaInspector.RegisterCommands<PlayerCharacterCommands>();
            registeredCommands = true;
        }

        /// <summary>
        /// Move the player in a direction a certain amount of tiles.
        /// </summary>
        /// <param name="direction">Direction to move towards.</param>
        /// <param name="tiles">Number of tiles to move.</param>
        [ProximaCommand("Player",
                        description: "Make the player walk in a direction a certain amount of tiles."
                                   + "Directions:"
                                   + "0 - Down"
                                   + "1 - Up"
                                   + "2 - Left"
                                   + "3 - Right")]
        public static void Walk(int direction, int tiles) => MovePlayer(direction, tiles, false);

        /// <summary>
        /// Move the player in a direction a certain amount of tiles.
        /// </summary>
        /// <param name="direction">Direction to move towards.</param>
        /// <param name="tiles">Number of tiles to move.</param>
        [ProximaCommand("Player",
                        description: "Make the player run in a direction a certain amount of tiles."
                                   + "Directions:"
                                   + "0 - Down"
                                   + "1 - Up"
                                   + "2 - Left"
                                   + "3 - Right")]
        public static void Run(int direction, int tiles) => MovePlayer(direction, tiles, true);

        /// <summary>
        /// Make the player enter the water in the given direction.
        /// </summary>
        /// <param name="direction">Direction to move towards.</param>
        [ProximaCommand("Player",
                        description: "Make the player enter the water in the given direction."
                                   + "Directions:"
                                   + "0 - Down"
                                   + "1 - Up"
                                   + "2 - Left"
                                   + "3 - Right")]
        public static void EnterWater(int direction)
        {
            if (!TryGetPlayerCharacter(out PlayerCharacter playerCharacter)) return;

            CoroutineRunner.RunRoutine(playerCharacter.CharacterController.EnterWater(IntToDirection(direction)));
        }

        /// <summary>
        /// Make the player climb a waterfall.
        /// </summary>
        [ProximaCommand("Player", description: "Make the player climb a waterfall.")]
        public static void ClimbWaterfall()
        {
            if (!TryGetPlayerCharacter(out PlayerCharacter playerCharacter)) return;

            CoroutineRunner.RunRoutine(playerCharacter.CharacterController.ClimbWaterfall());
        }

        /// <summary>
        /// Make the player fish.
        /// </summary>
        /// <param name="level">Fishing level. Encounters depend on this.</param>
        [ProximaCommand("Player", description: "Make the player fish.")]
        public static void Fish(int level)
        {
            if (!TryGetPlayerCharacter(out PlayerCharacter playerCharacter)) return;

            CoroutineRunner.RunRoutine(playerCharacter.Fish(level));
        }

        /// <summary>
        /// Make the player use a flashlight.
        /// </summary>
        [ProximaCommand("Player", description: "Make the player use a flashlight.")]
        public static void Flash()
        {
            if (!TryGetPlayerCharacter(out PlayerCharacter playerCharacter)) return;

            CoroutineRunner.RunRoutine(playerCharacter.UseFlash());
        }

        /// <summary>
        /// Make the player escape the dungeon.
        /// </summary>
        [ProximaCommand("Player", description: "Make the player escape the dungeon.")]
        public static void EscapeDungeon()
        {
            if (!TryGetPlayerCharacter(out PlayerCharacter playerCharacter)) return;

            CoroutineRunner.RunRoutine(playerCharacter.EscapeDungeon());
        }

        /// <summary>
        /// Trigger the egg hatching after an egg cycle.
        /// </summary>
        [ProximaCommand("Player", description: "Trigger the egg hatching after an egg cycle.")]
        public static void TriggerHatchingAfterEggCycle()
        {
            if (!TryGetPlayerCharacter(out PlayerCharacter playerCharacter)) return;

            CoroutineRunner.RunRoutine(playerCharacter.HatchingManager.TriggerHatchingAfterCycle(playerCharacter
                                          .PlayerRoster.RosterData));
        }

        /// <summary>
        /// Set the value of a bool variable in the scene.
        /// </summary>
        /// <param name="variableName">Variable to set.</param>
        /// <param name="value">Value to set.</param>
        [ProximaCommand("Player", description: "Set the value of a bool variable in the scene.")]
        public static void SetLocalBoolVariable(string variableName, bool value)
        {
            if (!TryGetPlayerCharacter(out PlayerCharacter playerCharacter)) return;

            playerCharacter.Scene.SetVariable(variableName, value);
        }

        /// <summary>
        /// Make the player start a quest.
        /// </summary>
        /// <param name="questName">Quest to start.</param>
        /// <param name="startingObjective">Objective for the quest to start with. Can be -1 for no current objective.</param>
        [ProximaCommand("Quests", description: "Make the player start a quest.")]
        public static void StartQuest(string questName, int startingObjective)
        {
            if (!TryGetPlayerCharacter(out PlayerCharacter _)) return;

            instance.questManager.StartQuest(instance.worldDatabase.GetQuestByName(questName),
                                             startingObjective);
        }

        /// <summary>
        /// Set a new objective for a quest.
        /// </summary>
        /// <param name="questName">Quest to update the objective from.</param>
        /// <param name="objective">New objective to set.</param>
        [ProximaCommand("Quests", description: "Set a new objective for a quest.")]
        public static void SetQuestObjective(string questName, int objective)
        {
            if (!TryGetPlayerCharacter(out PlayerCharacter _)) return;

            instance.questManager.SetQuestObjective(instance.worldDatabase.GetQuestByName(questName),
                                                    objective);
        }

        /// <summary>
        /// Complete the given quest.
        /// </summary>
        /// <param name="questName">Quest to complete.</param>
        [ProximaCommand("Quests", description: "Complete the given quest.")]
        public static void CompleteQuest(string questName)
        {
            if (!TryGetPlayerCharacter(out PlayerCharacter _)) return;

            instance.questManager.CompleteQuest(instance.worldDatabase.GetQuestByName(questName));
        }

        /// <summary>
        /// Remove a quest from the dictionary, making it as it was never assigned to the player.
        /// </summary>
        /// <param name="questName">Quest to remove.</param>
        [ProximaCommand("Quests",
                        description:
                        "Remove a quest from the dictionary, making it as it was never assigned to the player.")]
        public static void ResetQuest(string questName)
        {
            if (!TryGetPlayerCharacter(out PlayerCharacter _)) return;

            instance.questManager.ResetQuest(instance.worldDatabase.GetQuestByName(questName));
        }

        /// <summary>
        /// Move the player in a direction a certain amount of tiles.
        /// </summary>
        /// <param name="direction">Direction to move towards.</param>
        /// <param name="tiles">Number of tiles to move.</param>
        /// <param name="run">Should the player run?</param>
        private static void MovePlayer(int direction, int tiles, bool run)
        {
            if (!TryGetPlayerCharacter(out PlayerCharacter playerCharacter)) return;

            playerCharacter.CharacterController.IsRunning = run;

            CoroutineRunner.RunRoutine(playerCharacter.CharacterController.Move(IntToDirection(direction), tiles));
        }

        /// <summary>
        /// Translate an int direction to the Direction enum.
        /// </summary>
        /// <param name="intDirection">Direction in int value.</param>
        /// <returns>The direction in enum value.</returns>
        private static CharacterController.Direction IntToDirection(int intDirection) =>
            intDirection switch
            {
                0 => CharacterController.Direction.Down,
                1 => CharacterController.Direction.Up,
                2 => CharacterController.Direction.Left,
                3 => CharacterController.Direction.Right,
                _ => CharacterController.Direction.None
            };

        /// <summary>
        /// Attempt to get a reference to the player character.
        /// This won't work when the player is not instantiated.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <returns>True if the player was found.</returns>
        private static bool TryGetPlayerCharacter(out PlayerCharacter playerCharacter)
        {
            playerCharacter = null;

            if (instance == null)
            {
                StaticLogger.Error("Player is not currently instantiated.");
                return false;
            }

            if (instance.TryGetCachedComponent(out playerCharacter)) return true;

            StaticLogger.Error("Couldn't find player character component.");
            return false;
        }
    }
}