using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.UI.Dex;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.Monster.Evolution
{
    /// <summary>
    /// Evolution data class for specifically the evolution of Nincada.
    /// https://pokemondb.net/pokedex/shedinja#dex-evolution
    /// </summary>
    [Serializable]
    public class NincadaEvolution : EvolveByLevel
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
        /// Target form to evolve to.
        /// </summary>
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllForms))]
        #endif
        protected Form OtherForm;

        /// <summary>
        /// Balls that are compatible with this evolution.
        /// </summary>
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllBalls))]
        #endif
        private List<Ball> CompatibleBalls;

        /// <summary>
        /// Add the second monster to the roster if there is space and balls.
        /// </summary>
        public override void AfterEvolutionCallback(MonsterInstance evolvedMonster,
                                                    PlayerCharacter playerCharacter,
                                                    YAPUSettings settings,
                                                    MonsterDatabaseInstance database,
                                                    ILocalizer localizer)
        {
            if (playerCharacter.PlayerRoster.RosterSize == 6) return;

            Ball ball = CompatibleBalls.FirstOrDefault(candidate => playerCharacter.PlayerBag.Contains(candidate));

            if (ball == null) return;

            int slot = -1;

            for (int i = 0; i < playerCharacter.PlayerRoster.RosterData.Length; i++)
            {
                MonsterInstance candidate = playerCharacter.PlayerRoster.RosterData[i];

                if (candidate is {IsNullEntry: false}) continue;

                slot = i;
                break;
            }

            if (slot == -1) return;

            Form otherFinalForm = OtherForm;

            if (evolvedMonster.Form.IsShiny && otherFinalForm.HasShinyVersion)
                otherFinalForm = otherFinalForm.ShinyVersion;

            playerCharacter.PlayerRoster[slot] = new MonsterInstance(settings,
                                                                     database,
                                                                     OtherSpecies,
                                                                     otherFinalForm,
                                                                     evolvedMonster.StatData.Level,
                                                                     captureBall: ball,
                                                                     currentTrainer: playerCharacter.CharacterController
                                                                        .GetCharacterData()
                                                                        .GetLocalizedName(localizer),
                                                                     originRegion: localizer
                                                                         [playerCharacter.Region.LocalizableName],
                                                                     originLocation: localizer[playerCharacter.Region
                                                                        .LocalizableName],
                                                                     originalTrainer: playerCharacter
                                                                        .CharacterController
                                                                        .GetCharacterData()
                                                                        .GetLocalizedName(localizer),
                                                                     originType: OriginData.Type.Unknown);

            playerCharacter.PlayerBag.ChangeItemAmount(ball, -1);
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
                                                                         "Dex/Evolutions/ByLevel"
                                                                 },
                                                                 new DexMonsterRelationshipData
                                                                 {
                                                                     Species = OtherSpecies,
                                                                     Form = OtherForm,
                                                                     Gender = gender,
                                                                     Mode = DexMonsterRelationshipData
                                                                           .RelationShipDisplayType.Icon,
                                                                     Icon = CompatibleBalls[0].Icon,
                                                                     LocalizableDescriptionKey =
                                                                         "Dex/Evolutions/Shedinja"
                                                                 },
                                                             };

            return relationships;
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>A cloned object.</returns>
        public override EvolutionData Clone() =>
            new NincadaEvolution
            {
                TargetLevel = TargetLevel,
                TargetSpecies = TargetSpecies,
                TargetForm = TargetForm,
                KeepShinyIfItIs = KeepShinyIfItIs,
                OtherForm = OtherForm,
                OtherSpecies = OtherSpecies,
                CompatibleBalls = CompatibleBalls.ShallowClone()
            };
    }
}