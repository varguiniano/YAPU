using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnStatCalculation
{
    /// <summary>
    /// Data class for an item effect that multiplies a specific stat when it is calculated if the monster can still evolve.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/OnStatCalculation/MultiplyStatOnCalculationIfMonsterCanEvolve",
                     fileName = "MultiplyStatOnCalculationIfMonsterCanEvolve")]
    public class MultiplyStatOnCalculationIfMonsterCanEvolve : MultiplyStatOnCalculation
    {
        /// <summary>
        /// Check if the condition for modifying the stat applies.
        /// </summary>
        /// <param name="item">Item with this effect.</param>
        /// <param name="monster">Monster holding the item.</param>
        /// <param name="stat">Stat being calculated.</param>
        /// <returns>True if the stat should be modified.</returns>
        protected override bool CheckCondition(Item item, MonsterInstance monster, Stat stat) =>
            base.CheckCondition(item, monster, stat) && monster.FormData.Evolutions.Count > 0;
    }
}