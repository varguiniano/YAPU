using System;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph.Variables;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Dex
{
    /// <summary>
    /// If command that checks if the player has the completed dex.
    /// </summary>
    [Serializable]
    public class IfDexIsComplete : IfCondition
    {
        /// <summary>
        /// Mode to check.
        /// </summary>
        [SerializeField]
        private CompletionMode Mode;

        /// <summary>
        /// Check if they have the dex and if it is complete.
        /// </summary>
        protected override bool CheckCondition(CommandParameterData parameterData)
        {
            if (!parameterData.GlobalGameData.HasDex) return false;

            MonsterDex.Dex dex = parameterData.PlayerCharacter.PlayerDex;

            return Mode switch
            {
                CompletionMode.Seen => dex.NumberOfMonsters == dex.NumberSeenInAtLeastOneForm,
                CompletionMode.Caught => dex.NumberOfMonsters == dex.NumberCaughtInAtLeastOneForm,
                CompletionMode.CaughtInAllForms => dex.NumberOfFormsIncludingGenderDifferences
                                                == dex.NumberCaughtIncludingFormsAndGenders,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        /// <summary>
        /// Modes in which the dex can be completed.
        /// </summary>
        public enum CompletionMode
        {
            Seen,
            Caught,
            CaughtInAllForms
        }
    }
}