using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Scenarios;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Bags;
using Varguiniano.YAPU.Runtime.World.Encounters;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Parameters to initialize a battle.
    /// </summary>
    public class BattleParameters
    {
        /// <summary>
        /// Type of battle.
        /// </summary>
        public BattleType BattleType;

        /// <summary>
        /// Type of enemy in the battle.
        /// </summary>
        public EnemyType EnemyType;

        /// <summary>
        /// Rosters to be used in this battle.
        /// </summary>
        public Roster[] Rosters;

        /// <summary>
        /// Characters that will fight in this battle.
        /// </summary>
        public CharacterData[] Characters;

        /// <summary>
        /// Flag to know if the player controls the first roster or it's controller by an AI.
        /// </summary>
        public bool PlayerControlsFirstRoster;

        /// <summary>
        /// AIs that will control each roster of the battle.
        /// </summary>
        public BattleAI[] AIs;

        /// <summary>
        /// Bags to be used in the battle.
        /// </summary>
        public Bag[] Bags;

        /// <summary>
        /// Is the fight menu available?
        /// </summary>
        public bool IsFightAvailable;

        /// <summary>
        /// Is the monsters menu available?
        /// </summary>
        public bool IsMonstersMenuAvailable;

        /// <summary>
        /// Is the bag available to the trainers?
        /// </summary>
        public bool IsBagAvailable;

        /// <summary>
        /// Dialog to be said by the enemy trainer after tha battle.
        /// </summary>
        public string[] EnemyTrainersAfterBattleDialogKeys;

        /// <summary>
        /// Scenario to use in this battle.
        /// </summary>
        public BattleScenario Scenario;

        /// <summary>
        /// Starting weather.
        /// </summary>
        public Weather Weather;

        /// <summary>
        /// Type of encounter this battle is.
        /// </summary>
        public EncounterType EncounterType;

        /// <summary>
        /// Reference to the player character.
        /// </summary>
        public PlayerCharacter PlayerCharacter;

        /// <summary>
        /// Respawn the player if lost?
        /// </summary>
        public bool RespawnPlayerOnLoose;
    }
}