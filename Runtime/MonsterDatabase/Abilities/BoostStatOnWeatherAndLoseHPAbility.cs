using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Base class for an ability that boosts a stat of the monster on certain weathers but makes it lose HP.
    /// </summary>
    public abstract class BoostStatOnWeatherAndLoseHPAbility : BoostStatOnWeatherAbility
    {
        /// <summary>
        /// HP reduction of the ability.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        [PropertyRange(0, 1)]
        private float HPReduction = 1f / 8f;

        /// <summary>
        /// Called once after each turn.
        /// </summary>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Localizer reference.</param>
        public override IEnumerator AfterTurnPostStatus(Battler battler,
                                                        BattleManager battleManager,
                                                        ILocalizer localizer)
        {
            if (!battleManager.Scenario.GetWeather(out Weather weather)
             || !MultiplierPerWeather.ContainsKey(weather))
                yield break;

            ShowAbilityNotification(battler);

            (BattlerType type, int index) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            yield return DialogManager.ShowDialogAndWait("Abilities/BoostStatOnWeatherAndLoseHPAbility/Hit",
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        battler.GetNameOrNickName(localizer),
                                                                        localizer[LocalizableName]
                                                                    },
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            yield return battleManager.BattlerHealth.ChangeLife(battler,
                                                                type,
                                                                index,
                                                                -(int) (HPReduction
                                                                      * battler.GetStats(battleManager)[Stat.Hp]),
                                                                isSecondaryDamage: true);
        }
    }
}