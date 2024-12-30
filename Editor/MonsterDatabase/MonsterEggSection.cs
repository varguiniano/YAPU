using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using WhateverDevs.Core.Runtime.Common;

namespace Varguiniano.YAPU.Editor.MonsterDatabase
{
    /// <summary>
    /// Section of the Monster creation helper that handles the egg graphics.
    /// </summary>
    [Serializable]
    public class MonsterEggSection : Loggable<MonsterEggSection>
    {
        /// <summary>
        /// Constructor that receives the helper window.
        /// </summary>
        public MonsterEggSection(MonsterCreationHelper helperWindowReference) => helperWindow = helperWindowReference;

        /// <summary>
        /// Reference to the helper window.
        /// </summary>
        private MonsterCreationHelper helperWindow;

        /// <summary>
        /// Form's material. 
        /// </summary>
        [ShowIf("@helperWindow.FormSelected")]
        [ShowInInspector]
        [InlineEditor(InlineEditorModes.LargePreview, Expanded = true)]
        private Material Material
        {
            get => !helperWindow.FormSelected ? null : helperWindow.MonsterEntry[helperWindow.Form].EggMaterial;
            set
            {
                helperWindow.MonsterEntry[helperWindow.Form].EggMaterial = value;
                EditorUtility.SetDirty(helperWindow.MonsterEntry);
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// Use the default egg material?
        /// </summary>
        [ShowIf("@helperWindow.FormSelected")]
        [Button(ButtonSizes.Large)]
        public void UseDefaultMaterial() => Material = MonsterCreationHelper.Configuration.DefaultEggMaterial;

        /// <summary>
        /// Form's Icon. 
        /// </summary>
        [ShowIf("@helperWindow.FormSelected")]
        [ShowInInspector]
        [InlineEditor(InlineEditorModes.LargePreview, Expanded = true)]
        [PropertyOrder(1)]
        private Sprite Icon
        {
            get => !helperWindow.FormSelected ? null : helperWindow.MonsterEntry[helperWindow.Form].EggIcon;
            set
            {
                helperWindow.MonsterEntry[helperWindow.Form].EggIcon = value;
                EditorUtility.SetDirty(helperWindow.MonsterEntry);
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// Use the default egg icon?
        /// </summary>
        [ShowIf("@helperWindow.FormSelected")]
        [Button(ButtonSizes.Large)]
        [PropertyOrder(1)]
        public void UseDefaultIcon() => Icon = MonsterCreationHelper.Configuration.DefaultEggIcon;

        /// <summary>
        /// Clear the sprites in this section.
        /// </summary>
        public void ClearSprites()
        {
            Material = null;
            Icon = null;
        }
    }
}