using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Varguiniano.YAPU.Editor.MonsterDatabase
{
    /// <summary>
    /// Data class to hold a texture asset inside a choosing list.
    /// </summary>
    [Serializable]
    public class GenericTextureAssetListItem
    {
        /// <summary>
        /// Reference to the texture.
        /// </summary>
        [InlineEditor(InlineEditorModes.LargePreview)]
        public Texture Texture;

        /// <summary>
        /// Object to call back to when the texture has been loaded.
        /// </summary>
        public ITextureLoader TextureLoader;

        /// <summary>
        /// Call back with the texture.
        /// </summary>
        [Button]
        private void Assign() => TextureLoader.LoadTexture(Texture);
    }
}