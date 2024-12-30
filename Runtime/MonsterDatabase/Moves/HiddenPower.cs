using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move HiddenPower.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/HiddenPower", fileName = "HiddenPower")]
    public class HiddenPower : DamageMove
    {
        /// <summary>
        /// Type the move will be depending on the result of the IV equation.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializableDictionary<int, MonsterType> TypeTable;

        /// <summary>
        /// Get the type of this move out of battle.
        /// https://bulbapedia.bulbagarden.net/wiki/Hidden_Power_(move)/Calculation#Type_2
        /// </summary>
        /// <param name="monster">Owner of the move.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <returns>The type of the move.</returns>
        public override MonsterType GetMoveType(MonsterInstance monster, YAPUSettings settings)
        {
            if (monster == null) return base.GetMoveType(null, settings);
            
            SerializableDictionary<Stat, byte> ivs = monster.StatData.IndividualValues;

            int value = Mathf.FloorToInt((ivs[Stat.Hp] % 2
                                        + 2 * (ivs[Stat.Attack] % 2)
                                        + 4 * (ivs[Stat.Defense] % 2)
                                        + 8 * (ivs[Stat.Speed] % 2)
                                        + 16 * (ivs[Stat.SpecialAttack] % 2)
                                        + 32 * (ivs[Stat.SpecialDefense] % 2))
                                       * 15f
                                       / 63);

            return TypeTable[value];
        }
    }
}