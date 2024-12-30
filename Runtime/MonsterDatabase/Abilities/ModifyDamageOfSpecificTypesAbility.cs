using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for an ability that modifies the damage of specific types when hitting the owner of the ability.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/ModifyDamageOfSpecificTypesAbility",
                     fileName = "ModifyDamageOfSpecificTypesAbility")]
    public class ModifyDamageOfSpecificTypesAbility : Ability
    {
        /// <summary>
        /// Types and the multiplier to apply to them.
        /// </summary>
        [SerializeField]
        private SerializableDictionary<MonsterType, float> Multipliers;

        /// <summary>
        /// Called when calculating a move's damage on itself.
        /// Multiply the damage by the multiplier if the move's type is in the dictionary.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="multiplier">Current move multiplier.</param>
        /// <param name="user">Battler using the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="finished">Callback stating the new multiplier.</param>
        public override IEnumerator OnCalculateMoveDamageWhenTargeted(DamageMove move,
                                                                      float multiplier,
                                                                      Battler user,
                                                                      Battler target,
                                                                      BattleManager battleManager,
                                                                      Action<float> finished)
        {
            if (!AffectsUserOfEffect(user, target, IgnoresOtherAbilities(battleManager, target, null), battleManager))
            {
                finished.Invoke(multiplier);
                yield break;
            }

            MonsterType moveType = move.GetMoveTypeInBattle(user, battleManager);

            if (Multipliers.TryGetValue(moveType, out float typeMultiplier))
            {
                ShowAbilityNotification(user);
                multiplier *= typeMultiplier;
            }

            finished.Invoke(multiplier);
        }
    }
}