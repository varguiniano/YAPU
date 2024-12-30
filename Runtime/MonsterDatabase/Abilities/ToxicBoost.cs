using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ToxicBoost ability.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/ToxicBoost", fileName = "ToxicBoost")]
    public class ToxicBoost : Ability
    {
        /// <summary>
        /// Statuses compatible with this ability.
        /// </summary>
        [SerializeField]
        private List<Status> CompatibleStatuses;

        /// <summary>
        /// Increase attack when having a status.
        /// </summary>
        public override float OnCalculateStat(MonsterInstance monster, Stat stat, BattleManager battleManager) =>
            (stat == Stat.Attack && CompatibleStatuses.Contains(monster.GetStatus()) ? 1.5f : 1)
          * base.OnCalculateStat(monster, stat, battleManager);
    }
}