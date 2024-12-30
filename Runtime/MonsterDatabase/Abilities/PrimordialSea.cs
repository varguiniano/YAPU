using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Ability that sets up a weather when entering the battle.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/PrimordialSea", fileName = "PrimordialSea")]
    public class PrimordialSea : SetupWeatherWhenEnteringTheBattle
    {
        /// <summary>
        /// Remove the weather upon leaving the battle.
        /// </summary>
        public override IEnumerator OnMonsterLeavingBattle(BattleManager battleManager, Battler battler)
        {
            yield return base.OnMonsterLeavingBattle(battleManager, battler);

            if (battleManager.Scenario.GetWeather(out Weather weather) && weather == WeatherToSet)
                yield return battleManager.Scenario.SetWeather(null, 0);
        }
    }
}