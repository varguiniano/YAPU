using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph
{
    /// <summary>
    /// Class that holds the graph of commands an actor can execute.
    /// </summary>
    [Serializable]
    public class ActorCommandGraph
    {
        /// <summary>
        /// Is this graph currently running?
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Root node of this graph.
        /// </summary>
        [SerializeReference]
        public CommandNode RootNode;

        /// <summary>
        /// Not all nodes may be attached to the root node,
        /// so we need to keep a list of all nodes in the graph.
        /// </summary>
        [SerializeReference]
        public List<CommandNode> Nodes = new();

        /// <summary>
        /// Flag to check if the graph is empty.
        /// </summary>
        public bool IsGraphEmpty => RootNode == null || RootNode.GetChildren().Count == 0;

        /// <summary>
        /// Run this graph.
        /// </summary>
        /// <param name="parameterData">Parameters for the commands.</param>
        /// <param name="onLoopRunningUpdatedCallback">Event raised when the loop running status is updated.</param>
        public IEnumerator Run(CommandParameterData parameterData,
                               Action<bool> onLoopRunningUpdatedCallback = null)
        {
            parameterData.Graph = this;

            IsRunning = true;
            yield return RootNode.RunCommandAndContinue(parameterData);
            IsRunning = false;
        }

        /// <summary>
        /// Create a node of the given type.
        /// </summary>
        /// <param name="type">Type of the node.</param>
        /// <param name="position">Position to create the node in.</param>
        /// <returns>The node created.</returns>
        public CommandNode CreateNode(Type type, Vector2 position = default)
        {
            if (Activator.CreateInstance(type) is not CommandNode node) return null;

            node.name = type.Name;
            #if UNITY_EDITOR
            node.GUID = GUID.Generate().ToString();
            #endif
            node.Position = position;
            Nodes.Add(node);

            return node;
        }

        /// <summary>
        /// Delete the given node from the graph.
        /// </summary>
        /// <param name="node">Node to delete.</param>
        public void DeleteNode(CommandNode node) => Nodes.Remove(node);

        /// <summary>
        /// Connect two nodes in the graph.
        /// </summary>
        /// <param name="parent">Origin of the connection.</param>
        /// <param name="child">Receiver of the connection.</param>
        /// <param name="parentIndex">Index of the port in the parent.</param>
        public void ConnectNodes(CommandNode parent, CommandNode child, int parentIndex) =>
            parent.AddChild(child, parentIndex);

        /// <summary>
        /// Remove the connection between two nodes in the graph.
        /// </summary>
        /// <param name="parent">Origin of the connection.</param>
        /// <param name="child">Receiver of the connection.</param>
        /// <param name="parentIndex">Index of the port in the parent.</param>
        public void RemoveConnection(CommandNode parent, CommandNode child, int parentIndex) =>
            parent.RemoveChild(child, parentIndex);

        /// <summary>
        /// Get all the children of a node.
        /// </summary>
        /// <param name="node">The node to check.</param>
        /// <returns>All the children of this node.</returns>
        public List<CommandNode> GetChildren(CommandNode node) => node.GetChildren();
    }
}