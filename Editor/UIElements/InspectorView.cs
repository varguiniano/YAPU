using System;
using UnityEngine;
using UnityEngine.UIElements;
using Varguiniano.YAPU.Editor.Actors.CommandGraph;
using Object = UnityEngine.Object;

namespace Varguiniano.YAPU.Editor.UIElements
{
    /// <summary>
    /// View for an inspector panel.
    /// </summary>
    [UxmlElement]
    public partial class InspectorView : VisualElement
    {
        /// <summary>
        /// Editor to use to draw the inspector.
        /// </summary>
        private UnityEditor.Editor editor;

        /// <summary>
        /// Request the editor to mark the component, game object and scene as dirty.
        /// </summary>
        public Action OnRequestMarkItemsDirty;

        /// <summary>
        /// Update the inspector view with the new node to inspect.
        /// </summary>
        /// <param name="nodeView">Node to inspect.</param>
        public void UpdateView(CommandNodeView nodeView)
        {
            Clear();

            if (editor != null) Object.DestroyImmediate(editor);

            CommandNodeInspectorWrapper wrapper = ScriptableObject.CreateInstance<CommandNodeInspectorWrapper>();
            wrapper.Node = nodeView.Node;

            editor = UnityEditor.Editor.CreateEditor(wrapper);

            IMGUIContainer container = new(() =>
                                           {
                                               editor.OnInspectorGUI();

                                               // Actual undo recording is handled by the inspector, we only need to mark the items as dirty.
                                               OnRequestMarkItemsDirty?.Invoke();
                                           });

            Add(container);
        }
    }
}