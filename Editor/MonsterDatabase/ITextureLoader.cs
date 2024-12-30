using UnityEngine;

namespace Varguiniano.YAPU.Editor.MonsterDatabase
{
    /// <summary>
    /// Interface that defines a section in the monster creation helper that can load a texture.
    /// </summary>
    public interface ITextureLoader
    {
        /// <summary>
        /// Load the texture provided by a menu.
        /// </summary>
        void LoadTexture(Texture texture);
    }
}