using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

namespace Varguiniano.YAPU.Editor.Shaders
{
    /// <summary>
    /// Tool to generate textures to pass to the Spinda shader as data.
    /// </summary>
    public class SpindaDataGenerator : OdinEditorWindow
    {
        /// <summary>
        /// Cached shader properties.
        /// </summary>
        private static readonly int Spot1Position = Shader.PropertyToID("_Spot1Coords");

        /// <summary>
        /// Cached shader properties.
        /// </summary>
        private static readonly int Spot2Position = Shader.PropertyToID("_Spot2Coords");

        /// <summary>
        /// Cached shader properties.
        /// </summary>
        private static readonly int Spot3Position = Shader.PropertyToID("_Spot3Coords");

        /// <summary>
        /// Cached shader properties.
        /// </summary>
        private static readonly int Spot4Position = Shader.PropertyToID("_Spot4Coords");

        /// <summary>
        /// Cached shader properties.
        /// </summary>
        private static readonly int AutoAnimateShaderProperty = Shader.PropertyToID("_AutoAnimate");

        /// <summary>
        /// Cached shader properties.
        /// </summary>
        private static readonly int Flipbook = Shader.PropertyToID("_Flipbook");

        /// <summary>
        /// Cached shader properties.
        /// </summary>
        private static readonly int SpriteIndex = Shader.PropertyToID("_SpriteIndex");

        /// <summary>
        /// Open the window.
        /// </summary>
        [MenuItem("YAPU/Spinda data generator")]
        private static void OpenWindow() => GetWindow<SpindaDataGenerator>("Spinda Spot Visualizer").Show();

        /// <summary>
        /// Repaint the UI continuously to allow previews to move.
        /// </summary>
        private void Update() => Repaint();

        /// <summary>
        /// Reference to the material being visualized.
        /// </summary>
        [HorizontalGroup("Spinda editor")]
        [SerializeField]
        [InlineEditor(InlineEditorModes.LargePreview, Expanded = true, PreviewHeight = 1000)]
        [PropertyOrder(-100)]
        private Material SpindaMaterial;

        /// <summary>
        /// Asset to hold the data texture.
        /// </summary>
        [HorizontalGroup("Spinda editor")]
        [VerticalGroup("Spinda editor/Info")]
        [SerializeField]
        [OnValueChanged(nameof(LoadPositionsFromTexture))]
        [PropertyOrder(-2)]
        private Texture2D DataTexture;

        /// <summary>
        /// Flag to know is the editor is ready to be used.
        /// </summary>
        private bool EditorReady => SpindaMaterial != null && DataTexture != null;

        /// <summary>
        /// Auto animate the material?
        /// </summary>
        [HorizontalGroup("Spinda editor")]
        [VerticalGroup("Spinda editor/Info")]
        [FoldoutGroup("Spinda editor/Info/Animation")]
        [ShowIf(nameof(EditorReady))]
        [PropertyOrder(-1)]
        [ShowInInspector]
        private bool AutoAnimate
        {
            get
            {
                if (!EditorReady) return false;
                return SpindaMaterial.GetFloat(AutoAnimateShaderProperty) >= 1;
            }
            set => SpindaMaterial.SetFloat(AutoAnimateShaderProperty, value ? 1 : 0);
        }

        [HorizontalGroup("Spinda editor")]
        [VerticalGroup("Spinda editor/Info")]
        [FoldoutGroup("Spinda editor/Info/Animation")]
        [ShowIf("@" + nameof(EditorReady) + " && !" + nameof(AutoAnimate))]
        [ShowInInspector]
        [PropertyRange(0, nameof(MaxSpriteIndex))]
        [PropertyOrder(-1)]
        private int CurrentSpriteIndex
        {
            get => !EditorReady ? 0 : Mathf.RoundToInt(SpindaMaterial.GetFloat(SpriteIndex));
            set => SpindaMaterial.SetFloat(SpriteIndex, value);
        }

        /// <summary>
        /// Max index the flipbook has.
        /// </summary>
        private int MaxSpriteIndex
        {
            get
            {
                if (!EditorReady) return 0;

                Vector4 flipbookInfo = SpindaMaterial.GetVector(Flipbook);
                return (int) (flipbookInfo.x * flipbookInfo.y) - 1;
            }
        }

        /// <summary>
        /// Anchors for spot 1 on the current frame.
        /// </summary>
        [HorizontalGroup("Spinda editor")]
        [VerticalGroup("Spinda editor/Info")]
        [FoldoutGroup("Spinda editor/Info/Animation")]
        [Header("Current frame anchors")]
        [ShowIf("@" + nameof(EditorReady) + " && !" + nameof(AutoAnimate))]
        [ShowInInspector]
        [PropertyOrder(-1)]
        private Vector2Int Spot1Anchors
        {
            get => !EditorReady ? Vector2Int.zero : SpindaLeftEarPositions[CurrentSpriteIndex];
            set => SpindaLeftEarPositions[CurrentSpriteIndex] = value;
        }

        /// <summary>
        /// Anchors for spot 2 on the current frame.
        /// </summary>
        [HorizontalGroup("Spinda editor")]
        [VerticalGroup("Spinda editor/Info")]
        [FoldoutGroup("Spinda editor/Info/Animation")]
        [ShowIf("@" + nameof(EditorReady) + " && !" + nameof(AutoAnimate))]
        [ShowInInspector]
        [PropertyOrder(-1)]
        private Vector2Int Spot2Anchors
        {
            get => !EditorReady ? Vector2Int.zero : SpindaRightEarPositions[CurrentSpriteIndex];
            set => SpindaRightEarPositions[CurrentSpriteIndex] = value;
        }

        /// <summary>
        /// Anchors for spot 3 on the current frame.
        /// </summary>
        [HorizontalGroup("Spinda editor")]
        [VerticalGroup("Spinda editor/Info")]
        [FoldoutGroup("Spinda editor/Info/Animation")]
        [ShowIf("@" + nameof(EditorReady) + " && !" + nameof(AutoAnimate))]
        [ShowInInspector]
        [PropertyOrder(-1)]
        private Vector2Int Spot3Anchors
        {
            get => !EditorReady ? Vector2Int.zero : SpindaLeftFacePositions[CurrentSpriteIndex];
            set => SpindaLeftFacePositions[CurrentSpriteIndex] = value;
        }

        /// <summary>
        /// Anchors for spot 4 on the current frame.
        /// </summary>
        [HorizontalGroup("Spinda editor")]
        [VerticalGroup("Spinda editor/Info")]
        [FoldoutGroup("Spinda editor/Info/Animation")]
        [ShowIf("@" + nameof(EditorReady) + " && !" + nameof(AutoAnimate))]
        [ShowInInspector]
        [PropertyOrder(-1)]
        private Vector2Int Spot4Anchors
        {
            get => !EditorReady ? Vector2Int.zero : SpindaRightFacePositions[CurrentSpriteIndex];
            set => SpindaRightFacePositions[CurrentSpriteIndex] = value;
        }

        /// <summary>
        /// Save the relative positions for this frame.
        /// </summary>
        [HorizontalGroup("Spinda editor")]
        [VerticalGroup("Spinda editor/Info")]
        [FoldoutGroup("Spinda editor/Info/Animation")]
        [ShowIf("@" + nameof(EditorReady) + " && !" + nameof(AutoAnimate))]
        [Button]
        [PropertyOrder(-1)]
        private void Save() => ExportPositionsToTexture();

        /// <summary>
        /// Copy the relative positions from the previous frame.
        /// </summary>
        [HorizontalGroup("Spinda editor")]
        [VerticalGroup("Spinda editor/Info")]
        [FoldoutGroup("Spinda editor/Info/Animation")]
        [ShowIf("@"
              + nameof(EditorReady)
              + " && !"
              + nameof(AutoAnimate)
              + " && "
              + nameof(CurrentSpriteIndex)
              + " > 0")]
        [Button]
        [PropertyOrder(-1)]
        private void CopyFromPreviousFrame()
        {
            Spot1Anchors = SpindaLeftEarPositions[CurrentSpriteIndex - 1];
            Spot2Anchors = SpindaRightEarPositions[CurrentSpriteIndex - 1];
            Spot3Anchors = SpindaLeftFacePositions[CurrentSpriteIndex - 1];
            Spot4Anchors = SpindaRightFacePositions[CurrentSpriteIndex - 1];

            Save();
        }

        /// <summary>
        /// Relative position to be used by spot 1.
        /// </summary>
        [HorizontalGroup("Spinda editor")]
        [VerticalGroup("Spinda editor/Info")]
        [FoldoutGroup("Spinda editor/Info/Personality")]
        [ShowIf(nameof(EditorReady))]
        [SerializeField]
        private Vector2Int Spot1RelativePosition;

        /// <summary>
        /// Relative position to be used by spot 2.
        /// </summary>
        [HorizontalGroup("Spinda editor")]
        [VerticalGroup("Spinda editor/Info")]
        [FoldoutGroup("Spinda editor/Info/Personality")]
        [ShowIf(nameof(EditorReady))]
        [SerializeField]
        private Vector2Int Spot2RelativePosition;

        /// <summary>
        /// Relative position to be used by spot 3.
        /// </summary>
        [HorizontalGroup("Spinda editor")]
        [VerticalGroup("Spinda editor/Info")]
        [FoldoutGroup("Spinda editor/Info/Personality")]
        [ShowIf(nameof(EditorReady))]
        [SerializeField]
        private Vector2Int Spot3RelativePosition;

        /// <summary>
        /// Relative position to be used by spot 4.
        /// </summary>
        [HorizontalGroup("Spinda editor")]
        [VerticalGroup("Spinda editor/Info")]
        [FoldoutGroup("Spinda editor/Info/Personality")]
        [ShowIf(nameof(EditorReady))]
        [SerializeField]
        private Vector2Int Spot4RelativePosition;

        /// <summary>
        /// Center all relative positions
        /// </summary>
        [HorizontalGroup("Spinda editor")]
        [VerticalGroup("Spinda editor/Info")]
        [FoldoutGroup("Spinda editor/Info/Personality")]
        [ShowIf(nameof(EditorReady))]
        [Button("Center")]
        private void CenterRelativePositions()
        {
            Spot1RelativePosition = new Vector2Int(8, 8);
            Spot2RelativePosition = new Vector2Int(8, 8);
            Spot3RelativePosition = new Vector2Int(8, 8);
            Spot4RelativePosition = new Vector2Int(8, 8);

            SaveRelativePositions();
        }

        /// <summary>
        /// Generate random relative positions and save them.
        /// </summary>
        [HorizontalGroup("Spinda editor")]
        [VerticalGroup("Spinda editor/Info")]
        [FoldoutGroup("Spinda editor/Info/Personality")]
        [ShowIf(nameof(EditorReady))]
        [Button("Random")]
        private void CreateRandomRelativePositions()
        {
            Spot1RelativePosition = new Vector2Int(Random.Range(0, 16), Random.Range(0, 16));
            Spot2RelativePosition = new Vector2Int(Random.Range(0, 16), Random.Range(0, 16));
            Spot3RelativePosition = new Vector2Int(Random.Range(0, 16), Random.Range(0, 16));
            Spot4RelativePosition = new Vector2Int(Random.Range(0, 16), Random.Range(0, 16));

            SaveRelativePositions();
        }

        /// <summary>
        /// Save the relative positions to the material.
        /// </summary>
        [HorizontalGroup("Spinda editor")]
        [VerticalGroup("Spinda editor/Info")]
        [FoldoutGroup("Spinda editor/Info/Personality")]
        [ShowIf(nameof(EditorReady))]
        [Button]
        private void SaveRelativePositions()
        {
            SpindaMaterial.SetVector(Spot1Position,
                                     new Vector2(Spot1RelativePosition.x, Spot1RelativePosition.y));

            SpindaMaterial.SetVector(Spot2Position,
                                     new Vector2(Spot2RelativePosition.x, Spot2RelativePosition.y));

            SpindaMaterial.SetVector(Spot3Position,
                                     new Vector2(Spot3RelativePosition.x, Spot3RelativePosition.y));

            SpindaMaterial.SetVector(Spot4Position,
                                     new Vector2(Spot4RelativePosition.x, Spot4RelativePosition.y));
        }

        /// <summary>
        /// Bounds of Spinda's left ear across all sprites in the animation.
        /// </summary>
        [HorizontalGroup("Spinda editor")]
        [VerticalGroup("Spinda editor/Info")]
        [FoldoutGroup("Spinda editor/Info/Spot Anchors")]
        [ShowIf(nameof(EditorReady))]
        [SerializeField]
        private List<Vector2Int> SpindaLeftEarPositions = new();

        /// <summary>
        /// Bounds of Spinda's right ear across all sprites in the animation.
        /// </summary>
        [HorizontalGroup("Spinda editor")]
        [VerticalGroup("Spinda editor/Info")]
        [FoldoutGroup("Spinda editor/Info/Spot Anchors")]
        [ShowIf(nameof(EditorReady))]
        [SerializeField]
        private List<Vector2Int> SpindaRightEarPositions = new();

        /// <summary>
        /// Bounds of Spinda's left face across all sprites in the animation.
        /// </summary>
        [HorizontalGroup("Spinda editor")]
        [VerticalGroup("Spinda editor/Info")]
        [FoldoutGroup("Spinda editor/Info/Spot Anchors")]
        [ShowIf(nameof(EditorReady))]
        [SerializeField]
        private List<Vector2Int> SpindaLeftFacePositions = new();

        /// <summary>
        /// Bounds of Spinda's right face across all sprites in the animation.
        /// </summary>
        [HorizontalGroup("Spinda editor")]
        [VerticalGroup("Spinda editor/Info")]
        [FoldoutGroup("Spinda editor/Info/Spot Anchors")]
        [ShowIf(nameof(EditorReady))]
        [SerializeField]
        private List<Vector2Int> SpindaRightFacePositions = new();

        /// <summary>
        /// Load the data from the texture.
        /// </summary>
        private void LoadPositionsFromTexture()
        {
            if (DataTexture == null) return;

            SpindaLeftEarPositions.Clear();
            SpindaRightEarPositions.Clear();
            SpindaLeftFacePositions.Clear();
            SpindaRightFacePositions.Clear();

            int i = 0;

            foreach (Color pixel in DataTexture.GetPixels())
            {
                switch (i)
                {
                    case < 52:
                        SpindaLeftEarPositions.Add(new Vector2Int(Mathf.RoundToInt(pixel.r * 51),
                                                                  Mathf.RoundToInt(pixel.g * 51)));

                        break;
                    case < 104:
                        SpindaRightEarPositions.Add(new Vector2Int(Mathf.RoundToInt(pixel.r * 51),
                                                                   Mathf.RoundToInt(pixel.g * 51)));

                        break;
                    case < 156:
                        SpindaLeftFacePositions.Add(new Vector2Int(Mathf.RoundToInt(pixel.r * 51),
                                                                   Mathf.RoundToInt(pixel.g * 51)));

                        break;
                    default:
                        SpindaRightFacePositions.Add(new Vector2Int(Mathf.RoundToInt(pixel.r * 51),
                                                                    Mathf.RoundToInt(pixel.g * 51)));

                        break;
                }

                i++;
            }
        }

        /// <summary>
        /// Export the data to texture.
        /// </summary>
        [HorizontalGroup("Spinda editor")]
        [VerticalGroup("Spinda editor/Info")]
        [FoldoutGroup("Spinda editor/Info/Spot Anchors")]
        [Button]
        [ShowIf(nameof(EditorReady))]
        private void ExportPositionsToTexture()
        {
            string dataTexturePath = AssetDatabase.GetAssetPath(DataTexture);
            DataTexture = new Texture2D(52, 4);

            for (int i = 0; i < SpindaLeftEarPositions.Count; i++)
            {
                Vector2 datum = SpindaLeftEarPositions[i];

                DataTexture.SetPixel(i, 0, new Color(datum.x / 51, datum.y / 51, 0, 1));
            }

            for (int i = 0; i < SpindaRightEarPositions.Count; i++)
            {
                Vector2 datum = SpindaRightEarPositions[i];

                DataTexture.SetPixel(i, 1, new Color(datum.x / 51, datum.y / 51, 0, 1));
            }

            for (int i = 0; i < SpindaLeftFacePositions.Count; i++)
            {
                Vector2 datum = SpindaLeftFacePositions[i];

                DataTexture.SetPixel(i, 2, new Color(datum.x / 51, datum.y / 51, 0, 1));
            }

            for (int i = 0; i < SpindaRightFacePositions.Count; i++)
            {
                Vector2 datum = SpindaRightFacePositions[i];

                DataTexture.SetPixel(i, 3, new Color(datum.x / 51, datum.y / 51, 0, 1));
            }

            DataTexture.Apply();

            File.WriteAllBytes(dataTexturePath, DataTexture.EncodeToPNG());

            DataTexture = null;

            AssetDatabase.Refresh();

            DataTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(dataTexturePath);
        }

        /// <summary>
        /// Export the data to texture.
        /// </summary>
        [HorizontalGroup("Spinda editor")]
        [VerticalGroup("Spinda editor/Info")]
        [FoldoutGroup("Spinda editor/Info/Spot Anchors")]
        [Button]
        [ShowIf(nameof(EditorReady))]
        private void ResetAllPositions()
        {
            SpindaLeftEarPositions.Clear();
            SpindaRightEarPositions.Clear();
            SpindaLeftFacePositions.Clear();
            SpindaRightFacePositions.Clear();

            for (int i = 0; i < 52; i++)
            {
                SpindaLeftEarPositions.Add(Vector2Int.zero);
                SpindaRightEarPositions.Add(Vector2Int.zero);
                SpindaLeftFacePositions.Add(Vector2Int.zero);
                SpindaRightFacePositions.Add(Vector2Int.zero);
            }

            ExportPositionsToTexture();
        }
    }
}