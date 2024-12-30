using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Forecast.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Forecast", fileName = "Forecast")]
    public class Forecast : Ability
    {
        /// <summary>
        /// Forms for each weather.
        /// </summary>
        [SerializeField]
        private SerializableDictionary<Weather, Form> WeatherFormCombos;

        /// <summary>
        /// Form to go back to by default.
        /// </summary>
        [SerializeField]
        private Form DefaultForm;

        /// <summary>
        /// Switch to the appropriate form depending on the weather.
        /// </summary>
        public override IEnumerator OnMonsterEnteredBattle(BattleManager battleManager, Battler battler)
        {
            yield return base.OnMonsterEnteredBattle(battleManager, battler);

            yield return TriggerAbility(battler, battleManager);
        }

        /// <summary>
        /// Switch to the appropriate form depending on the weather.
        /// </summary>
        public override IEnumerator AfterAction(Battler owner,
                                                BattleAction action,
                                                Battler user,
                                                BattleManager battleManager)
        {
            yield return base.AfterAction(owner, action, user, battleManager);

            yield return TriggerAbility(owner, battleManager);
        }

        /// <summary>
        /// Switch to the appropriate form depending on the weather.
        /// </summary>
        public override IEnumerator AfterTurnPostStatus(Battler battler,
                                                        BattleManager battleManager,
                                                        ILocalizer localizer)
        {
            yield return base.AfterTurnPostStatus(battler, battleManager, localizer);

            yield return TriggerAbility(battler, battleManager);
        }

        /// <summary>
        /// Switch to the appropriate form depending on the weather.
        /// </summary>
        private IEnumerator TriggerAbility(Battler owner,
                                           BattleManager battleManager)
        {
            Form targetForm = owner.Form.IsShiny && DefaultForm.HasShinyVersion
                                  ? DefaultForm.ShinyVersion
                                  : DefaultForm;

            if (battleManager.Scenario.GetWeather(out Weather weather)
             && WeatherFormCombos.TryGetValue(weather, out Form form))
            {
                if (owner.Form.IsShiny && form.HasShinyVersion)
                    targetForm = form.ShinyVersion;
                else
                    targetForm = form;
            }

            if (owner.Form == targetForm) yield break;

            ShowAbilityNotification(owner);
            yield return battleManager.Battlers.ChangeForm(owner, targetForm);
        }
    }
}