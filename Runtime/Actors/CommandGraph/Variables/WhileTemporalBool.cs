using System;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Variables
{
    /// <summary>
    /// Command to run a loop while a temporal variable is true.
    /// </summary>
    [Serializable]
    public class WhileTemporalBool : WhileCondition
    {
        /// <summary>
        /// Name of the temporal variable to check.
        /// </summary>
        [SerializeField]
        private string VariableName;

        /// <summary>
        /// Run while the variable is true.
        /// </summary>
        protected override bool CheckCondition(CommandParameterData parameterData) =>
            parameterData.Actor.TemporalVariables.GetVariable<bool>(VariableName);
    }
}