using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Scenarios;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Battles
{
    /// <summary>
    /// Configuration for the RandomFightAgainstRandomRosters command.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Battle/RandomFightAgainstRandomRostersConfig", fileName = "RandomFightAgainstRandomRostersConfig")]
    public class RandomFightAgainstRandomRostersConfig : WhateverScriptable<RandomFightAgainstRandomRostersConfig>
    {
        /// <summary>
        /// Template for a single battle.
        /// </summary>
        [SerializeField]
        public BattleTemplate SingleBattleTemplate;

        /// <summary>
        /// Template for a double battle with the player against 2.
        /// </summary>
        [SerializeField]
        public BattleTemplate OneVSTwoDoubleBattleTemplate;

        /// <summary>
        /// Template for a double battle with the player and another against 2.
        /// </summary>
        [SerializeField]
        public BattleTemplate TwoVSTwoDoubleBattleTemplate;

        /// <summary>
        /// Chances of it being a single battle.
        /// </summary>
        [SerializeField]
        [PropertyRange(0, 1)]
        public float SingleBattleChance = .75f;

        /// <summary>
        /// Max different items a bag can have.
        /// </summary>
        [SerializeField]
        public int MaxTypesOfItemsPerBag = 10;

        /// <summary>
        /// Max number of a single item a bag can have.
        /// </summary>
        [SerializeField]
        public int MaxNumberOfSingleItem = 5;

        /// <summary>
        /// Possible scenarios for the battle.
        /// </summary>
        [SerializeField]
        public List<BattleScenario> PossibleScenarios;

        /// <summary>
        /// Possible weathers for the battle.
        /// </summary>
        [SerializeField]
        public List<Weather> PossibleWeathers;

        /// <summary>
        /// Custom pool to use when generating the random rosters.
        /// </summary>
        [SerializeField]
        public List<MonsterEntry> CustomMonsterPool;
    }
}