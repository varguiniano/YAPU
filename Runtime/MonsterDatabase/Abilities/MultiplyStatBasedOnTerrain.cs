using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Core.Runtime.DataStructures;
using Terrain = Varguiniano.YAPU.Runtime.Battle.Statuses.Terrain.Terrain;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Base class for abilities that multiply a stat based on the terrain.
    /// </summary>
    public abstract class MultiplyStatBasedOnTerrain : Ability
    {
        /// <summary>
        /// Multiplier to apply to stats by terrain.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializableDictionary<Terrain, SerializableDictionary<Stat, float>> Multipliers;

        /// <summary>
        /// Called when calculating a stat of the monster that has this ability.
        /// </summary>
        /// <param name="monster">Monster that has the ability.</param>
        /// <param name="stat">Stat to calculate.</param>
        /// <param name="battleManager">Reference to the battle manager if we are in battle.</param>
        /// <returns>A multiplier to apply to that stat.</returns>
        public override float OnCalculateStat(MonsterInstance monster, Stat stat, BattleManager battleManager)
        {
            if (battleManager == null
             || !Multipliers.ContainsKey(battleManager.Scenario.Terrain)
             || !Multipliers[battleManager.Scenario.Terrain].ContainsKey(stat))
                return base.OnCalculateStat(monster, stat, battleManager);

            return Multipliers[battleManager.Scenario.Terrain][stat];
        }
    }
}