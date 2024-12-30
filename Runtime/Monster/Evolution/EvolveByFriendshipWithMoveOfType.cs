using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.UI.Dex;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.Monster.Evolution
{
    /// <summary>
    /// Evolution data class that evolves when the monster reaches a certain friendship level and knows a move of the specific type.
    /// </summary>
    [Serializable]
    public class EvolveByFriendshipWithMoveOfType : EvolveByFriendship
    {
        /// <summary>
        /// Type that needs to know to evolve.
        /// </summary>
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsterTypes))]
        #endif
        private MonsterType Type;

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
         && monster.CurrentMoves.Any(slot => slot.Move != null
                                          && slot.Move.GetMoveType(monster, playerCharacter.YAPUSettings) == Type);

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
                                                                           .RelationShipDisplayType.Text,
                                                                     Text = localizer["Dex/Evolutions/ByFriendship"]
                                                                          + TargetFriendship
                                                                          + "\n"
                                                                          + localizer[Type.LocalizableName],
                                                                     LocalizableDescriptionKey =
                                                                         "Dex/Evolutions/ByFriendshipWithMoveOfType/Description"
                                                                 }
                                                             };

            return relationships;
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>A cloned object.</returns>
        public override EvolutionData Clone() =>
            new EvolveByFriendshipWithMoveOfType
            {
                Type = Type,
                TargetFriendship = TargetFriendship,
                TargetSpecies = TargetSpecies,
                TargetForm = TargetForm
            };
    }
}