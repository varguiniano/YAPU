using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using WhateverDevs.Core.Runtime.Common;

namespace Varguiniano.YAPU.Editor.MonsterDatabase
{
    /// <summary>
    /// Monster creation helper section that represents a material.
    /// </summary>
    [Serializable]
    public class MonsterMaterialSection : Loggable<MonsterMaterialSection>
    {
        /// <summary>
        /// Constructor that receives the helper window reference.
        /// </summary>
        public MonsterMaterialSection(MonsterCreationHelper helperWindowReference, MaterialType type)
        {
            helperWindow = helperWindowReference;
            materialType = type;
        }

        /// <summary>
        /// Reference to the helper window.
        /// </summary>
        private MonsterCreationHelper helperWindow;

        /// <summary>
        /// Material type to use.
        /// </summary>
        private MaterialType materialType;

        /// <summary>
        /// String version of the material type.
        /// </summary>
        private string MaterialName => materialType.ToString();

        /// <summary>
        /// Texture property ID to use when setting the shader textures.
        /// </summary>
        private static readonly int MainTex = Shader.PropertyToID("_SpriteSheet");

        /// <summary>
        /// Flipbook property ID to use when setting the shader textures.
        /// </summary>
        private static readonly int Flipbook = Shader.PropertyToID("_Flipbook");

        /// <summary>
        /// Suffix to use based on the material type.
        /// </summary>
        private string MaterialTypeSuffix => MonsterCreationHelper.Configuration.TypeSuffixes[materialType];

        /// <summary>
        /// Get the materials folder based on the material type.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private string MaterialsPath =>
            materialType switch
            {
                MaterialType.Front => MonsterCreationHelper.Configuration.FrontMaterialsPath,
                MaterialType.FrontShiny => MonsterCreationHelper.Configuration.FrontShinyMaterialsPath,
                MaterialType.FrontMale => MonsterCreationHelper.Configuration.FrontMaterialsPath,
                MaterialType.FrontShinyMale => MonsterCreationHelper.Configuration.FrontShinyMaterialsPath,
                MaterialType.Back => MonsterCreationHelper.Configuration.BackMaterialsPath,
                MaterialType.BackShiny => MonsterCreationHelper.Configuration.BackShinyMaterialsPath,
                MaterialType.BackMale => MonsterCreationHelper.Configuration.BackMaterialsPath,
                MaterialType.BackShinyMale => MonsterCreationHelper.Configuration.BackShinyMaterialsPath,
                _ => throw new ArgumentOutOfRangeException()
            };

        /// <summary>
        /// Path for the material.
        /// </summary>
        private string MaterialPath =>
            MaterialsPath
          + "/"
          + helperWindow.DexNumber
          + MonsterCreationHelper.Configuration.FormSuffixes[helperWindow.Form]
          + MaterialTypeSuffix
          + ".mat";

        /// <summary>
        /// Reference to the form's material, based on the material type.
        /// </summary>
        private Material MaterialReference
        {
            get =>
                materialType switch
                {
                    MaterialType.Front => helperWindow.MonsterEntry[helperWindow.Form].Front,
                    MaterialType.FrontShiny => helperWindow.MonsterEntry[helperWindow.Form].FrontShiny,
                    MaterialType.FrontMale => helperWindow.MonsterEntry[helperWindow.Form].FrontMale,
                    MaterialType.FrontShinyMale => helperWindow.MonsterEntry[helperWindow.Form].FrontShinyMale,
                    MaterialType.Back => helperWindow.MonsterEntry[helperWindow.Form].Back,
                    MaterialType.BackShiny => helperWindow.MonsterEntry[helperWindow.Form].BackShiny,
                    MaterialType.BackMale => helperWindow.MonsterEntry[helperWindow.Form].BackMale,
                    MaterialType.BackShinyMale => helperWindow.MonsterEntry[helperWindow.Form].BackShinyMale,
                    _ => throw new ArgumentOutOfRangeException()
                };

            set
            {
                switch (materialType)
                {
                    case MaterialType.Front:
                        helperWindow.MonsterEntry[helperWindow.Form].Front = value;
                        break;
                    case MaterialType.FrontShiny:
                        helperWindow.MonsterEntry[helperWindow.Form].FrontShiny = value;
                        break;
                    case MaterialType.FrontMale:
                        helperWindow.MonsterEntry[helperWindow.Form].FrontMale = value;
                        break;
                    case MaterialType.FrontShinyMale:
                        helperWindow.MonsterEntry[helperWindow.Form].FrontShinyMale = value;
                        break;
                    case MaterialType.Back:
                        helperWindow.MonsterEntry[helperWindow.Form].Back = value;
                        break;
                    case MaterialType.BackShiny:
                        helperWindow.MonsterEntry[helperWindow.Form].BackShiny = value;
                        break;
                    case MaterialType.BackMale:
                        helperWindow.MonsterEntry[helperWindow.Form].BackMale = value;
                        break;
                    case MaterialType.BackShinyMale:
                        helperWindow.MonsterEntry[helperWindow.Form].BackShinyMale = value;
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Check which materials exist and assign them to the entry.
        /// </summary>
        private void CheckAndUpdateMaterialReferences()
        {
            MaterialReference = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);

            EditorUtility.SetDirty(helperWindow.MonsterEntry);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Form's material. 
        /// </summary>
        [FoldoutGroup("@ " + nameof(MaterialName))]
        [ShowIf("@helperWindow.FormSelected")]
        [ShowInInspector]
        [InlineEditor(InlineEditorModes.LargePreview, Expanded = true)]
        private Material Material
        {
            get => !helperWindow.FormSelected ? null : MaterialReference;
            set => MaterialReference = value;
        }

        /// <summary>
        /// Create a material if it doesn't exist.
        /// </summary>
        [FoldoutGroup("@ " + nameof(MaterialName))]
        [Button("Create Material", ButtonSizes.Large)]
        [ShowIf("@ helperWindow.FormSelected && " + nameof(Material) + " == null")]
        private void CreateMaterial() => CreateMaterial(MaterialPath);

        /// <summary>
        /// Texture for the front material.
        /// </summary>
        [FoldoutGroup("@ " + nameof(MaterialName))]
        [ShowIf("@ helperWindow.FormSelected && " + nameof(Material) + " != null")]
        [ShowInInspector]
        [InlineEditor(InlineEditorModes.LargePreview)]
        private Texture Texture
        {
            get =>
                !helperWindow.FormSelected || Material == null
                    ? null
                    : MaterialReference.GetTexture(MainTex);
            set
            {
                if (MaterialReference == null) return;

                MaterialReference.SetTexture(MainTex, value);
                EditorUtility.SetDirty(MaterialReference);
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// Path in which the textures are stored.
        /// </summary>
        private string TexturesPath =>
            materialType switch
            {
                MaterialType.Front => MonsterCreationHelper.Configuration.FrontTexturesPath,
                MaterialType.FrontShiny => MonsterCreationHelper.Configuration.FrontShinyTexturesPath,
                MaterialType.FrontMale => MonsterCreationHelper.Configuration.FrontTexturesPath,
                MaterialType.FrontShinyMale => MonsterCreationHelper.Configuration.FrontShinyTexturesPath,
                MaterialType.Back => MonsterCreationHelper.Configuration.BackTexturesPath,
                MaterialType.BackShiny => MonsterCreationHelper.Configuration.BackShinyTexturesPath,
                MaterialType.BackMale => MonsterCreationHelper.Configuration.BackTexturesPath,
                MaterialType.BackShinyMale => MonsterCreationHelper.Configuration.BackShinyTexturesPath,
                _ => throw new ArgumentOutOfRangeException()
            };

        /// <summary>
        /// Name of the texture.
        /// </summary>
        private string TextureName =>
            helperWindow.DexNumber + MonsterCreationHelper.Configuration.FormSuffixes[helperWindow.Form] + MaterialTypeSuffix;

        /// <summary>
        /// Path to the texture.
        /// </summary>
        private string TexturePath => TexturesPath + "/" + TextureName + ".png";

        /// <summary>
        /// Attempt to auto find the texture.
        /// </summary>
        [FoldoutGroup("@ " + nameof(MaterialName))]
        [ShowIf("@helperWindow.FormSelected && "
              + nameof(Material)
              + " != null && "
              + nameof(Texture)
              + " == null &&!"
              + nameof(failedToFindMaterial))]
        [Button(ButtonSizes.Large)]
        private void AutoFindTexture()
        {
            Texture = AssetDatabase.LoadAssetAtPath<Texture>(TexturePath);

            if (Texture != null) return;

            failedToFindMaterial = true;

            PotentialTextures = new List<TextureAssetListItem>();

            List<string> potentialFiles = Directory.GetFiles(TexturesPath)
                                                   .Where(file => !file.EndsWith(".meta"))
                                                   .Select(file => file.Replace("\\", "/"))
                                                   .ToList();

            List<string> nameMatchingFiles =
                potentialFiles.Where(file => file.Contains(helperWindow.MonsterName)
                                          || file.Contains(helperWindow.DexNumberWithoutZeros))
                              .ToList();

            if (nameMatchingFiles.Count > 0) potentialFiles = nameMatchingFiles;

            foreach (string file in potentialFiles)
                PotentialTextures.Add(new TextureAssetListItem
                                      {
                                          Texture = AssetDatabase.LoadAssetAtPath<Texture>(file),
                                          Material = MaterialReference
                                      });
        }

        /// <summary>
        /// Flag to know when auto finding the texture failed.
        /// </summary>
        [UsedImplicitly]
        #pragma warning disable CS0414
        private bool failedToFindMaterial;
        #pragma warning restore CS0414

        /// <summary>
        /// List of potential textures to assign to the material.
        /// </summary>
        [FoldoutGroup("@ " + nameof(MaterialName))]
        [PropertyOrder(1)]
        [ShowIf("@helperWindow.FormSelected && "
              + nameof(Material)
              + " != null && "
              + nameof(Texture)
              + " == null && "
              + nameof(failedToFindMaterial))]
        [Searchable]
        [TableList(AlwaysExpanded = true, IsReadOnly = true)]
        public List<TextureAssetListItem> PotentialTextures;

        /// <summary>
        /// Does the name of the texture match the convention?
        /// </summary>
        private bool TextureNameMatchesConvention
        {
            get
            {
                if (Texture == null) return true;

                return Texture.name == TextureName;
            }
        }

        /// <summary>
        /// Fix the name to match the convention.
        /// </summary>
        [FoldoutGroup("@ " + nameof(MaterialName))]
        [Button(ButtonSizes.Large)]
        [ShowIf("@!" + nameof(TextureNameMatchesConvention))]
        private void FixName()
        {
            string texturePath = AssetDatabase.GetAssetPath(Texture);
            string result = AssetDatabase.RenameAsset(texturePath, TextureName);

            if (!result.IsNullEmptyOrWhiteSpace()) Logger.Debug("Renaming result: " + result + ".");

            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Apply the default preset to the texture.
        /// </summary>
        [FoldoutGroup("@ " + nameof(MaterialName))]
        [Button(ButtonSizes.Large)]
        [PropertySpace]
        [ShowIf("@" + nameof(Texture) + " != null")]
        [InfoBox("You may need to adjust the sprite bounds, check the console.", VisibleIf = nameof(presetApplied))]
        private void ApplyDefaultPresetToTexture()
        {
            TextureImporter importer =
                (TextureImporter) AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(Texture));

            bool result = MonsterCreationHelper.Configuration.MonsterTexturePreset.ApplyTo(importer);

            if (!result) Logger.Error("Error applying preset to texture.");

            ISpriteEditorDataProvider dataProvider =
                new SpriteDataProviderFactories().GetSpriteEditorDataProviderFromObject(importer);

            dataProvider.InitSpriteEditorDataProvider();
            SpriteRect[] rects = dataProvider.GetSpriteRects();

            // Set the first single sprite to use the first frame of the animation.
            Rect rect = rects[0].rect;
            rect.width = Texture.height;
            rect.height = Texture.height;
            rects[0].rect = rect;

            dataProvider.SetSpriteRects(rects);
            dataProvider.Apply();

            importer.SaveAndReimport();

            EditorUtility.SetDirty(Texture);
            AssetDatabase.SaveAssets();

            presetApplied = true;
        }

        /// <summary>
        /// Flag to know if the preset has been applied.
        /// </summary>
        [UsedImplicitly]
        #pragma warning disable CS0414
        private bool presetApplied;
        #pragma warning restore CS0414

        /// <summary>
        /// Frames this flipbook has.
        /// </summary>
        [FoldoutGroup("@ " + nameof(MaterialName))]
        [ShowIf("@helperWindow.FormSelected && " + nameof(Material) + " != null")]
        [ShowInInspector]
        [PropertyOrder(1)]
        [PropertySpace]
        private int NumberOfFrames
        {
            get
            {
                if (Material == null) return 0;
                return (int) Material.GetVector(Flipbook).x;
            }
            set
            {
                Vector4 vector = Material.GetVector(Flipbook);
                vector.x = value;
                Material.SetVector(Flipbook, vector);

                EditorUtility.SetDirty(Material);
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// Auto calculate the number of frames.
        /// </summary>
        [FoldoutGroup("@ " + nameof(MaterialName))]
        [ShowIf("@helperWindow.FormSelected && " + nameof(Material) + " != null")]
        [PropertyOrder(1)]
        [Button(ButtonSizes.Large)]
        private void AutoCalculate() => NumberOfFrames = Texture.width / Texture.height;

        /// <summary>
        /// Create a material with the given path.
        /// </summary>
        private void CreateMaterial(string path)
        {
            AssetDatabase.CreateAsset(new Material(MonsterCreationHelper.Configuration.MaterialShader), path);
            CheckAndUpdateMaterialReferences();
        }

        /// <summary>
        /// Clear the list of potential textures.
        /// </summary>
        public void ClearSearchCache() => PotentialTextures?.Clear();

        /// <summary>
        /// Clear all the sprites.
        /// </summary>
        public void ClearAllSprites()
        {
            Material = null;
            Texture = null;
        }
    }
}