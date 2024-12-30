using System;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Variables
{
    /// <summary>
    /// Check if a temporal bool variable is true and run commands accordingly.
    /// </summary>
    [Serializable]
    public class IfTemporalBool : IfCondition
    {
        /// <summary>
        /// Name of the variable to check.
        /// </summary>
        [SerializeField]
        private string VariableName;

        /// <summary>
        /// Check if the variable is true.
        /// </summary>
        protected override bool CheckCondition(CommandParameterData parameterData) =>
            parameterData.Actor.TemporalVariables.GetVariable<bool>(VariableName);
    }
}