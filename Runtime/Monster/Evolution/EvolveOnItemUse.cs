using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
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
    /// Evolution data class that has a monster evolve when an item is used on it.
    /// </summary>
    [Serializable]
    public class EvolveOnItemUse : TargetSpeciesAndFormEvolutionData
    {
        /// <summary>
        /// Item to be used.
        /// </summary>
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllItems))]
        #endif
        public Item ItemToUse;

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
                                                   out bool consumeHeldItem)
        {
            consumeHeldItem = false;
            return false;
        }

        /// <summary>
        /// Check if the monster can evolve after a battle due to some extra data values.
        /// </summary>
        /// <param name="monster">Monster to check.</param>
        /// <param name="currentTime">Current time of the day.</param>
        /// <param name="consumeHeldItem">Should the held item be consumed?</param>
        /// <returns>True if it can evolve.</returns>
        public override bool CanEvolveAfterBattleThroughExtraData(MonsterInstance monster,
                                                                  DayMoment currentTime,
                                                                  out bool consumeHeldItem)
        {
            consumeHeldItem = false;
            return false;
        }

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
                                                    out bool consumeHeldItem)
        {
            consumeHeldItem = false;
            return monster.AllowEvolution(this) && ItemToUse == item;
        }

        /// <summary>
        /// Checks if the monster can evolve when trading it with another.
        /// </summary>
        /// <param name="monster">Monster to check.</param>
        /// <param name="currentTime">Current time of the day.</param>
        /// <param name="otherMonster">Monster it's being traded with.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="consumeHeldItem">Should the held item be consumed?</param>
        /// <returns>True if it can evolve.</returns>
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
                                                                         "Dex/Evolutions/OnItemUse"
                                                                 }
                                                             };

            return relationships;
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>A cloned object.</returns>
        public override EvolutionData Clone() =>
            new EvolveOnItemUse
            {
                ItemToUse = ItemToUse,
                TargetSpecies = TargetSpecies,
                TargetForm = TargetForm,
                KeepShinyIfItIs = KeepShinyIfItIs
            };
    }
}