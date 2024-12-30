using UnityEngine;
using UnityEngine.Serialization;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Scenarios;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Bags;
using Varguiniano.YAPU.Runtime.World.Encounters;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Template with parameters for a battle.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Battle/Template", fileName = "BattleTemplate")]
    public class BattleTemplate : WhateverScriptable<BattleTemplate>
    {
        /// <summary>
        /// Battle type.
        /// </summary>
        public BattleType BattleType;

        /// <summary>
        /// Enemy type.
        /// </summary>
        public EnemyType EnemyType;

        /// <summary>
        /// Rosters for the battle.
        /// </summary>
        public Roster[] Rosters;

        /// <summary>
        /// Characters for the battle.
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
        /// Bags to use in this battle.
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
        public string[] EnemyTrainersAfterBattleDialogs;

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
        /// Respawn the player if they lose?
        /// </summary>
        public bool RespawnPlayerIfLose;
    }
}