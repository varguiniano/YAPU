using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph
{
    /// <summary>
    /// Class that represents a command node that can only have one execution path out.
    /// </summary>
    [Serializable]
    public abstract class SingleExecutionPathCommandNode : CommandNode
    {
        /// <summary>
        /// Command to be executed after this one.
        /// Can be null if it's the last command in an execution chain.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        public CommandNode NextCommand;

        /// <summary>
        /// Run this command and all its children.
        /// </summary>
        /// <param name="parameterData">Parameters for the command.</param>
        public override IEnumerator RunCommandAndContinue(CommandParameterData parameterData)
        {
            yield return base.RunCommandAndContinue(parameterData);
            if (NextCommand != null) yield return NextCommand.RunCommandAndContinue(parameterData);
        }

        /// <summary>
        /// Get the input ports for this node.
        /// By default, it has one for execution in.
        /// </summary>
        public override List<string> GetInputPorts() => new() {""};

        /// <summary>
        /// Get the output ports for this node.
        /// By default, it has one for execution out.
        /// </summary>
        public override List<string> GetOutputPorts() => new() {""};

        /// <summary>
        /// Add a child to this node.
        /// Normal command nodes only have a single output port.
        /// </summary>
        /// <param name="child">Child to add.</param>
        /// <param name="index">Index of the port to use.</param>
        public override void AddChild(CommandNode child, int index)
        {
            if (index == 0) NextCommand = child;
        }

        /// <summary>
        /// Remove a child from this node.
        /// Normal command nodes only have a single output port.
        /// </summary>
        /// <param name="child">Child to remove.</param>
        /// <param name="index">Index of the port to use.</param>
        public override void RemoveChild(CommandNode child, int index)
        {
            if (index == 0 && NextCommand == child) NextCommand = null;
        }

        /// <summary>
        /// Get all the children of this node.
        /// Normal command nodes only have a single output port.
        /// </summary>
        /// <returns>A list of all the children.</returns>
        public override List<CommandNode> GetChildren()
        {
            List<CommandNode> children = new();

            if (NextCommand != null) children.Add(NextCommand);

            return children;
        }

        /// <summary>
        /// Get all the children of this node, including nulls, so they are indexed.
        /// Normal command nodes only have a single output port.
        /// </summary>
        /// <returns>A list of all the children.</returns>
        public override List<CommandNode> GetIndexedChildren() => new() {NextCommand};
    }
}