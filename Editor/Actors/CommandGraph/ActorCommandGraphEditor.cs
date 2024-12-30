using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using Varguiniano.YAPU.Editor.UIElements;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph;

namespace Varguiniano.YAPU.Editor.Actors.CommandGraph
{
    /// <summary>
    /// Editor for the actor command graph.
    /// </summary>
    public class ActorCommandGraphEditor : EditorWindow
    {
        /// <summary>
        /// Game object being edited.
        /// </summary>
        private GameObject gameObject;

        /// <summary>
        /// Component that holds this graph.
        /// </summary>
        private MonoBehaviour component;

        /// <summary>
        /// Reference to the graph view.
        /// </summary>
        private ActorCommandGraphView graphView;

        /// <summary>
        /// Reference to the inspector view.
        /// </summary>
        private InspectorView inspectorView;

        /// <summary>
        /// Graph to edit.
        /// </summary>
        private ActorCommandGraph graph;

        /// <summary>
        /// Reference to the visual tree asset being edited.
        /// </summary>
        [SerializeField]
        // ReSharper disable once InconsistentNaming
        // It has to have that name or else it will throw an error.
        private VisualTreeAsset m_VisualTreeAsset;

        /// <summary>
        /// Open the window.
        /// </summary>
        public static void OpenWindow(MonoBehaviour componentToEdit, ActorCommandGraph graphToEdit)
        {
            ActorCommandGraphEditor window = GetWindow<ActorCommandGraphEditor>();
            window.gameObject = componentToEdit.gameObject;
            window.component = componentToEdit;
            window.graph = graphToEdit;
            window.titleContent = new GUIContent(window.gameObject.name);
            window.PopulateView();
        }

        /// <summary>
        /// Build the graph UI.
        /// </summary>
        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            m_VisualTreeAsset.CloneTree(root);

            graphView = root.Q<ActorCommandGraphView>();
            graphView.focusable = true;
            graphView.OnRequestRecordUndo = OnGraphRequestedRecordUndo;
            graphView.OnRequestMarkItemsDirty = MarkItemsDirtyForEditor;

            root.Q<Button>("SaveButton").clicked += OnSaveButtonClicked;

            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
            Undo.undoRedoPerformed += OnUndoRedoPerformed;
        }

        /// <summary>
        /// Called each editor frame.
        /// </summary>
        private void OnInspectorUpdate() => UpdateRuntimeState();

        /// <summary>
        /// Populate the view.
        /// </summary>
        private void PopulateView() => graphView.PopulateView(graph);

        /// <summary>
        /// Make sure the editor marks the component, object and scenes as dirty.
        /// </summary>
        private void MarkItemsDirtyForEditor()
        {
            if (Application.isPlaying) return;

            EditorUtility.SetDirty(component);
            EditorUtility.SetDirty(gameObject);
            EditorSceneManager.MarkAllScenesDirty();
        }

        /// <summary>
        /// Called when the graph requests recording undo.
        /// </summary>
        /// <param name="action">Action taken.</param>
        private void OnGraphRequestedRecordUndo(string action)
        {
            if (Application.isPlaying) return;

            MarkItemsDirtyForEditor();
            Undo.RegisterCompleteObjectUndo(gameObject, action);
        }

        /// <summary>
        /// Called each time the player performs an undo or redo.
        /// </summary>
        private void OnUndoRedoPerformed() => PopulateView();

        /// <summary>
        /// Called when the save button is clicked.
        /// </summary>
        private void OnSaveButtonClicked()
        {
            if (Application.isPlaying) return;

            MarkItemsDirtyForEditor();

            AssetDatabase.SaveAssetIfDirty(gameObject);
            AssetDatabase.SaveAssets();
            EditorSceneManager.SaveOpenScenes();
        }

        /// <summary>
        /// Update the runtime state of the entire graph if playing.
        /// </summary>
        private void UpdateRuntimeState()
        {
            if (!Application.isPlaying) return;

            graphView.UpdateRuntimeState();
        }
    }
}