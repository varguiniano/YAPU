using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Saves;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Variables
{
    /// <summary>
    /// Actor command that compares a temporal int and an int.
    /// </summary>
    [Serializable]
    public class IfCompareTemporalIntAndInt : IfCondition
    {
        /// <summary>
        /// Name of the variable to check.
        /// </summary>
        [SerializeField]
        [PropertyOrder(-1)]
        private string TemporalInt;

        /// <summary>
        /// Variable to check.
        /// </summary>
        [InfoBox("You must choose a variable of the correct type.",
                 InfoMessageType.Error,
                 visibleIfMemberName: nameof(IsVariableNotValid))]
        [SerializeField]
        [BoxGroup]
        [PropertyOrder(-1)]
        private GameVariableReference IntVariable;

        /// <summary>
        /// Returns true if the variable is not a bool.
        /// </summary>
        private bool IsVariableNotValid =>
            IntVariable?.VariableType == null || IntVariable.VariableTypeClass != typeof(int);

        /// <summary>
        /// Mode for this command.
        /// </summary>
        [SerializeField]
        [PropertyOrder(-1)]
        private ComparisonMode Mode;

        /// <summary>
        /// Check the condition.
        /// </summary>
        protected override bool CheckCondition(CommandParameterData parameterData)
        {
            int temporalValue = parameterData.Actor.TemporalVariables.GetVariable<int>(TemporalInt);

            int variableValue = IntVariable.VariableHolder.GetVariable<int>(IntVariable);

            return Mode switch
            {
                ComparisonMode.Equals => temporalValue == variableValue,
                ComparisonMode.GreaterThan => temporalValue > variableValue,
                ComparisonMode.LessThan => temporalValue < variableValue,
                ComparisonMode.GreaterOrEqual => temporalValue >= variableValue,
                ComparisonMode.LessOrEqual => temporalValue <= variableValue,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        /// <summary>
        /// Modes for this command.
        /// </summary>
        public enum ComparisonMode
        {
            Equals,
            GreaterThan,
            LessThan,
            GreaterOrEqual,
            LessOrEqual
        }
    }
}