using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability FurCoat.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/FurCoat", fileName = "FurCoat")]
    public class FurCoat : Ability
    {
        /// <summary>
        /// Stat to multiply.
        /// </summary>
        [SerializeField]
        private Stat StatToMultiply;

        /// <summary>
        /// Multiplier for the stat.
        /// </summary>
        [SerializeField]
        private float Multiplier;

        /// <summary>
        /// Multiply the stat.
        /// </summary>
        public override float OnCalculateStat(MonsterInstance monster, Stat stat, BattleManager battleManager) =>
            base.OnCalculateStat(monster, stat, battleManager) * (stat == StatToMultiply ? Multiplier : 1);
    }
}