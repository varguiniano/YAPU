using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Berries;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.World;
using Varguiniano.YAPU.Runtime.World.Encounters;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Harvest ability implementation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Harvest", fileName = "Harvest")]
    public class Harvest : Ability
    {
        /// <summary>
        /// Chance to harvest an item after each turn.
        /// </summary>
        [SerializeField]
        private float HarvestChance = 0.5f;

        /// <summary>
        /// Weathers that guarantee a harvest.
        /// </summary>
        [SerializeField]
        private List<Weather> GuaranteeingWeathers;

        /// <summary>
        /// Reference to the type this ability attracts.
        /// </summary>
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsterTypes))]
        #endif
        private MonsterType AttractedType;

        /// <summary>
        /// Pickup a recyclable item.
        /// </summary>
        public override IEnumerator AfterTurnPreStatus(Battler battler,
                                                       BattleManager battleManager,
                                                       ILocalizer localizer)
        {
            yield return base.AfterTurnPreStatus(battler, battleManager, localizer);

            if (battler.HeldItem != null
             || !battler.ConsumedItemData.HasConsumedItem
             || !battler.ConsumedItemData.CanBeRecycled
             || battler.ConsumedItemData.ConsumedItem is not Berry)
                yield break;

            float roll = battleManager.Scenario.GetWeather(out Weather weather)
                      && GuaranteeingWeathers.Contains(weather)
                             ? 0
                             : Random.value;

            if (roll > HarvestChance) yield break;

            ShowAbilityNotification(battler);

            battler.HeldItem = battler.ConsumedItemData.ConsumedItem;

            battler.ConsumedItemData.CanBeRecycled = false;

            yield return DialogManager.ShowDialogAndWait("Abilities/Harvest/Effect",
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed,
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        battler.GetLocalizedName(localizer),
                                                                        battler.HeldItem.GetLocalizedName(localizer)
                                                                    });

            yield return battler.HeldItem.OnItemReceivedInBattle(battler, battleManager);
        }

        /// <summary>
        /// 50% chance of forcing an encounter with the attracted type.
        /// </summary>
        /// <param name="possibleEncounters">Current possible encounters.</param>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="sceneInfo">Info for the current scene.</param>
        /// <param name="encounterType">Encounter type.</param>
        public override void ModifyPossibleEncounters(ref List<WildEncounter> possibleEncounters,
                                                      MonsterInstance owner,
                                                      SceneInfo sceneInfo,
                                                      EncounterType encounterType)
        {
            if (!(Random.value <= .5f)) return;
            Logger.Info("Attempting to force an encounter with a " + AttractedType.name + " type.");

            List<WildEncounter> newCandidates = possibleEncounters
                                               .Where(encounter =>
                                                          encounter
                                                             .Monster[encounter.FormCalculator
                                                                         .GetEncounterForm(sceneInfo,
                                                                              encounterType)]
                                                             .IsOfType(AttractedType))
                                               .ToList();

            if (newCandidates.Count > 0) possibleEncounters = newCandidates;
        }
    }
}