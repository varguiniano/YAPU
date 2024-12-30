using System.Reflection;
using JetBrains.Annotations;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph;

namespace Varguiniano.YAPU.Editor.Actors.CommandGraph
{
    /// <summary>
    /// Drawer for the actor command graph.
    /// It allows creating a new one and opening the editor for an existing one.
    /// </summary>
    [UsedImplicitly]
    public class ActorCommandGraphDrawer : OdinValueDrawer<ActorCommandGraph>
    {
        /// <summary>
        /// Component that holds this graph.
        /// </summary>
        private MonoBehaviour component;

        /// <summary>
        /// Paint the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            ActorCommandGraph graph = ValueEntry.SmartValue;

            EditorGUILayout.BeginHorizontal();

            {
                if (graph != null)
                {
                    if (GUILayout.Button("Edit graph"))
                    {
                        if (component == null) component = FindComponentThatHoldsGraph(graph);

                        ActorCommandGraphEditor.OpenWindow(component, graph);
                    }

                    if (GUILayout.Button("Delete graph"))
                        if (EditorUtility.DisplayDialog("Actor command graph",
                                                        "Do you really want to delete this graph? You won't be able to undo this operation.",
                                                        "Yes",
                                                        "No"))
                            ValueEntry.SmartValue = null;
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Find the component that is holding the graph in the scene.
        /// </summary>
        /// <param name="targetGraph">Graph being held by the component.</param>
        /// <returns>Component holding the graph.</returns>
        private static MonoBehaviour FindComponentThatHoldsGraph(ActorCommandGraph targetGraph)
        {
            MonoBehaviour component = null;

            foreach (GameObject obj in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
            {
                component = FindComponentThatHoldsGraph(targetGraph, obj);

                if (component != null) return component;
            }

            foreach (string prefabGuid in AssetDatabase.FindAssets("t:prefab"))
            {
                string path = AssetDatabase.GUIDToAssetPath(prefabGuid);
                component = FindComponentThatHoldsGraph(targetGraph, AssetDatabase.LoadAssetAtPath<GameObject>(path));

                if (component != null) return component;
            }

            return component;
        }

        /// <summary>
        /// Find the component that is holding the graph in the scene.
        /// </summary>
        /// <param name="targetGraph">Graph being held by the component.</param>
        /// <param name="candidate">Candidate to having the graph.</param>
        /// <returns>Component holding the graph.</returns>
        private static MonoBehaviour FindComponentThatHoldsGraph(ActorCommandGraph targetGraph, GameObject candidate)
        {
            MonoBehaviour[] components = candidate.GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour component in components)
            {
                FieldInfo[] fields = component.GetType()
                                              .GetFields(BindingFlags.Public
                                                       | BindingFlags.NonPublic
                                                       | BindingFlags.Instance);

                foreach (FieldInfo field in fields)
                {
                    if (field.FieldType != typeof(ActorCommandGraph)) continue;
                    ActorCommandGraph graph = field.GetValue(component) as ActorCommandGraph;
                    if (graph == targetGraph) return component;
                }
            }

            return null;
        }
    }
}