using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;
using WhateverDevs.Core.Runtime.Common;
using Object = UnityEngine.Object;

namespace Varguiniano.YAPU.Editor.MonsterDatabase
{
    /// <summary>
    /// Monster creation helper section that represents an icon.
    /// </summary>
    [Serializable]
    public class MonsterIconSection : Loggable<MonsterIconSection>, ITextureLoader
    {
        /// <summary>
        /// Constructor that receives the helper window reference.
        /// </summary>
        public MonsterIconSection(MonsterCreationHelper helperWindowReference, IconType type)
        {
            helperWindow = helperWindowReference;
            iconType = type;
        }

        /// <summary>
        /// Reference to the helper window.
        /// </summary>
        private MonsterCreationHelper helperWindow;

        /// <summary>
        /// Icon type to use.
        /// </summary>
        private IconType iconType;

        /// <summary>
        /// String version of the icon type.
        /// </summary>
        private string IconName => iconType.ToString();

        /// <summary>
        /// Suffix to use based on the icon type.
        /// </summary>
        private string IconTypeSuffix => MonsterCreationHelper.Configuration.IconSuffixes[iconType];

        /// <summary>
        /// Get the icons folder based on the icon type.
        /// </summary>
        private string IconsPath =>
            iconType switch
            {
                IconType.Normal => MonsterCreationHelper.Configuration.IconsPath,
                IconType.NormalMale => MonsterCreationHelper.Configuration.MaleIconsPath,
                IconType.Shiny => MonsterCreationHelper.Configuration.ShinyIconsPath,
                IconType.ShinyMale => MonsterCreationHelper.Configuration.ShinyMaleIconsPath,
                _ => throw new ArgumentOutOfRangeException()
            };

        /// <summary>
        /// Name of the icon file.
        /// </summary>
        private string IconFileName =>
            helperWindow.DexNumber
          + MonsterCreationHelper.Configuration.FormSuffixes[helperWindow.Form]
          + IconTypeSuffix;

        /// <summary>
        /// Path for the icon.
        /// </summary>
        private string IconPath => IconsPath + "/" + IconFileName + ".png";

        /// <summary>
        /// Reference to the form's icon, based on the icon type.
        /// </summary>
        private Sprite IconReference
        {
            get =>
                iconType switch
                {
                    IconType.Normal => helperWindow.MonsterEntry[helperWindow.Form].Icon,
                    IconType.NormalMale => helperWindow.MonsterEntry[helperWindow.Form].IconMale,
                    IconType.Shiny => helperWindow.MonsterEntry[helperWindow.Form].IconShiny,
                    IconType.ShinyMale => helperWindow.MonsterEntry[helperWindow.Form].IconShinyMale,
                    _ => throw new ArgumentOutOfRangeException()
                };

            set
            {
                switch (iconType)
                {
                    case IconType.Normal:
                        helperWindow.MonsterEntry[helperWindow.Form].Icon = value;
                        break;
                    case IconType.NormalMale:
                        helperWindow.MonsterEntry[helperWindow.Form].IconMale = value;
                        break;
                    case IconType.Shiny:
                        helperWindow.MonsterEntry[helperWindow.Form].IconShiny = value;
                        break;
                    case IconType.ShinyMale:
                        helperWindow.MonsterEntry[helperWindow.Form].IconShinyMale = value;
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }

                EditorUtility.SetDirty(helperWindow.MonsterEntry);
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// Form's icon. 
        /// </summary>
        [FoldoutGroup("@ " + nameof(IconName))]
        [ShowIf("@helperWindow.FormSelected")]
        [ShowInInspector]
        [InlineEditor(InlineEditorModes.LargePreview, Expanded = true)]
        private Sprite Icon
        {
            get => !helperWindow.FormSelected ? null : IconReference;
            set => IconReference = value;
        }

        /// <summary>
        /// Texture to use to load the icon from.
        /// </summary>
        [FoldoutGroup("@ " + nameof(IconName))]
        [ShowIf("@helperWindow.FormSelected && "
              + nameof(Icon)
              + " == null")]
        [PropertyOrder(1)]
        public Texture TextureToUse;

        [FoldoutGroup("@ " + nameof(IconName))]
        [ShowIf("@helperWindow.FormSelected && " + nameof(Icon) + " == null && " + nameof(TextureToUse) + " == null")]
        [Button(ButtonSizes.Large)]
        [PropertyOrder(1)]
        private void AutoFind()
        {
            TextureToUse = AssetDatabase.LoadAssetAtPath<Texture>(IconPath);

            if (TextureToUse != null) return;

            failedToFindTexture = true;

            PotentialTextures = new List<GenericTextureAssetListItem>();

            List<string> potentialFiles = Directory.GetFiles(IconsPath)
                                                   .Where(file => !file.EndsWith(".meta"))
                                                   .Select(file => file.Replace("\\", "/"))
                                                   .ToList();

            List<string> nameMatchingFiles =
                potentialFiles
                   .Where(file => file.ToLowerInvariant().Contains(helperWindow.MonsterName.ToLowerInvariant())
                               || file.Contains(helperWindow.DexNumberWithoutZeros))
                   .ToList();

            if (nameMatchingFiles.Count > 0) potentialFiles = nameMatchingFiles;

            foreach (string file in potentialFiles)
                PotentialTextures.Add(new GenericTextureAssetListItem
                                      {
                                          Texture = AssetDatabase.LoadAssetAtPath<Texture>(file),
                                          TextureLoader = this
                                      });
        }

        /// <summary>
        /// Flag to know if it failed to auto find the texture.
        /// </summary>
        [UsedImplicitly]
        #pragma warning disable CS0414
        private bool failedToFindTexture;
        #pragma warning restore CS0414

        /// <summary>
        /// List of potential textures to load the icon from.
        /// </summary>
        [FoldoutGroup("@ " + nameof(IconName))]
        [PropertyOrder(1)]
        [ShowIf("@helperWindow.FormSelected && "
              + nameof(Icon)
              + " == null && "
              + nameof(failedToFindTexture))]
        [Searchable]
        [TableList(AlwaysExpanded = true, IsReadOnly = true)]
        public List<GenericTextureAssetListItem> PotentialTextures;

        /// <summary>
        /// Load the texture provided by the menu.
        /// </summary>
        public void LoadTexture(Texture newTexture)
        {
            TextureToUse = newTexture;
            failedToFindTexture = false;
        }

        /// <summary>
        /// Does the name of the texture match the convention?
        /// </summary>
        private bool TextureNameMatchesConvention
        {
            get
            {
                if (TextureToUse == null) return true;

                return TextureToUse.name == IconFileName;
            }
        }

        /// <summary>
        /// Fix the name to match the convention.
        /// </summary>
        [FoldoutGroup("@ " + nameof(IconName))]
        [Button(ButtonSizes.Large)]
        [ShowIf("@!" + nameof(TextureNameMatchesConvention))]
        [PropertyOrder(1)]
        private void FixName()
        {
            string texturePath = AssetDatabase.GetAssetPath(TextureToUse);
            string result = AssetDatabase.RenameAsset(texturePath, IconFileName);

            if (!result.IsNullEmptyOrWhiteSpace()) Logger.Debug("Renaming result: " + result + ".");

            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Preset to use for the icon, based on the icon type.
        /// </summary>
        private Preset Preset =>
            iconType switch
            {
                IconType.Normal => MonsterCreationHelper.Configuration.MonsterIconPreset,
                IconType.NormalMale => MonsterCreationHelper.Configuration.MonsterIconPreset,
                IconType.Shiny => MonsterCreationHelper.Configuration.ShinyMonsterIconPreset,
                IconType.ShinyMale => MonsterCreationHelper.Configuration.ShinyMonsterIconPreset,
                _ => throw new ArgumentOutOfRangeException()
            };

        /// <summary>
        /// Apply the icon preset and load the icon.
        /// </summary>
        [FoldoutGroup("@ " + nameof(IconName))]
        [ShowIf("@" + nameof(Icon) + " == null && " + nameof(TextureToUse) + " != null")]
        [Button(ButtonSizes.Large)]
        [PropertyOrder(1)]
        private void ApplyPresetAndLoadIcon()
        {
            TextureImporter importer =
                (TextureImporter) AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(TextureToUse));

            bool result = Preset.ApplyTo(importer);

            if (!result) Logger.Error("Error applying preset to texture.");

            importer.SaveAndReimport();

            EditorUtility.SetDirty(TextureToUse);
            AssetDatabase.SaveAssets();

            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(TextureToUse));

            Icon = assets.OfType<Sprite>().FirstOrDefault();
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
            Icon = null;
            TextureToUse = null;
        }
    }
}