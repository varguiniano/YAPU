using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnStatCalculation
{
    /// <summary>
    /// Data class for an item effect that multiplies a specific stat when it is calculated.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/OnStatCalculation/MultiplyStatOnCalculation",
                     fileName = "MultiplyStatOnCalculation")]
    public class MultiplyStatOnCalculation : OnStatCalculationItemEffect
    {
        /// <summary>
        /// Stat to affect.
        /// </summary>
        [SerializeField]
        private Stat Stat;

        /// <summary>
        /// Multiplier to apply.
        /// </summary>
        [SerializeField]
        private float Multiplier = 1;

        /// <summary>
        /// Called when a stat is about to be calculated.
        /// </summary>
        /// <param name="item">Reference to the held item.</param>
        /// <param name="monster">Reference to that monster.</param>
        /// <param name="stat">Stat to be calculated.</param>
        public override float OnCalculateStat(Item item,
                                              MonsterInstance monster,
                                              Stat stat) =>
            CheckCondition(item, monster, stat) ? Multiplier : 1;

        /// <summary>
        /// Check if the condition for modifying the stat applies.
        /// </summary>
        /// <param name="item">Item with this effect.</param>
        /// <param name="monster">Monster holding the item.</param>
        /// <param name="stat">Stat being calculated.</param>
        /// <returns>True if the stat should be modified.</returns>
        protected virtual bool CheckCondition(Item item,
                                              MonsterInstance monster,
                                              Stat stat) =>
            Stat == stat;
    }
}