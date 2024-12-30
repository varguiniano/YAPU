using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Varguiniano.YAPU.Editor.Shaders
{
    /// <summary>
    /// Editor for the Spinda animated flipbook sprite shader.
    /// </summary>
    [UsedImplicitly]
    public class SpindaShaderEditor : AnimatedFlipbookSpriteEditor
    {
        /// <summary>
        /// Is the spinda section open?
        /// </summary>
        private bool spindaSectionOpen;

        /// <summary>
        /// Paint the inspector.
        /// </summary>
        protected override void PaintUI()
        {
            base.PaintUI();

            EditorGUILayout.BeginVertical(new GUIStyle("box"));

            spindaSectionOpen = EditorGUILayout.Foldout(spindaSectionOpen, "Spinda");

            if (spindaSectionOpen)
            {
                PaintProperty("_Spot1");
                PaintProperty("_Spot2");
                PaintProperty("_Spot3");
                PaintProperty("_Spot4");

                PaintProperty("_Spot1Coords");
                PaintProperty("_Spot2Coords");
                PaintProperty("_Spot3Coords");
                PaintProperty("_Spot4Coords");
                
                PaintProperty("_SpotPositions");
            }

            EditorGUILayout.EndVertical();
        }
    }
}