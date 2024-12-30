using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the MarvelScale ability.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/MarvelScale", fileName = "MarvelScale")]
    public class MarvelScale : Ability
    {
        /// <summary>
        /// Increase defense when having a status.
        /// </summary>
        public override float OnCalculateStat(MonsterInstance monster, Stat stat, BattleManager battleManager) =>
            (stat == Stat.Defense && monster.GetStatus() != null ? 1.5f : 1)
          * base.OnCalculateStat(monster, stat, battleManager);
    }
}