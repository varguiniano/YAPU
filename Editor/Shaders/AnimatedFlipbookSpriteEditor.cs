using JetBrains.Annotations;
using WhateverDevs.URPShaders.Editor;

namespace Varguiniano.YAPU.Editor.Shaders
{
    /// <summary>
    /// Editor for the animated flipbook sprite shader.
    /// </summary>
    [UsedImplicitly]
    public class AnimatedFlipbookSpriteEditor : ShaderGUIBase
    {
        /// <summary>
        /// Paint the inspector.
        /// </summary>
        protected override void PaintUI()
        {
            PaintProperty("_SpriteSheet");
            PaintProperty("_Flipbook");

            PaintProperty("_IsShadow");

            if (GetPropertyBoolValue("_IsShadow"))
            {
                PaintProperty("_ShadowColor");
                PaintProperty("_ShadowAlpha");
            }

            PaintProperty("_AutoAnimate");

            if (GetPropertyBoolValue("_AutoAnimate"))
            {
                PaintProperty("_Speed");
                PaintProperty("_SpecificIndex");

                if (!GetPropertyBoolValue("_SpecificIndex")) return;
                
                PaintProperty("_MinIndex");
                PaintProperty("_MaxIndex");
            }
            else
                PaintProperty("_SpriteIndex");
        }
    }
}