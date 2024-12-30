using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph;

namespace Varguiniano.YAPU.Editor.Actors.CommandGraph
{
    /// <summary>
    /// Graph view for the actor command graph.
    /// </summary>
    [UxmlElement]
    public partial class ActorCommandGraphView : GraphView
    {
        /// <summary>
        /// Graph being edited.
        /// </summary>
        public ActorCommandGraph Graph;

        /// <summary>
        /// Event raised when a node is selected.
        /// </summary>
        public Action<CommandNodeView> OnNodeSelected;

        /// <summary>
        /// Event raised when this graph requests recording undo.
        /// </summary>
        public Action<string> OnRequestRecordUndo;

        /// <summary>
        /// Request the editor to mark the component, game object and scene as dirty.
        /// </summary>
        public Action OnRequestMarkItemsDirty;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ActorCommandGraphView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            StyleSheet styleSheet =
                AssetDatabase
                   .LoadAssetAtPath<StyleSheet>("Assets/YAPU/Editor/Actors/CommandGraph/ActorCommandGraphEditor.uss");

            styleSheets.Add(styleSheet);
        }

        /// <summary>
        /// Populate the view with the given graph.
        /// </summary>
        /// <param name="graphToEdit">Graph to use to populate.</param>
        public void PopulateView(ActorCommandGraph graphToEdit)
        {
            Graph = graphToEdit;

            graphViewChanged -= OnGraphViewChanged;

            DeleteElements(graphElements);

            graphViewChanged += OnGraphViewChanged;

            Graph.RootNode ??= Graph.CreateNode(typeof(RootNode));

            foreach (CommandNode node in Graph.Nodes) CreateNodeView(node, false);

            // We need to create the edges after all the nodes are created.
            foreach (CommandNode node in Graph.Nodes)
            {
                CommandNodeView nodeView = GetNodeViewFromNode(node);

                List<CommandNode> children = node.GetIndexedChildren();

                for (int i = 0; i < children.Count; i++)
                {
                    CommandNode child = children[i];

                    if (child == null) continue;

                    CommandNodeView childView = GetNodeViewFromNode(child);

                    Edge edge = nodeView.OutputPorts[i].ConnectTo(childView.InputPorts[0]);
                    AddElement(edge);
                }
            }
        }

        /// <summary>
        /// Contextual menu to create new nodes and work with them.
        /// </summary>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            //base.BuildContextualMenu(evt);

            // Get the mouse position taking into account zoom and pan.
            Vector2 localMousePosition = evt.localMousePosition;
            Matrix4x4 inverseMatrix = contentViewContainer.transform.matrix.inverse;
            Vector2 worldMousePosition = inverseMatrix.MultiplyPoint3x4(localMousePosition);

            // Copied from base method.
            if (evt.target is GraphView or Node or Group or Edge)
            {
                evt.menu.AppendSeparator();

                evt.menu.AppendAction("Delete",
                                      _ => DeleteSelectionCallback(AskUser.DontAskUser),
                                      _ => canDeleteSelection
                                               ? DropdownMenuAction.Status.Normal
                                               : DropdownMenuAction.Status.Disabled);
            }

            foreach (Type type in TypeCache.GetTypesDerivedFrom<CommandNode>()
                                           .Where(type => !type.GetTypeInfo().IsAbstract && type != typeof(RootNode)))
                // ReSharper disable once PossibleNullReferenceException
                evt.menu.AppendAction("Create Command/" + type.Namespace.Split('.').Last() + "/" + type.Name,
                                      _ =>
                                      {
                                          CreateNode(type, worldMousePosition);
                                      });
        }

        /// <summary>
        /// Create a new node of the given type.
        /// </summary>
        /// <param name="type">Type of the node to create.</param>
        /// <param name="position">Position to create the node in.</param>
        private void CreateNode(Type type, Vector2 position = default)
        {
            CommandNode node = Graph.CreateNode(type, position);
            CreateNodeView(node);
        }

        /// <summary>
        /// Create the view for a single node.
        /// </summary>
        /// <param name="node">Node to create the view from.</param>
        /// <param name="registerUndo">Register to undo?</param>
        private void CreateNodeView(CommandNode node, bool registerUndo = true)
        {
            CommandNodeView view = new(node)
                                   {
                                       OnNodeSelected = OnNodeSelected,
                                       OnRequestRecordUndo = OnRequestRecordUndo,
                                       OnRequestMarkItemsDirty = OnRequestMarkItemsDirty
                                   };

            AddElement(view);

            if (registerUndo) OnRequestRecordUndo?.Invoke("Created node " + node.name + ".");

            view.OnNodePortsUpdated = () => PopulateView(Graph);
        }

        /// <summary>
        /// Callback for the graph view changes.
        /// </summary>
        /// <param name="graphViewChange">Changes that happened to the graph view.</param>
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
                foreach (GraphElement element in graphViewChange.elementsToRemove)
                    switch (element)
                    {
                        case CommandNodeView view:
                            Graph.DeleteNode(view.Node);

                            OnRequestRecordUndo?.Invoke("Deleted node " + view.Node.name + ".");

                            break;
                        case Edge edge:
                        {
                            if (edge.output.node is CommandNodeView startNodeView
                             && edge.input.node is CommandNodeView endNodeView)
                            {
                                Graph.RemoveConnection(startNodeView.Node,
                                                       endNodeView.Node,
                                                       startNodeView.OutputPorts
                                                                    .IndexOf(edge.output));

                                OnRequestRecordUndo?.Invoke("Removed connection between "
                                                          + startNodeView.Node.name
                                                          + " and "
                                                          + endNodeView.Node.name
                                                          + ".");
                            }

                            break;
                        }
                    }

            // ReSharper disable once InvertIf
            if (graphViewChange.edgesToCreate != null)
                foreach (Edge edge in graphViewChange.edgesToCreate)
                    if (edge.output.node is CommandNodeView startNodeView
                     && edge.input.node is CommandNodeView endNodeView)
                    {
                        Graph.ConnectNodes(startNodeView.Node,
                                           endNodeView.Node,
                                           startNodeView.OutputPorts.IndexOf(edge.output));

                        OnRequestRecordUndo?.Invoke("Created connection between "
                                                  + startNodeView.Node.name
                                                  + " and "
                                                  + endNodeView.Node.name
                                                  + ".");
                    }

            return graphViewChange;
        }

        /// <summary>
        /// Make sure no weird things are done then connecting ports.
        /// </summary>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) =>
            ports.Where(endPort => endPort.direction != startPort.direction
                                && endPort.node != startPort.node)
                 .ToList();

        /// <summary>
        /// Get the node view currently displaying a node.
        /// </summary>
        /// <param name="node">Node to check.</param>
        /// <returns>Associated view.</returns>
        private CommandNodeView GetNodeViewFromNode(CommandNode node) => GetNodeByGuid(node.GUID) as CommandNodeView;

        /// <summary>
        /// Update the runtime state of the entire graph.
        /// </summary>
        public void UpdateRuntimeState()
        {
            foreach (CommandNodeView nodeView in nodes.Select(node => node as CommandNodeView))
                nodeView?.UpdateRuntimeState();
        }
    }
}