using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Saves;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Variables
{
    /// <summary>
    /// Base class for commands that depend on a variable.
    /// </summary>
    /// <typeparam name="T">Type of the variable.</typeparam>
    [Serializable]
    public abstract class VariableCommand<T> : CommandNode
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