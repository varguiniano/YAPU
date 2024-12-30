using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Plus ability implementation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Plus", fileName = "Plus")]
    public class Plus : Ability
    {
        /// <summary>
        /// Abilities that interact with this one.
        /// </summary>
        [SerializeField]
        private List<Ability> InteractingAbilities;

        /// <summary>
        /// Multipliers to apply to the stat.
        /// </summary>
        [SerializeField]
        private SerializableDictionary<Stat, float> StatMultipliers;

        /// <summary>
        /// Multiply stat when an ally has Minus.
        /// </summary>
        public override float OnCalculateStat(MonsterInstance monster, Stat stat, BattleManager battleManager)
        {
            float multiplier = base.OnCalculateStat(monster, stat, battleManager);

            if (battleManager == null || monster is not Battler battler) return multiplier;

            (BattlerType monsterType, int _) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            if (battleManager.Battlers.GetBattlersFighting(monsterType)
                             .Any(candidate =>
                                      candidate != battler
                                   && candidate.CanUseAbility(battleManager, false)
                                   && InteractingAbilities.Contains(candidate.GetAbility())))
                multiplier *= StatMultipliers[stat];

            return multiplier;
        }
    }
}