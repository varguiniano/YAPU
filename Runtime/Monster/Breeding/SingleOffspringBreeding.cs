using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.UI.Dex;
using WhateverDevs.Core.Runtime.Common;

namespace Varguiniano.YAPU.Runtime.Monster.Breeding
{
    /// <summary>
    /// Class representing a normal breeding data that provides several forms of offspring of the same species.
    /// </summary>
    [Serializable]
    public class SingleOffspringBreeding : BreedingData
    {
        /// <summary>
        /// Species to create as offspring.
        /// </summary>
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsters))]
        #endif
        private MonsterEntry OffspringSpecies;

        /// <summary>
        /// Possible forms the offspring can have.
        /// </summary>
        [SerializeField]
        [InfoBox("The nursery may prioritize specific forms. Otherwise the first one will be used. There's no need to add shiny forms.")]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllForms))]
        #endif
        private List<Form> PossibleForms;

        /// <summary>
        /// Get the species and form of the offspring.
        /// This method doesn't check egg groups.
        /// </summary>
        /// <param name="monster">Current monster.</param>
        /// <param name="otherParent">The other parent.</param>
        /// <returns>A list of tuples with the possible species and forms of the offspring.</returns>
        public override List<(MonsterEntry, Form)> GetPossibleOffspring(MonsterInstance monster,
                                                                        MonsterInstance otherParent) =>
            PossibleForms.Select(form => (OffspringSpecies, form)).ToList();

        /// <summary>
        /// Get the relationships this breeding data can have to be displayed on the dex.
        /// </summary>
        /// <param name="entry">Monster entry being displayed.</param>
        /// <param name="formEntry">Form entry being displayed.</param>
        /// <returns>A list of the relationships generated.</returns>
        public override List<DexMonsterRelationshipData> GetDexRelationships(MonsterDexEntry entry,
                                                                             FormDexEntry formEntry)
        {
            List<DexMonsterRelationshipData> relationships = new();

            foreach (Form form in PossibleForms)
            {
                DataByFormEntry formData = entry.Species[formEntry.Form];

                // No need to add the normal if the species doesn't have females.
                if (formData.HasBinaryGender && formData.FemaleRatio > .001f) // Float tolerance.
                    relationships.Add(new DexMonsterRelationshipData
                                      {
                                          Mode = DexMonsterRelationshipData.RelationShipDisplayType
                                                                           .PresetIcon,
                                          PresetIcon = DexMonsterRelationshipData.IconType.NormalBreeding,
                                          Species = OffspringSpecies,
                                          Form = form,
                                          Gender = MonsterGender.Female,
                                          LocalizableDescriptionKey =
                                              "Dex/Relationships/NormalBreeding/Description"
                                      });

                relationships.Add(new DexMonsterRelationshipData
                                  {
                                      Mode = DexMonsterRelationshipData.RelationShipDisplayType
                                                                       .PresetIcon,
                                      PresetIcon = DexMonsterRelationshipData.IconType.DittoBreeding,
                                      Species = OffspringSpecies,
                                      Form = form,
                                      Gender = MonsterGender.Female,
                                      LocalizableDescriptionKey =
                                          "Dex/Relationships/DittoBreeding/Description"
                                  });
            }

            return relationships;
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>A cloned object.</returns>
        public override BreedingData Clone() =>
            new SingleOffspringBreeding
            {
                OffspringSpecies = OffspringSpecies,
                PossibleForms = PossibleForms.ShallowClone()
            };
    }
}