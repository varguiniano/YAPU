using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Saves;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Variables
{
    /// <summary>
    /// Command to run a loop while a variable is true.
    /// </summary>
    [Serializable]
    public abstract class WhileVariable<T> : WhileCondition
    {
        /// <summary>
        /// Variable to check.
        /// </summary>
        [InfoBox("You must choose a variable of the correct type.",
                 InfoMessageType.Error,
                 visibleIfMemberName: nameof(IsVariableNotValid))]
        [SerializeField]
        [BoxGroup]
        [PropertyOrder(-1)]
        protected GameVariableReference Variable;

        /// <summary>
        /// Get the value of the variable.
        /// </summary>
        protected T VariableValue => Variable.VariableHolder.GetVariable<T>(Variable);

        /// <summary>
        /// Returns true if the variable is not a bool.
        /// </summary>
        protected bool IsVariableNotValid => Variable?.VariableType == null || Variable.VariableTypeClass != typeof(T);
    }
}