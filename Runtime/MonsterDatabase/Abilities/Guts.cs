using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the Guts ability.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Guts", fileName = "Guts")]
    public class Guts : Ability
    {
        /// <summary>
        /// Increase attack when having a status.
        /// </summary>
        public override float OnCalculateStat(MonsterInstance monster, Stat stat, BattleManager battleManager) =>
            (stat == Stat.Attack && monster.GetStatus() != null ? 1.5f : 1)
          * base.OnCalculateStat(monster, stat, battleManager);

        /// <summary>
        /// Not affected by burn damage reduction.
        /// </summary>
        public override bool IsAffectedByBurnDamageReduction(Battler battler, BattleManager battleManager) => false;
    }
}