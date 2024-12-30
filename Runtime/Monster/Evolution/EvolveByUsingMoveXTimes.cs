using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDex;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.Monster.Evolution
{
    /// <summary>
    /// Evolution data class that has a monster evolve when it has used a move X times.
    /// </summary>
    [Serializable]
    [InfoBox("Remember to set the move to add to this species evolution counter in the move's scriptable object.")]
    public class EvolveByUsingMoveXTimes : EvolveByEvolutionCounter
    {
        /// <summary>
        /// Move to use.
        /// Only used for localization.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMoves))]
        #endif
        [SerializeField]
        private Move Move;

        /// <summary>
        /// Get the text to show on the dex relationship data.
        /// </summary>
        protected override string GetDexText(MonsterDexEntry entry,
                                             FormDexEntry formEntry,
                                             MonsterGender gender,
                                             ILocalizer localizer) =>
            Move.GetLocalizedName(localizer) + " - " + TargetCounter;

        /// <summary>
        /// Get the key to show on the dex relationship data.
        /// </summary>
        protected override string GetDexDescriptionKey(MonsterDexEntry entry,
                                                       FormDexEntry formEntry,
                                                       MonsterGender gender) =>
            "Dex/Evolutions/UseMoveXTimes/Description";

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>A cloned object.</returns>
        public override EvolutionData Clone() =>
            new EvolveByUsingMoveXTimes
            {
                TargetCounter = TargetCounter,
                TargetSpecies = TargetSpecies,
                TargetForm = TargetForm,
                KeepShinyIfItIs = KeepShinyIfItIs
            };
    }
}