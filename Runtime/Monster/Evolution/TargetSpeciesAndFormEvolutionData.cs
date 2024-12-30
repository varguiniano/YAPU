using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;

namespace Varguiniano.YAPU.Runtime.Monster.Evolution
{
    /// <summary>
    /// Base class for evolutions that target a specific species and form.
    /// </summary>
    [Serializable]
    public abstract class TargetSpeciesAndFormEvolutionData : EvolutionData
    {
        /// <summary>
        /// Target species to evolve to.
        /// </summary>
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsters))]
        #endif
        protected MonsterEntry TargetSpecies;
        
        /// <summary>
        /// Target form to evolve to.
        /// </summary>
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllForms))]
        #endif
        protected Form TargetForm;

        /// <summary>
        /// If it's shiny, keep being shiny.
        /// </summary>
        [SerializeField]
        protected bool KeepShinyIfItIs = true;

        /// <summary>
        /// Return the same species and form.
        /// </summary>
        /// <param name="monster">Monster to evolve.</param>
        /// <param name="currentTime">Current time of the day.</param>
        /// <returns>The new species and the same form.</returns>
        public override (MonsterEntry, Form) GetTargetEvolution(MonsterInstance monster, DayMoment currentTime)
        {
            if (KeepShinyIfItIs && TargetForm.HasShinyVersion && monster.Form.IsShiny)
                return (TargetSpecies, TargetForm.ShinyVersion);

            return (TargetSpecies, TargetForm);
        }
    }
}