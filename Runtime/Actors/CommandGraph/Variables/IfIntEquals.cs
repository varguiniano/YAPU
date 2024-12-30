using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Variables
{
    /// <summary>
    /// Check if an int variable equals a value.
    /// </summary>
    [Serializable]
    public class IfIntEquals : IfSingleVariable<int>
    {
        /// <summary>
        /// Value to compare.
        /// </summary>
        [SerializeField]
        [PropertyOrder(-1)]
        private int Value;

        /// <summary>
        /// Check the condition.
        /// </summary>
        protected override bool CheckCondition(CommandParameterData parameterData) => VariableValue == Value;
    }
}