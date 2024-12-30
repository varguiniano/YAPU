using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using WhateverDevs.Core.Runtime.Common;

namespace Varguiniano.YAPU.Editor.MonsterDatabase
{
    /// <summary>
    /// Monster creation helper section that represents the world sprites.
    /// </summary>
    [Serializable]
    public class MonsterWorldSpritesSection : Loggable<MonsterWorldSpritesSection>, ITextureLoader
    {
        /// <summary>
        /// Constructor that receives the helper window reference.
        /// </summary>
        public MonsterWorldSpritesSection(MonsterCreationHelper helperWindowReference, IconType type)
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
                IconType.Normal => MonsterCreationHelper.Configuration.WorldPath,
                IconType.NormalMale => MonsterCreationHelper.Configuration.MaleWorldPath,
                IconType.Shiny => MonsterCreationHelper.Configuration.ShinyWorldPath,
                IconType.ShinyMale => MonsterCreationHelper.Configuration.ShinyMaleWorldPath,
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
        private Sprite IdleDownIconReference
        {
            get =>
                iconType switch
                {
                    IconType.Normal => helperWindow.MonsterEntry[helperWindow.Form].WorldIdleSpriteDown,
                    IconType.NormalMale => helperWindow.MonsterEntry[helperWindow.Form].WorldIdleSpriteDownMale,
                    IconType.Shiny => helperWindow.MonsterEntry[helperWindow.Form].WorldIdleShinySpriteDown,
                    IconType.ShinyMale => helperWindow.MonsterEntry[helperWindow.Form].WorldIdleShinySpriteDownMale,
                    _ => throw new ArgumentOutOfRangeException()
                };

            set
            {
                switch (iconType)
                {
                    case IconType.Normal:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldIdleSpriteDown = value;
                        break;
                    case IconType.NormalMale:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldIdleSpriteDownMale = value;
                        break;
                    case IconType.Shiny:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldIdleShinySpriteDown = value;
                        break;
                    case IconType.ShinyMale:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldIdleShinySpriteDownMale = value;
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }

                EditorUtility.SetDirty(helperWindow.MonsterEntry);
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// Reference to the form's icon, based on the icon type.
        /// </summary>
        private Sprite IdleUpIconReference
        {
            get =>
                iconType switch
                {
                    IconType.Normal => helperWindow.MonsterEntry[helperWindow.Form].WorldIdleSpriteUp,
                    IconType.NormalMale => helperWindow.MonsterEntry[helperWindow.Form].WorldIdleSpriteUpMale,
                    IconType.Shiny => helperWindow.MonsterEntry[helperWindow.Form].WorldIdleShinySpriteUp,
                    IconType.ShinyMale => helperWindow.MonsterEntry[helperWindow.Form].WorldIdleShinySpriteUpMale,
                    _ => throw new ArgumentOutOfRangeException()
                };

            set
            {
                switch (iconType)
                {
                    case IconType.Normal:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldIdleSpriteUp = value;
                        break;
                    case IconType.NormalMale:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldIdleSpriteUpMale = value;
                        break;
                    case IconType.Shiny:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldIdleShinySpriteUp = value;
                        break;
                    case IconType.ShinyMale:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldIdleShinySpriteUpMale = value;
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }

                EditorUtility.SetDirty(helperWindow.MonsterEntry);
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// Reference to the form's icon, based on the icon type.
        /// </summary>
        private Sprite IdleLeftIconReference
        {
            get =>
                iconType switch
                {
                    IconType.Normal => helperWindow.MonsterEntry[helperWindow.Form].WorldIdleSpriteLeft,
                    IconType.NormalMale => helperWindow.MonsterEntry[helperWindow.Form].WorldIdleSpriteLeftMale,
                    IconType.Shiny => helperWindow.MonsterEntry[helperWindow.Form].WorldIdleShinySpriteLeft,
                    IconType.ShinyMale => helperWindow.MonsterEntry[helperWindow.Form].WorldIdleShinySpriteLeftMale,
                    _ => throw new ArgumentOutOfRangeException()
                };

            set
            {
                switch (iconType)
                {
                    case IconType.Normal:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldIdleSpriteLeft = value;
                        break;
                    case IconType.NormalMale:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldIdleSpriteLeftMale = value;
                        break;
                    case IconType.Shiny:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldIdleShinySpriteLeft = value;
                        break;
                    case IconType.ShinyMale:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldIdleShinySpriteLeftMale = value;
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }

                EditorUtility.SetDirty(helperWindow.MonsterEntry);
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// Reference to the form's icon, based on the icon type.
        /// </summary>
        private Sprite IdleRightIconReference
        {
            get =>
                iconType switch
                {
                    IconType.Normal => helperWindow.MonsterEntry[helperWindow.Form].WorldIdleSpriteRight,
                    IconType.NormalMale => helperWindow.MonsterEntry[helperWindow.Form].WorldIdleSpriteRightMale,
                    IconType.Shiny => helperWindow.MonsterEntry[helperWindow.Form].WorldIdleShinySpriteRight,
                    IconType.ShinyMale => helperWindow.MonsterEntry[helperWindow.Form].WorldIdleShinySpriteRightMale,
                    _ => throw new ArgumentOutOfRangeException()
                };

            set
            {
                switch (iconType)
                {
                    case IconType.Normal:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldIdleSpriteRight = value;
                        break;
                    case IconType.NormalMale:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldIdleSpriteRightMale = value;
                        break;
                    case IconType.Shiny:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldIdleShinySpriteRight = value;
                        break;
                    case IconType.ShinyMale:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldIdleShinySpriteRightMale = value;
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }

                EditorUtility.SetDirty(helperWindow.MonsterEntry);
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// Reference to the form's icon, based on the icon type.
        /// </summary>
        private List<Sprite> WalkingDownIconReference
        {
            get =>
                iconType switch
                {
                    IconType.Normal => helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingSpriteDown,
                    IconType.NormalMale => helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingSpriteDownMale,
                    IconType.Shiny => helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingShinySpriteDown,
                    IconType.ShinyMale => helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingShinySpriteDownMale,
                    _ => throw new ArgumentOutOfRangeException()
                };

            set
            {
                switch (iconType)
                {
                    case IconType.Normal:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingSpriteDown = value;
                        break;
                    case IconType.NormalMale:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingSpriteDownMale = value;
                        break;
                    case IconType.Shiny:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingShinySpriteDown = value;
                        break;
                    case IconType.ShinyMale:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingShinySpriteDownMale = value;
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }

                EditorUtility.SetDirty(helperWindow.MonsterEntry);
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// Reference to the form's icon, based on the icon type.
        /// </summary>
        private List<Sprite> WalkingUpIconReference
        {
            get =>
                iconType switch
                {
                    IconType.Normal => helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingSpriteUp,
                    IconType.NormalMale => helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingSpriteUpMale,
                    IconType.Shiny => helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingShinySpriteUp,
                    IconType.ShinyMale => helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingShinySpriteUpMale,
                    _ => throw new ArgumentOutOfRangeException()
                };

            set
            {
                switch (iconType)
                {
                    case IconType.Normal:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingSpriteUp = value;
                        break;
                    case IconType.NormalMale:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingSpriteUpMale = value;
                        break;
                    case IconType.Shiny:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingShinySpriteUp = value;
                        break;
                    case IconType.ShinyMale:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingShinySpriteUpMale = value;
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }

                EditorUtility.SetDirty(helperWindow.MonsterEntry);
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// Reference to the form's icon, based on the icon type.
        /// </summary>
        private List<Sprite> WalkingLeftIconReference
        {
            get =>
                iconType switch
                {
                    IconType.Normal => helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingSpriteLeft,
                    IconType.NormalMale => helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingSpriteLeftMale,
                    IconType.Shiny => helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingShinySpriteLeft,
                    IconType.ShinyMale => helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingShinySpriteLeftMale,
                    _ => throw new ArgumentOutOfRangeException()
                };

            set
            {
                switch (iconType)
                {
                    case IconType.Normal:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingSpriteLeft = value;
                        break;
                    case IconType.NormalMale:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingSpriteLeftMale = value;
                        break;
                    case IconType.Shiny:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingShinySpriteLeft = value;
                        break;
                    case IconType.ShinyMale:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingShinySpriteLeftMale = value;
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }

                EditorUtility.SetDirty(helperWindow.MonsterEntry);
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// Reference to the form's icon, based on the icon type.
        /// </summary>
        private List<Sprite> WalkingRightIconReference
        {
            get =>
                iconType switch
                {
                    IconType.Normal => helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingSpriteRight,
                    IconType.NormalMale => helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingSpriteRightMale,
                    IconType.Shiny => helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingShinySpriteRight,
                    IconType.ShinyMale => helperWindow.MonsterEntry[helperWindow.Form]
                                                      .WorldWalkingShinySpriteRightMale,
                    _ => throw new ArgumentOutOfRangeException()
                };

            set
            {
                switch (iconType)
                {
                    case IconType.Normal:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingSpriteRight = value;
                        break;
                    case IconType.NormalMale:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingSpriteRightMale = value;
                        break;
                    case IconType.Shiny:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingShinySpriteRight = value;
                        break;
                    case IconType.ShinyMale:
                        helperWindow.MonsterEntry[helperWindow.Form].WorldWalkingShinySpriteRightMale = value;
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }

                EditorUtility.SetDirty(helperWindow.MonsterEntry);
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// Form's sprite. 
        /// </summary>
        [FoldoutGroup("@ " + nameof(IconName))]
        [ShowIf("@helperWindow.FormSelected")]
        [ShowInInspector]
        [InlineEditor(InlineEditorModes.LargePreview, Expanded = true)]
        [HorizontalGroup("@ " + nameof(IconName) + "/Idle")]
        private Sprite IdleDown
        {
            get => !helperWindow.FormSelected ? null : IdleDownIconReference;
            set => IdleDownIconReference = value;
        }

        /// <summary>
        /// Form's sprite. 
        /// </summary>
        [FoldoutGroup("@ " + nameof(IconName))]
        [ShowIf("@helperWindow.FormSelected")]
        [ShowInInspector]
        [InlineEditor(InlineEditorModes.LargePreview, Expanded = true)]
        [HorizontalGroup("@ " + nameof(IconName) + "/Idle")]
        private Sprite IdleUp
        {
            get => !helperWindow.FormSelected ? null : IdleUpIconReference;
            set => IdleUpIconReference = value;
        }

        /// <summary>
        /// Form's sprite. 
        /// </summary>
        [FoldoutGroup("@ " + nameof(IconName))]
        [ShowIf("@helperWindow.FormSelected")]
        [ShowInInspector]
        [InlineEditor(InlineEditorModes.LargePreview, Expanded = true)]
        [HorizontalGroup("@ " + nameof(IconName) + "/Idle")]
        private Sprite IdleLeft
        {
            get => !helperWindow.FormSelected ? null : IdleLeftIconReference;
            set => IdleLeftIconReference = value;
        }

        /// <summary>
        /// Form's sprite. 
        /// </summary>
        [FoldoutGroup("@ " + nameof(IconName))]
        [ShowIf("@helperWindow.FormSelected")]
        [ShowInInspector]
        [InlineEditor(InlineEditorModes.LargePreview, Expanded = true)]
        [HorizontalGroup("@ " + nameof(IconName) + "/Idle")]
        private Sprite IdleRight
        {
            get => !helperWindow.FormSelected ? null : IdleRightIconReference;
            set => IdleRightIconReference = value;
        }

        /// <summary>
        /// Form's sprite. 
        /// </summary>
        [FoldoutGroup("@ " + nameof(IconName))]
        [ShowIf("@helperWindow.FormSelected")]
        [ShowInInspector]
        [InlineEditor(InlineEditorModes.LargePreview, Expanded = true)]
        [HorizontalGroup("@ " + nameof(IconName) + "/Walking")]
        private List<Sprite> WalkingDown
        {
            get => !helperWindow.FormSelected ? null : WalkingDownIconReference;
            set => WalkingDownIconReference = value;
        }

        /// <summary>
        /// Form's sprite. 
        /// </summary>
        [FoldoutGroup("@ " + nameof(IconName))]
        [ShowIf("@helperWindow.FormSelected")]
        [ShowInInspector]
        [InlineEditor(InlineEditorModes.LargePreview, Expanded = true)]
        [HorizontalGroup("@ " + nameof(IconName) + "/Walking")]
        private List<Sprite> WalkingUp
        {
            get => !helperWindow.FormSelected ? null : WalkingUpIconReference;
            set => WalkingUpIconReference = value;
        }

        /// <summary>
        /// Form's sprite. 
        /// </summary>
        [FoldoutGroup("@ " + nameof(IconName))]
        [ShowIf("@helperWindow.FormSelected")]
        [ShowInInspector]
        [InlineEditor(InlineEditorModes.LargePreview, Expanded = true)]
        [HorizontalGroup("@ " + nameof(IconName) + "/Walking")]
        private List<Sprite> WalkingLeft
        {
            get => !helperWindow.FormSelected ? null : WalkingLeftIconReference;
            set => WalkingLeftIconReference = value;
        }

        /// <summary>
        /// Form's sprite. 
        /// </summary>
        [FoldoutGroup("@ " + nameof(IconName))]
        [ShowIf("@helperWindow.FormSelected")]
        [ShowInInspector]
        [InlineEditor(InlineEditorModes.LargePreview, Expanded = true)]
        [HorizontalGroup("@ " + nameof(IconName) + "/Walking")]
        private List<Sprite> WalkingRight
        {
            get => !helperWindow.FormSelected ? null : WalkingRightIconReference;
            set => WalkingRightIconReference = value;
        }

        /// <summary>
        /// Is any of the sprites null?
        /// </summary>
        private bool AnySpriteIsNull =>
            IdleDown == null
         || IdleUp == null
         || IdleLeft == null
         || IdleRight == null
         || WalkingDown == null
         || WalkingDown.Count == 0
         || WalkingUp == null
         || WalkingUp.Count == 0
         || WalkingLeft == null
         || WalkingLeft.Count == 0
         || WalkingRight == null
         || WalkingRight.Count == 0;

        /// <summary>
        /// Texture to use to load the icon from.
        /// </summary>
        [FoldoutGroup("@ " + nameof(IconName))]
        [ShowIf("@helperWindow.FormSelected && " + nameof(AnySpriteIsNull))]
        [PropertyOrder(-1)]
        public Texture TextureToUse;

        [FoldoutGroup("@ " + nameof(IconName))]
        [ShowIf("@helperWindow.FormSelected && "
              + nameof(AnySpriteIsNull)
              + " && "
              + nameof(TextureToUse)
              + " == null")]
        [Button(ButtonSizes.Large)]
        [PropertyOrder(-1)]
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
        [PropertyOrder(-1)]
        [ShowIf("@helperWindow.FormSelected && "
              + nameof(AnySpriteIsNull)
              + " && "
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
        [PropertyOrder(-1)]
        private void FixName()
        {
            string texturePath = AssetDatabase.GetAssetPath(TextureToUse);
            string result = AssetDatabase.RenameAsset(texturePath, IconFileName);

            if (!result.IsNullEmptyOrWhiteSpace()) Logger.Debug("Renaming result: " + result + ".");

            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Apply the icon preset and load the icon.
        /// </summary>
        [FoldoutGroup("@ " + nameof(IconName))]
        [ShowIf("@" + nameof(AnySpriteIsNull) + " && " + nameof(TextureToUse) + " != null")]
        [Button(ButtonSizes.Large)]
        [PropertyOrder(-1)]
        private void ApplyPresetAndLoadIcon()
        {
            TextureImporter importer =
                (TextureImporter) AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(TextureToUse));

            bool result = MonsterCreationHelper.Configuration.MonsterWorldSpritesPreset.ApplyTo(importer);

            if (!result) Logger.Error("Error applying preset to texture.");

            importer.SaveAndReimport();

            EditorUtility.SetDirty(TextureToUse);
            AssetDatabase.SaveAssets();

            Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(TextureToUse))
                                            .OfType<Sprite>()
                                            .ToArray();

            IdleDown = sprites[0];
            IdleUp = sprites[12];
            IdleLeft = sprites[4];
            IdleRight = sprites[8];

            WalkingDown = new List<Sprite>
                          {
                              sprites[1],
                              sprites[2]
                          };

            WalkingUp = new List<Sprite>
                        {
                            sprites[13],
                            sprites[14]
                        };

            WalkingLeft = new List<Sprite>
                          {
                              sprites[5],
                              sprites[6]
                          };

            WalkingRight = new List<Sprite>
                           {
                               sprites[9],
                               sprites[10]
                           };
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
            IdleDown = null;
            IdleUp = null;
            IdleLeft = null;
            IdleRight = null;

            WalkingDown = new List<Sprite>();
            WalkingUp = new List<Sprite>();
            WalkingLeft = new List<Sprite>();
            WalkingRight = new List<Sprite>();

            TextureToUse = null;
        }
    }
}