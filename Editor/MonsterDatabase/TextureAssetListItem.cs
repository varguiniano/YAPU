using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Varguiniano.YAPU.Editor.MonsterDatabase
{
    /// <summary>
    /// Data class to hold a texture asset inside a choosing list.
    /// </summary>
    [Serializable]
    public class TextureAssetListItem
    {
        /// <summary>
        /// Reference to the texture.
        /// </summary>
        [InlineEditor(InlineEditorModes.LargePreview)]
        public Texture Texture;

        /// <summary>
        /// Reference to the material, used for assigning the texture when clicked.
        /// </summary>
        [HideInInspector]
        public Material Material;

        /// <summary>
        /// Texture property ID to use when setting the shader textures.
        /// </summary>
        private static readonly int MainTex = Shader.PropertyToID("_SpriteSheet");

        /// <summary>
        /// Assign the texture to the material.
        /// </summary>
        [Button]
        private void Assign()
        {
            Material.SetTexture(MainTex, Texture);
            EditorUtility.SetDirty(Material);
            AssetDatabase.SaveAssets();
        }
    }
}