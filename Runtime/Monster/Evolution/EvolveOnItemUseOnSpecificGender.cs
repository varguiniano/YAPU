using System;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.UI.Dex;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.Monster.Evolution
{
    /// <summary>
    /// Evolution data class that has a monster evolve when an item is used on it and has a specific gender.
    /// </summary>
    [Serializable]
    public class EvolveOnItemUseOnSpecificGender : EvolveOnItemUse
    {
        /// <summary>
        /// Gender to match.
        /// </summary>
        [SerializeField]
        private MonsterGender Gender;

        /// <summary>
        /// Checks if the monster can evolve when using an item on it.
        /// </summary>
        /// <param name="monster">Monster to check.</param>
        /// <param name="currentTime">Current time of the day.</param>
        /// <param name="item">Item being used on the monster.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="consumeHeldItem">Should the held item be consumed?</param>
        /// <returns>True if it can evolve.</returns>
        public override bool CanEvolveWhenUsingItem(MonsterInstance monster,
                                                    DayMoment currentTime,
                                                    Item item,
                                                    PlayerCharacter playerCharacter,
                                                    out bool consumeHeldItem) =>
            base.CanEvolveWhenUsingItem(monster, currentTime, item, playerCharacter, out consumeHeldItem)
         && monster.PhysicalData.Gender == Gender;

        /// <summary>
        /// Get the dex relationships to display for this evolution.
        /// </summary>
        public override List<DexMonsterRelationshipData> GetDexRelationships(MonsterDexEntry entry,
                                                                             FormDexEntry formEntry,
                                                                             MonsterGender gender,
                                                                             ILocalizer localizer)
        {
            List<DexMonsterRelationshipData> relationships = new()
                                                             {
                                                                 new DexMonsterRelationshipData
                                                                 {
                                                                     Species = TargetSpecies,
                                                                     Form = TargetForm,
                                                                     Gender = gender,
                                                                     Mode = DexMonsterRelationshipData
                                                                           .RelationShipDisplayType.Icon,
                                                                     Icon = ItemToUse.Icon,
                                                                     LocalizableDescriptionKey =
                                                                         "Dex/Evolutions/OnItemUseOnSpecificGender"
                                                                 }
                                                             };

            return relationships;
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>A cloned object.</returns>
        public override EvolutionData Clone() =>
            new EvolveOnItemUseOnSpecificGender
            {
                ItemToUse = ItemToUse,
                TargetSpecies = TargetSpecies,
                TargetForm = TargetForm,
                KeepShinyIfItIs = KeepShinyIfItIs,
                Gender = Gender
            };
    }
}