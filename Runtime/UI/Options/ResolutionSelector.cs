using System.Collections.Generic;
using Varguiniano.YAPU.Runtime.Rendering;
using WhateverDevs.Core.Runtime.DataStructures;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Options
{
    /// <summary>
    /// UI element for selecting a resolution.
    /// </summary>
    public class ResolutionSelector : OptionsMenuItem
    {
        /// <summary>
        /// Reference to the rendering manager.
        /// </summary>
        private RenderingManager renderingManager;

        /// <summary>
        /// Available resolutions.
        /// </summary>
        private List<Resolution> resolutions;

        /// <summary>
        /// Get the current option.
        /// </summary>
        /// <param name="renderingManagerReference">Reference to the rendering manager.</param>
        [Inject]
        public void Construct(RenderingManager renderingManagerReference)
        {
            renderingManager = renderingManagerReference;

            UpdateOptions();

            renderingManager.ResolutionUpdated += UpdateOptions;
        }

        /// <summary>
        /// Update the displayed options.
        /// </summary>
        private void UpdateOptions()
        {
            resolutions = renderingManager.GetAvailableResolutionsForThisMonitor();

            Options = new List<ObjectPair<string, string>>();

            foreach (Resolution resolution in resolutions)
                Options.Add(new ObjectPair<string, string>
                            {
                                Key = resolution.GetLocalizationKey(),
                                Value = "Resolution/Description"
                            });

            SetOption(resolutions.IndexOf(renderingManager.GetCurrentResolution()), true);
        }

        /// <summary>
        /// Change the resolution to the new chosen one.
        /// </summary>
        /// <param name="isFirstSetup"></param>
        protected override void OnOptionSwitched(bool isFirstSetup)
        {
            Resolution newResolution = resolutions[CurrentIndex];

            if (newResolution != renderingManager.GetCurrentResolution()) renderingManager.SetResolution(newResolution);
        }
    }
}