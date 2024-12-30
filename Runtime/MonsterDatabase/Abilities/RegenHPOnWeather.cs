using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Base class for abilities that regen the HP at the end of the turn on certain weathers.
    /// </summary>
    public abstract class RegenHPOnWeather : Ability
    {
        /// <summary>
        /// Weathers and the HP to regen.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        protected SerializableDictionary<Weather, float> PercentagePerWeather;

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
             || !PercentagePerWeather.ContainsKey(weather)
             || !battler.CanHeal(battleManager))
                yield break;

            (BattlerType type, int index) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            int hpToRegen = (int) (PercentagePerWeather[weather] * battler.GetStats(battleManager)[Stat.Hp]);

            ShowAbilityNotification(battler);

            battleManager.GetMonsterSprite(type, index).FXAnimator.PlayBoost(battleManager.BattleSpeed);

            yield return battleManager.BattlerHealth.ChangeLife(battler,
                                                                type,
                                                                index,
                                                                hpToRegen);

            yield return DialogManager.ShowDialogAndWait("Abilities/RegenHPOnWeather/Regen",
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        battler.GetNameOrNickName(localizer),
                                                                        hpToRegen.ToString(),
                                                                        localizer[LocalizableName]
                                                                    },
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);
        }
    }
}