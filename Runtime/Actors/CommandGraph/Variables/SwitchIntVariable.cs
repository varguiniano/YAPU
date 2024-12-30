using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Variables
{
    /// <summary>
    /// Command that switches with an int variable as the condition.
    /// </summary>
    [Serializable]
    public class SwitchIntVariable : VariableCommand<int>
    {
        /// <summary>
        /// List of cases for the switch.
        /// </summary>
        [SerializeField]
        private List<int> Cases = new();

        /// <summary>
        /// Child nodes that will be run when the case is met.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        private List<CommandNode> CaseNodes = new();

        /// <summary>
        /// Node that will be run by default if no case is met.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        private CommandNode DefaultNode;

        /// <summary>
        /// Run this command and all its children.
        /// </summary>
        /// <param name="parameterData">Parameters for the command.</param>
        public override IEnumerator RunCommandAndContinue(CommandParameterData parameterData)
        {
            yield return base.RunCommandAndContinue(parameterData);

            if (Cases.Contains(VariableValue))
                yield return CaseNodes[Cases.IndexOf(VariableValue)].RunCommandAndContinue(parameterData);
            else
                yield return DefaultNode.RunCommandAndContinue(parameterData);
        }

        /// <summary>
        /// Nothing to do here.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            yield break;
        }

        /// <summary>
        /// Update the cases list when the cases are updated.
        /// </summary>
        [Button]
        private void UpdateCases()
        {
            if (Cases.Count < CaseNodes.Count)
                CaseNodes.RemoveRange(Cases.Count, CaseNodes.Count - Cases.Count);
            else if (Cases.Count > CaseNodes.Count)
                CaseNodes.AddRange(Enumerable.Repeat<CommandNode>(null, Cases.Count - CaseNodes.Count));

            OnPortsUpdated?.Invoke();
        }

        /// <summary>
        /// Get the input ports for this node.
        /// </summary>
        public override List<string> GetInputPorts() => new() {""};

        /// <summary>
        /// Get the output ports for this node.
        /// </summary>
        public override List<string> GetOutputPorts()
        {
            List<string> ports = new() {"Default"};

            ports.AddRange(Cases.Select(caseInt => caseInt.ToString()).ToList());

            return ports;
        }

        /// <summary>
        /// Add a child to this node.
        /// Normal command nodes only have a single output port.
        /// </summary>
        /// <param name="child">Child to add.</param>
        /// <param name="index">Index of the port to use.</param>
        public override void AddChild(CommandNode child, int index)
        {
            if (index == 0)
                DefaultNode = child;
            else
            {
                index--;
                if (index < Cases.Count) CaseNodes[index] = child;
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
            if (index == 0 && child == DefaultNode)
                DefaultNode = null;
            else
            {
                index--;
                if (index < Cases.Count && CaseNodes[index] == child) CaseNodes[index] = null;
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

            if (DefaultNode != null) children.Add(DefaultNode);

            children.AddRange(CaseNodes.Where(node => node != null));

            return children;
        }

        /// <summary>
        /// Get all the children of this node, including nulls, so they are indexed.
        /// Normal command nodes only have a single output port.
        /// </summary>
        /// <returns>A list of all the children.</returns>
        public override List<CommandNode> GetIndexedChildren()
        {
            List<CommandNode> children = new() {DefaultNode};

            children.AddRange(CaseNodes);

            return children;
        }
    }
}