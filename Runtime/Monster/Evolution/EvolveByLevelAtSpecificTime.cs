﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.UI.Dex;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.Monster.Evolution
{
    /// <summary>
    /// Evolution data class that has a monster evolve when it's on or above a certain level and it's a specific time.
    /// </summary>
    [Serializable]
    public class EvolveByLevelAtSpecificTime : EvolveByLevel
    {
        /// <summary>
        /// Times at which the monster can evolve.
        /// </summary>
        [SerializeField]
        protected List<DayMoment> EvolvingTimes;

        /// <summary>
        /// Check if it is on or above the level.
        /// </summary>
        /// <param name="monster">Monster to check.</param>
        /// <param name="currentTime">Current time of the day.</param>
        /// <param name="playerCharacter"></param>
        /// <param name="consumeHeldItem">Should the held item be consumed?</param>
        /// <returns>True if it can evolve.</returns>
        public override bool CanEvolveAfterLevelUp(MonsterInstance monster,
                                                   DayMoment currentTime,
                                                   PlayerCharacter playerCharacter,
                                                   out bool consumeHeldItem) =>
            base.CanEvolveAfterLevelUp(monster, currentTime, playerCharacter, out consumeHeldItem)
         && EvolvingTimes.Contains(currentTime);

        /// <summary>
        /// Get the dex relationships to display for this evolution.
        /// </summary>
        public override List<DexMonsterRelationshipData> GetDexRelationships(MonsterDexEntry entry,
                                                                             FormDexEntry formEntry,
                                                                             MonsterGender gender,
                                                                             ILocalizer localizer) =>
            EvolvingTimes.Select(moment => new DexMonsterRelationshipData
                                           {
                                               Species = TargetSpecies,
                                               Form = TargetForm,
                                               Gender = gender,
                                               Mode = DexMonsterRelationshipData
                                                     .RelationShipDisplayType.Text,
                                               Text = "Lv. "
                                                    + TargetLevel
                                                    + "\n"
                                                    + localizer[moment.AtTimeToLocalizableKey()],
                                               LocalizableDescriptionKey =
                                                   "Dex/Evolutions/ByLevelAtSpecificTime"
                                           })
                         .ToList();

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>A cloned object.</returns>
        public override EvolutionData Clone() =>
            new EvolveByLevelAtSpecificTime
            {
                TargetLevel = TargetLevel,
                EvolvingTimes = EvolvingTimes.ShallowClone(),
                TargetSpecies = TargetSpecies,
                TargetForm = TargetForm,
                KeepShinyIfItIs = KeepShinyIfItIs
            };
    }
}