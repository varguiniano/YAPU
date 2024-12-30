using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Side
{
    /// <summary>
    /// Base class for side statuses that modify stats.
    /// </summary>
    public abstract class StatModifierSideStatus : SideStatus
    {
        /// <summary>
        /// Dictionary with the stats to modify and the multipliers to apply.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializableDictionary<Stat, float> MultiplierPerStat;
        
        /// <summary>
        /// Called when a stat is about to be calculated.
        /// </summary>
        /// <param name="monster">Reference to that monster.</param>
        /// <param name="stat">Stat to be calculated.</param>
        /// <returns>The multiplier to apply to that stat.</returns>
        public override float OnCalculateStat(MonsterInstance monster, Stat stat)
        {
            if (MultiplierPerStat.TryGetValue(stat, out float calculateStat)) return calculateStat;

            return 1;
        }
    }
}