using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.Animation.Debug
{
    /// <summary>
    /// It's difficult to know which Vector4 values are being used by the shadows midtones highlights post processing.
    /// This scripts reads them so that we can copy them to modify them by script.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Volume))]
    public class ShaMidHighValuesReader : WhateverBehaviour<ShaMidHighValuesReader>
    {
        /// <summary>
        /// Shadows value.
        /// </summary>
        [SerializeField]
        [UsedImplicitly]
        private Vector4 Shadows;

        /// <summary>
        /// Midtones value.
        /// </summary>
        [SerializeField]
        [UsedImplicitly]
        private Vector4 Midtones;

        /// <summary>
        /// Highlights value.
        /// </summary>
        [SerializeField]
        [UsedImplicitly]
        private Vector4 Highlights;

        /// <summary>
        /// Reference to the post processing effect.
        /// </summary>
        private ShadowsMidtonesHighlights Effect
        {
            get
            {
                if (effect != null) return effect;

                Volume volume = GetCachedComponent<Volume>();

                if (!volume.sharedProfile.TryGet(out effect))
                    Logger.Error("No shadows midtones highlights effect found in the volume.");

                return effect;
            }
        }

        /// <summary>
        /// Backfield for effect.
        /// </summary>
        private ShadowsMidtonesHighlights effect;

        /// <summary>
        /// Read the values.
        /// </summary>
        private void Update()
        {
            Shadows = Effect.shadows.GetValue<Vector4>();
            Midtones = Effect.midtones.GetValue<Vector4>();
            Highlights = Effect.highlights.GetValue<Vector4>();
        }
    }
}