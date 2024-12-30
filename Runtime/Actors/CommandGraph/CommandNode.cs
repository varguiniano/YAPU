using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph
{
    /// <summary>
    /// Base class for commands an actor can perform.
    /// </summary>
    [Serializable]
    public abstract class CommandNode : MonsterDatabaseData<CommandNode>
    {
        /// <summary>
        /// Is this command currently running?
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Used by the unity editor.
        /// </summary>
        [HideInInspector]
        // ReSharper disable once InconsistentNaming
        public string name;

        /// <summary>
        /// GUID of this command.
        /// </summary>
        [HideInInspector]
        public string GUID;

        /// <summary>
        /// Position of this node in the graph.
        /// </summary>
        [HideInInspector]
        public Vector2 Position;

        /// <summary>
        /// Event raised when the ports are updated.
        /// </summary>
        public Action OnPortsUpdated;

        /// <summary>
        /// Run this command and all its children.
        /// </summary>
        /// <param name="parameterData">Parameters for the command.</param>
        public virtual IEnumerator RunCommandAndContinue(CommandParameterData parameterData)
        {
            if (parameterData.IsRunningOnLoop)
            {
                yield return new WaitUntil(() => parameterData.Actor.ShouldContinueLoop());
                parameterData.Actor.LoopRunning = true;
            }

            IsRunning = true;
            yield return Run(parameterData, newParameters => parameterData = newParameters);
            IsRunning = false;

            if (parameterData.IsRunningOnLoop) parameterData.Actor.LoopRunning = false;
        }

        /// <summary>
        /// Run the command.
        /// </summary>
        /// <param name="parameterData">Object with all the parameters of the command.</param>
        /// <param name="callback">Callback raised when finished with the modified parameters.</param>
        protected abstract IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback);

        /// <summary>
        /// Get the input ports for this node.
        /// </summary>
        public abstract List<string> GetInputPorts();

        /// <summary>
        /// Get the output ports for this node.
        /// </summary>
        public abstract List<string> GetOutputPorts();

        /// <summary>
        /// Add a child to this node.
        /// </summary>
        /// <param name="child">Child to add.</param>
        /// <param name="index">Index of the port to use.</param>
        public abstract void AddChild(CommandNode child, int index);

        /// <summary>
        /// Remove a child from this node.
        /// </summary>
        /// <param name="child">Child to remove.</param>
        /// <param name="index">Index of the port to use.</param>
        public abstract void RemoveChild(CommandNode child, int index);

        /// <summary>
        /// Get all the children of this node.
        /// Normal command nodes only have a single output port.
        /// </summary>
        /// <returns>A list of all the children.</returns>
        public abstract List<CommandNode> GetChildren();
        
        /// <summary>
        /// Get all the children of this node, including nulls, so they are indexed.
        /// Normal command nodes only have a single output port.
        /// </summary>
        /// <returns>A list of all the children.</returns>
        public abstract List<CommandNode> GetIndexedChildren();
    }
}