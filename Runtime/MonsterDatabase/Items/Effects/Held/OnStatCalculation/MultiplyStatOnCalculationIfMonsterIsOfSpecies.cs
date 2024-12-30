using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnStatCalculation
{
    /// <summary>
    /// Data class for an item effect that multiplies a specific stat when it is calculated if the monster is of a specific species.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/OnStatCalculation/MultiplyStatOnCalculationIfMonsterIsOfSpecies",
                     fileName = "MultiplyStatOnCalculationIfMonsterIsOfSpecies")]
    public class MultiplyStatOnCalculationIfMonsterIsOfSpecies : MultiplyStatOnCalculation
    {
        /// <summary>
        /// Species to check.
        /// </summary>
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsters))]
        #endif
        private MonsterEntry Species;

        /// <summary>
        /// Check if the condition for modifying the stat applies.
        /// </summary>
        /// <param name="item">Item with this effect.</param>
        /// <param name="monster">Monster holding the item.</param>
        /// <param name="stat">Stat being calculated.</param>
        /// <returns>True if the stat should be modified.</returns>
        protected override bool CheckCondition(Item item, MonsterInstance monster, Stat stat) =>
            base.CheckCondition(item, monster, stat) && monster.Species == Species;
    }
}