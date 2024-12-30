using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.UI.Dex;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.Monster.Evolution
{
    /// <summary>
    /// Evolution data class that has a monster evolve to a "random" species when it reaches a certain level.
    /// </summary>
    [Serializable]
    public class EvolveToRandomSpeciesByLevel : EvolveByLevel
    {
        /// <summary>
        /// Target species to evolve to.
        /// </summary>
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsters))]
        #endif
        protected MonsterEntry OtherSpecies;

        /// <summary>
        /// Second form that it can evolve to.
        /// </summary>
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllForms))]
        #endif
        private Form OtherForm;

        /// <summary>
        /// Any monster with a personality value equal or higher than this will evolve to the other species, otherwise it will evolve to the target species.
        /// </summary>
        [InfoBox("Any monster with a personality value equal or higher than this will evolve to the other species, otherwise it will evolve to the target species.")]
        [SerializeField]
        private int PersonalityThreshold = 2000000000;

        /// <summary>
        /// Return the same species and form.
        /// </summary>
        /// <param name="monster">Monster to evolve.</param>
        /// <param name="currentTime">Current time of the day.</param>
        /// <returns>The new species and the same form.</returns>
        public override (MonsterEntry, Form) GetTargetEvolution(MonsterInstance monster, DayMoment currentTime)
        {
            MonsterEntry targetSpecies = monster.ExtraData.PersonalityValue >= PersonalityThreshold ? OtherSpecies : TargetSpecies;
            Form targetForm = monster.ExtraData.PersonalityValue >= PersonalityThreshold ? OtherForm : TargetForm;

            if (KeepShinyIfItIs && targetForm.HasShinyVersion && monster.Form.IsShiny)
                return (targetSpecies, targetForm.ShinyVersion);

            return (targetSpecies, targetForm);
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
                                                                           .RelationShipDisplayType.Text,
                                                                     Text = "Lv. " + TargetLevel,
                                                                     LocalizableDescriptionKey =
                                                                         "Dex/Evolutions/EvolveToRandomSpeciesByLevel"
                                                                 },
                                                                 new DexMonsterRelationshipData
                                                                 {
                                                                     Species = OtherSpecies,
                                                                     Form = OtherForm,
                                                                     Gender = gender,
                                                                     Mode = DexMonsterRelationshipData
                                                                           .RelationShipDisplayType.Text,
                                                                     Text = "Lv. " + TargetLevel,
                                                                     LocalizableDescriptionKey =
                                                                         "Dex/Evolutions/EvolveToRandomSpeciesByLevel"
                                                                 }
                                                             };

            return relationships;
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>A cloned object.</returns>
        public override EvolutionData Clone() =>
            new EvolveToRandomSpeciesByLevel
            {
                TargetLevel = TargetLevel,
                TargetSpecies = TargetSpecies,
                TargetForm = TargetForm,
                KeepShinyIfItIs = KeepShinyIfItIs,
                OtherSpecies = OtherSpecies,
                OtherForm = OtherForm
            };
    }
}