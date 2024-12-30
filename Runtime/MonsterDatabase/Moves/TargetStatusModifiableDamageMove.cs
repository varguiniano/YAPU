using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for a damage move which power is modified if the target has some status.
    /// </summary>
    public abstract class TargetStatusModifiableDamageMove : DamageMove
    {
        /// <summary>
        /// Modifiers to apply when the target has a status.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializableDictionary<Status, float> TargetStatusPowerModifiers;

        /// <summary>
        /// Get the move's power.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="hitNumber"></param>
        /// <returns>The move's power.</returns>
        public override int GetMovePowerInBattle(BattleManager battleManager,
                                                 Battler user,
                                                 Battler target,
                                                 bool ignoresAbilities,
                                                 int hitNumber = 0)
        {
            float multiplier = 1;

            if (target is { IsNullEntry: false } && TargetStatusPowerModifiers.ContainsKey(target.GetStatus()))
                multiplier = TargetStatusPowerModifiers[target.GetStatus()];

            return (int)(multiplier * base.GetMovePowerInBattle(battleManager, user, target, ignoresAbilities, hitNumber));
        }
    }
}