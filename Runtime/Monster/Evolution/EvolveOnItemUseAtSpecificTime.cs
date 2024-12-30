using System;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.UI.Dex;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.Monster.Evolution
{
    /// <summary>
    /// Evolution data class that has a monster evolve when an item is used on it at a specific time.
    /// </summary>
    [Serializable]
    public class EvolveOnItemUseAtSpecificTime : EvolveOnItemUse
    {
        /// <summary>
        /// Times at which the monster can evolve.
        /// </summary>
        [SerializeField]
        protected List<DayMoment> EvolvingTimes;

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
         && EvolvingTimes.Contains(currentTime);

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
                                                                         "Dex/Evolutions/OnItemUseAtSpecificTimes"
                                                                 }
                                                             };

            return relationships;
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>A cloned object.</returns>
        public override EvolutionData Clone() =>
            new EvolveOnItemUseAtSpecificTime
            {
                ItemToUse = ItemToUse,
                TargetSpecies = TargetSpecies,
                TargetForm = TargetForm,
                KeepShinyIfItIs = KeepShinyIfItIs,
                EvolvingTimes = EvolvingTimes.ShallowClone()
            };
    }
}