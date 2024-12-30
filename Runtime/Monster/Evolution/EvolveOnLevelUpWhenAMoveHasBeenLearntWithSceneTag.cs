using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.UI.Dex;
using Varguiniano.YAPU.Runtime.World;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.Monster.Evolution
{
    /// <summary>
    /// Evolution data class that has a monster evolve when it knows a certain move, but only in a scene with a specific tag.
    /// </summary>
    [Serializable]
    public class EvolveOnLevelUpWhenAMoveHasBeenLearntWithSceneTag : EvolveOnLevelUpWhenAMoveHasBeenLearnt
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
                                                   out bool consumeHeldItem) =>
            base.CanEvolveAfterLevelUp(monster, currentTime, playerCharacter, out consumeHeldItem)
         && CompatibleTags.Any(playerCharacter.Scene.HasTag);

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
                                             Text = Move.GetLocalizedName(localizer)
                                                  + "\n"
                                                  + localizer[tag.LocalizableName],
                                             LocalizableDescriptionKey =
                                                 "Dex/Evolutions/WhenAMoveHasBeenLearntWithSceneTag"
                                         })
                          .ToList();

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>A cloned object.</returns>
        public override EvolutionData Clone() =>
            new EvolveOnLevelUpWhenAMoveHasBeenLearntWithSceneTag
            {
                CompatibleTags = CompatibleTags.ShallowClone(),
                Move = Move,
                TargetSpecies = TargetSpecies,
                TargetForm = TargetForm,
                KeepShinyIfItIs = KeepShinyIfItIs
            };
    }
}