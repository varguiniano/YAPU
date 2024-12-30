using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Variables
{
    /// <summary>
    /// Command to run a loop while a condition is true.
    /// </summary>
    [Serializable]
    public abstract class WhileCondition : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Commands run when the condition is true.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        private CommandNode Loop;

        /// <summary>
        /// Run the loop while it's true.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData, Action<CommandParameterData> callback)
        {
            while (CheckCondition(parameterData)) yield return Loop.RunCommandAndContinue(parameterData);
        }

        /// <summary>
        /// Check the condition.
        /// </summary>
        protected abstract bool CheckCondition(CommandParameterData parameterData);

        /// <summary>
        /// Get the output ports for this node.
        /// By default, it has one for execution out.
        /// </summary>
        public override List<string> GetOutputPorts() =>
            new()
            {
                "",
                "Loop"
            };

        /// <summary>
        /// Add a child to this node.
        /// Normal command nodes only have a single output port.
        /// </summary>
        /// <param name="child">Child to add.</param>
        /// <param name="index">Index of the port to use.</param>
        public override void AddChild(CommandNode child, int index)
        {
            switch (index)
            {
                case 0:
                    NextCommand = child;
                    break;
                case 1:
                    Loop = child;
                    break;
            }
        }

        /// <summary>
        /// Remove a child from this node.
        /// Normal command nodes only have a single output port.
        /// </summary>
        /// <param name="child">Child to remove.</param>
        /// <param name="index">Index of the port to use.</param>
        public override void RemoveChild(CommandNode child, int index)
        {
            switch (index)
            {
                case 0 when NextCommand == child:
                    NextCommand = null;
                    break;
                case 1 when Loop == child:
                    Loop = null;
                    break;
            }
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
            if (Loop != null) children.Add(Loop);

            return children;
        }

        /// <summary>
        /// Get all the children of this node, including nulls, so they are indexed.
        /// Normal command nodes only have a single output port.
        /// </summary>
        /// <returns>A list of all the children.</returns>
        public override List<CommandNode> GetIndexedChildren() =>
            new()
            {
                NextCommand,
                Loop
            };
    }
}