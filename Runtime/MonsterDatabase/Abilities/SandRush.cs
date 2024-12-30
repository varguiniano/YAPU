using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the SandRush ability.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/SandRush", fileName = "SandRush")]
    public class SandRush : Ability
    {
        /// <summary>
        /// Weather compatible with this ability.
        /// </summary>
        [SerializeField]
        private List<Weather> CompatibleWeathers;

        /// <summary>
        /// Called when calculating a stat of the monster that has this ability.
        /// Double speed in the correct weather.
        /// </summary>
        /// <param name="monster">Monster that has the ability.</param>
        /// <param name="stat">Stat to calculate.</param>
        /// <param name="battleManager">Reference to the battle manager if we are in battle.</param>
        /// <returns>A multiplier to apply to that stat.</returns>
        public override float OnCalculateStat(MonsterInstance monster, Stat stat, BattleManager battleManager) =>
            (stat == Stat.Speed
          && battleManager != null
          && battleManager.Scenario.GetWeather(out Weather weather)
          && CompatibleWeathers.Contains(weather)
                 ? 2
                 : 1)
          * base.OnCalculateStat(monster, stat, battleManager);
    }
}