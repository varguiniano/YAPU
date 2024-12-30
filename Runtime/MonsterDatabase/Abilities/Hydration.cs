using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the Hydration ability.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Hydration", fileName = "Hydration")]
    public class Hydration : Ability
    {
        /// <summary>
        /// Weathers compatible with this ability.
        /// </summary>
        [SerializeField]
        private List<Weather> CompatibleWeathers;

        /// <summary>
        /// Heal status if the weather is compatible.
        /// </summary>
        public override IEnumerator AfterTurnPreStatus(Battler battler,
                                                       BattleManager battleManager,
                                                       ILocalizer localizer)
        {
            if (battler.GetStatus() == null
             || !battleManager.Scenario.GetWeather(out Weather weather)
             || !CompatibleWeathers.Contains(weather))
                yield break;

            ShowAbilityNotification(battler);

            yield return battleManager.Statuses.RemoveStatus(battler);
        }
    }
}