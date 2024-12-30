using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Saves;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Variables
{
    /// <summary>
    /// If condition variable command that references a single variable.
    /// </summary>
    [Serializable]
    public abstract class IfSingleVariable<T> : IfCondition
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