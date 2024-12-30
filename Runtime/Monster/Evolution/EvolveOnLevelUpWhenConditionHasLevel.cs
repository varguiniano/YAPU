using System;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.UI.Dex;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.Monster.Evolution
{
    /// <summary>
    ///  Evolution data class that has a monster evolve when levels up with a condition at a certain level.
    /// </summary>
    [Serializable]
    public class EvolveOnLevelUpWhenConditionHasLevel : TargetSpeciesAndFormEvolutionData
    {
        /// <summary>
        /// Conditions to match.
        /// </summary>
        [SerializeField]
        private SerializableDictionary<Condition, byte> Conditions;

        /// <summary>
        /// Evolve if in the correct scene.
        /// </summary>
        /// <param name="monster"></param>
        /// <param name="currentTime"></param>
        /// <param name="playerCharacter"></param>
        /// <param name="consumeHeldItem">Should the held item be consumed?</param>
        /// <returns></returns>
        public override bool CanEvolveAfterLevelUp(MonsterInstance monster,
                                                   DayMoment currentTime,
                                                   PlayerCharacter playerCharacter,
                                                   out bool consumeHeldItem)
        {
            consumeHeldItem = false;

            foreach (KeyValuePair<Condition, byte> pair in Conditions)
                if (monster.Conditions[pair.Key] < pair.Value)
                    return false;

            return monster.AllowEvolution(this);
        }

        /// <summary>
        /// Only after level up.
        /// </summary>
        public override bool CanEvolveAfterBattleThroughExtraData(MonsterInstance monster,
                                                                  DayMoment currentTime,
                                                                  out bool consumeHeldItem)
        {
            consumeHeldItem = false;
            return false;
        }

        /// <summary>
        /// Only after level up.
        /// </summary>
        public override bool CanEvolveWhenUsingItem(MonsterInstance monster,
                                                    DayMoment currentTime,
                                                    Item item,
                                                    PlayerCharacter playerCharacter,
                                                    out bool consumeHeldItem)
        {
            consumeHeldItem = false;
            return false;
        }

        /// <summary>
        /// Only after level up.
        /// </summary>
        public override bool CanEvolveWhenTrading(MonsterInstance monster,
                                                  DayMoment currentTime,
                                                  MonsterInstance otherMonster,
                                                  PlayerCharacter playerCharacter,
                                                  out bool consumeHeldItem)
        {
            consumeHeldItem = false;
            return false;
        }

        /// <summary>
        /// Get the dex relationships to display for this evolution.
        /// </summary>
        public override List<DexMonsterRelationshipData> GetDexRelationships(MonsterDexEntry entry,
                                                                             FormDexEntry formEntry,
                                                                             MonsterGender gender,
                                                                             ILocalizer localizer)
        {
            string localizationText = "";

            foreach (Condition condition in Conditions.Keys)
            {
                if (!localizationText.IsNullEmptyOrWhiteSpace()) localizationText += "\n";
                localizationText += localizer[condition.GetLocalizationString()];
            }

            List<DexMonsterRelationshipData> relationships = new()
                                                             {
                                                                 new DexMonsterRelationshipData
                                                                 {
                                                                     Species = TargetSpecies,
                                                                     Form = TargetForm,
                                                                     Gender = gender,
                                                                     Mode = DexMonsterRelationshipData
                                                                           .RelationShipDisplayType.Text,
                                                                     Text = localizationText,
                                                                     LocalizableDescriptionKey =
                                                                         "Dex/Evolutions/LevelUpWithConditions/Description"
                                                                 }
                                                             };

            return relationships;
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>A cloned object.</returns>
        public override EvolutionData Clone() =>
            new EvolveOnLevelUpWhenConditionHasLevel
            {
                Conditions = Conditions.ShallowClone(),
                TargetSpecies = TargetSpecies,
                TargetForm = TargetForm,
                KeepShinyIfItIs = KeepShinyIfItIs
            };
    }
}