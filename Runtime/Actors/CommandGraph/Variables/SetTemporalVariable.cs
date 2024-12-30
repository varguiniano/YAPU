using System;
using System.Collections;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Variables
{
    /// <summary>
    /// Actor command to set a temporal variable.
    /// </summary>
    /// <typeparam name="T">Type of variable.</typeparam>
    [Serializable]
    public abstract class SetTemporalVariable<T> : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Name of the variable.
        /// </summary>
        [SerializeField]
        private string Name;

        /// <summary>
        /// Value to set.
        /// </summary>
        [SerializeField]
        private T Value;

        /// <summary>
        /// Set the value.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData, Action<CommandParameterData> callback)
        {
            parameterData.Actor.TemporalVariables.SetVariable(Name, Value);
            yield break;
        }
    }
}