using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Ability that sets up a weather when entering the battle.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/SetupWeatherWhenEnteringTheBattle",
                     fileName = "SetupWeatherWhenEnteringTheBattle")]
    public class SetupWeatherWhenEnteringTheBattle : Ability
    {
        /// <summary>
        /// Reference to the weather the ability will set.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        protected Weather WeatherToSet;

        /// <summary>
        /// Countdown for the weather to last.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private int Countdown;

        /// <summary>
        /// Reference to the weathers that are incompatible if set and will make the ability fail.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private List<Weather> IncompatibleWeathers;

        /// <summary>
        /// Called when the monster is sent into battle.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Monster that entered the battle.</param>
        public override IEnumerator OnMonsterEnteredBattle(BattleManager battleManager, Battler battler)
        {
            yield return base.OnMonsterEnteredBattle(battleManager, battler);

            battleManager.Scenario.GetWeather(out Weather weather);

            // Always check even if the weather doesn't currently have an effect.
            if (IncompatibleWeathers.Contains(weather)) yield break;

            int customCountdown = battler.CanUseHeldItemInBattle(battleManager)
                                      ? battler.HeldItem.CalculateWeatherDuration(WeatherToSet, battler, battleManager)
                                      : -2;

            ShowAbilityNotification(battler);

            yield return
                battleManager.Scenario.SetWeather(WeatherToSet,
                                                  customCountdown != -2
                                                      ? customCountdown
                                                      : Countdown);
        }
    }
}