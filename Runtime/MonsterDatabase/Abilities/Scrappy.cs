using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Scrappy.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Scrappy", fileName = "Scrappy")]
    public class Scrappy : Ability
    {
        /// <summary>
        /// Target types that modify the effectiveness when hit by the value types.
        /// </summary>
        [SerializeField]
        private SerializableDictionary<MonsterType, List<MonsterType>> AffectedTypes;

        /// <summary>
        /// Allow the user of a move to modify the effectiveness of a move when attacking.
        /// </summary>
        public override void ModifyMultiplierOfTypesWhenAttacking(Battler owner,
                                                                  Battler target,
                                                                  Move move,
                                                                  BattleManager battleManager,
                                                                  ref float multiplier)
        {
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (KeyValuePair<MonsterType, List<MonsterType>> pair in AffectedTypes)
            {
                if (!target.IsOfType(pair.Key, battleManager.YAPUSettings)) continue;

                if (!pair.Value.Contains(move.GetMoveTypeInBattle(owner, battleManager)) || !(multiplier < 1f))
                    continue;

                multiplier = 1f;
                ShowAbilityNotification(owner, true);
            }
        }
    }
}