using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// HugePower ability implementation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/HugePower", fileName = "HugePower")]
    public class HugePower : Ability
    {
        /// <summary>
        /// Multipliers for each stat.
        /// </summary>
        [SerializeField]
        private SerializableDictionary<Stat, float> Multipliers;

        /// <summary>
        /// Multiply the stat.
        /// </summary>
        public override float OnCalculateStat(MonsterInstance monster, Stat stat, BattleManager battleManager)
        {
            float multiplier = base.OnCalculateStat(monster, stat, battleManager);

            foreach (KeyValuePair<Stat, float> pair in Multipliers)
                if (pair.Key == stat)
                    multiplier *= pair.Value;

            return multiplier;
        }
    }
}