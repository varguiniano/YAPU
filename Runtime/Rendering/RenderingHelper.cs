using System;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Rendering
{
    /// <summary>
    /// Helper class with methods for rendering.
    /// </summary>
    public static class RenderingHelper
    {
        /// <summary>
        /// Translate window type to Unity's fullscreen mode.
        /// </summary>
        /// <param name="windowType">Type of the window.</param>
        /// <returns>Corresponding fullscreen mode.</returns>
        public static FullScreenMode ToFullScreenMode(this WindowType windowType) =>
            windowType switch
            {
                WindowType.ExclusiveFullscreen => FullScreenMode.ExclusiveFullScreen,
                WindowType.FullscreenWindow => FullScreenMode.FullScreenWindow,
                WindowType.Window => FullScreenMode.Windowed,
                WindowType.BorderlessWindow => FullScreenMode.Windowed,
                _ => FullScreenMode.ExclusiveFullScreen
            };

        /// <summary>
        /// Get the localization key of a resolution.
        /// </summary>
        /// <param name="resolution">Resolution to get.</param>
        /// <returns>Its localization key.</returns>
        public static string GetLocalizationKey(this Resolution resolution) => "Resolution/" + resolution;

        /// <summary>
        /// Translate a resolution to a vector 2.
        /// </summary>
        /// <param name="resolution">Resolution to translate.</param>
        /// <returns>A vector 2 struct.</returns>
        public static Vector2 ToVector2(this Resolution resolution) =>
            resolution switch
            {
                Resolution.R854X480 => new Vector2(854, 480),
                Resolution.R960X540 => new Vector2(960, 540),
                Resolution.R1280X720 => new Vector2(1280, 720),
                Resolution.R1600X900 => new Vector2(1600, 900),
                Resolution.R1920X1080 => new Vector2(1920, 1080),
                Resolution.R2560X1440 => new Vector2(2560, 1440),
                Resolution.R3840X2160 => new Vector2(3840, 2160),
                _ => Vector2.zero
            };
    }
}