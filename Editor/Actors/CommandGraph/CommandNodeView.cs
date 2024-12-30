using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Varguiniano.YAPU.Editor.UIElements;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph;

namespace Varguiniano.YAPU.Editor.Actors.CommandGraph
{
    /// <summary>
    /// View for the command nodes.
    /// </summary>
    public class CommandNodeView : Node
    {
        /// <summary>
        /// Node being edited.
        /// </summary>
        public readonly CommandNode Node;

        /// <summary>
        /// Input ports of the node.
        /// </summary>
        public readonly List<Port> InputPorts = new();

        /// <summary>
        /// Output ports of the node.
        /// </summary>
        public readonly List<Port> OutputPorts = new();

        /// <summary>
        /// Event raised when the node is selected.
        /// </summary>
        public Action<CommandNodeView> OnNodeSelected;

        /// <summary>
        /// Event raised when this node requests recording undo.
        /// </summary>
        public Action<string> OnRequestRecordUndo;

        /// <summary>
        /// Request the editor to mark the component, game object and scene as dirty.
        /// </summary>
        public Action OnRequestMarkItemsDirty;

        /// <summary>
        /// Raised when the node ports are updated.
        /// </summary>
        public Action OnNodePortsUpdated;

        /// <summary>
        /// Constructor receiving the node to edit.
        /// </summary>
        public CommandNodeView(CommandNode node) :
            base("Assets/YAPU/Editor/Actors/CommandGraph/CommandNodeView.uxml")
        {
            Node = node;
            viewDataKey = node.GUID;

            ApplyStyling();

            CreateInputPorts();
            CreateOutputPorts();

            Node.OnPortsUpdated = OnPortsUpdated;

            InspectorView inspector = this.Q<InspectorView>();
            inspector.UpdateView(this);
            inspector.OnRequestMarkItemsDirty = () => OnRequestMarkItemsDirty?.Invoke();
        }

        /// <summary>
        /// Save the new position.
        /// </summary>
        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Node.Position.x = newPos.xMin;
            Node.Position.y = newPos.yMin;
            OnRequestRecordUndo?.Invoke("Change node " + Node.name + " position.");
        }

        /// <summary>
        /// Create the input ports of this node.
        /// </summary>
        private void CreateInputPorts()
        {
            inputContainer.Clear();
            InputPorts.Clear();

            foreach (string portName in Node.GetInputPorts())
            {
                Port port = InstantiatePort(Orientation.Horizontal,
                                            Direction.Input,
                                            Port.Capacity.Multi,
                                            typeof(bool));

                port.portName = portName;
                inputContainer.Add(port);
                InputPorts.Add(port);
            }
        }

        /// <summary>
        /// Create the output ports of this node.
        /// </summary>
        private void CreateOutputPorts()
        {
            outputContainer.Clear();
            OutputPorts.Clear();

            foreach (string portName in Node.GetOutputPorts())
            {
                Port port = InstantiatePort(Orientation.Horizontal,
                                            Direction.Output,
                                            Port.Capacity.Single,
                                            typeof(bool));

                port.portName = portName;
                outputContainer.Add(port);
                OutputPorts.Add(port);
            }
        }

        /// <summary>
        /// Called when the ports are updated.
        /// </summary>
        private void OnPortsUpdated()
        {
            CreateInputPorts();
            CreateOutputPorts();
            OnNodePortsUpdated?.Invoke();
        }

        /// <summary>
        /// Called each time the node is selected.
        /// </summary>
        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(this);
        }

        /// <summary>
        /// Apply the styling to the node based on its type.
        /// </summary>
        private void ApplyStyling()
        {
            title = Node.name;
            style.left = Node.Position.x;
            style.top = Node.Position.y;

            // ReSharper disable once PossibleNullReferenceException
            AddToClassList(Node.GetType().Namespace.Split('.')[^1].ToLower());
        }

        /// <summary>
        /// Update the state of the node view based on the state of the node.
        /// </summary>
        public void UpdateRuntimeState()
        {
            RemoveFromClassList("running");
            
            if (Node.IsRunning)
                AddToClassList("running");
        }
    }
}