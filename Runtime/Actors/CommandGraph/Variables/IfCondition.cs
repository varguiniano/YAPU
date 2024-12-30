using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Variables
{
    /// <summary>
    /// Base command for commands that check an if condition.
    /// </summary>
    [Serializable]
    public abstract class IfCondition : CommandNode
    {
        /// <summary>
        /// Command to be executed after this one.
        /// Can be null if it's the last command in an execution chain.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        public CommandNode True;

        /// <summary>
        /// Command to be executed after this one.
        /// Can be null if it's the last command in an execution chain.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        public CommandNode False;

        /// <summary>
        /// Run this command and all its children.
        /// </summary>
        /// <param name="parameterData">Parameters for the command.</param>
        public override IEnumerator RunCommandAndContinue(CommandParameterData parameterData)
        {
            yield return base.RunCommandAndContinue(parameterData);

            if (CheckCondition(parameterData))
            {
                if (True != null) yield return True.RunCommandAndContinue(parameterData);
            }
            else
            {
                if (False != null) yield return False.RunCommandAndContinue(parameterData);
            }
        }

        /// <summary>
        /// No need to do anything here.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            yield break;
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
        public override List<string> GetOutputPorts() =>
            new()
            {
                "True",
                "False"
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
                    True = child;
                    break;
                case 1:
                    False = child;
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
                case 0 when True == child:
                    True = null;
                    break;
                case 1 when False == child:
                    False = null;
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

            if (True != null) children.Add(True);
            if (False != null) children.Add(False);

            return children;
        }

        /// <summary>
        /// Get all the children of this node, including nulls, so they are indexed.
        /// </summary>
        /// <returns>A list of all the children.</returns>
        public override List<CommandNode> GetIndexedChildren() =>
            new()
            {
                True,
                False
            };

        /// <summary>
        /// Check the condition.
        /// </summary>
        protected abstract bool CheckCondition(CommandParameterData parameterData);
    }
}