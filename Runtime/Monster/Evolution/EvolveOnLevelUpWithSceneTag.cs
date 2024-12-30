using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.UI.Dex;
using Varguiniano.YAPU.Runtime.World;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.Monster.Evolution
{
    /// <summary>
    /// Evolution data class that has a monster evolve when leveling up on a scene that has a specific tag.
    /// </summary>
    [Serializable]
    public class EvolveOnLevelUpWithSceneTag : TargetSpeciesAndFormEvolutionData
    {
        /// <summary>
        /// Tags that are compatible with this evolution.
        /// </summary>
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllSceneTags))]
        #endif
        protected List<SceneTag> CompatibleTags;

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
            return monster.AllowEvolution(this) && CompatibleTags.Any(playerCharacter.Scene.HasTag);
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
                                                                             ILocalizer localizer) =>
            CompatibleTags.Select(tag => new DexMonsterRelationshipData
                                         {
                                             Species = TargetSpecies,
                                             Form = TargetForm,
                                             Gender = gender,
                                             Mode = DexMonsterRelationshipData.RelationShipDisplayType.Text,
                                             Text = localizer["Dex/Evolutions/LevelUpWithSceneTag"]
                                                  + "\n"
                                                  + localizer[tag.LocalizableName],
                                             LocalizableDescriptionKey =
                                                 "Dex/Evolutions/LevelUpWithSceneTag/Description"
                                         })
                          .ToList();

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>A cloned object.</returns>
        public override EvolutionData Clone() =>
            new EvolveOnLevelUpWithSceneTag
            {
                CompatibleTags = CompatibleTags.ShallowClone(),
                TargetSpecies = TargetSpecies,
                TargetForm = TargetForm,
                KeepShinyIfItIs = KeepShinyIfItIs
            };
    }
}