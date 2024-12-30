using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Rendering;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Options
{
    /// <summary>
    /// Option to change the fullscreen type.
    /// </summary>
    public class FullscreenTypeSelector : OptionsMenuItem
    {
        /// <summary>
        /// Types of options available.
        /// </summary>
        [SerializeField]
        private List<WindowType> OptionTypes;

        /// <summary>
        /// Reference to the rendering manager.
        /// </summary>
        private RenderingManager renderingManager;

        /// <summary>
        /// Get the current option.
        /// </summary>
        /// <param name="renderingManagerReference">Reference to the rendering manager.</param>
        [Inject]
        public void Construct(RenderingManager renderingManagerReference)
        {
            renderingManager = renderingManagerReference;

            SetOption(OptionTypes.IndexOf(renderingManager.GetWindowType()), true);
        }

        /// <summary>
        /// Update the fullscreen type of the app.
        /// </summary>
        /// <param name="isFirstSetup"></param>
        protected override void OnOptionSwitched(bool isFirstSetup)
        {
            WindowType newWindowType = OptionTypes[CurrentIndex];

            if (newWindowType != renderingManager.GetWindowType())
                renderingManager.SetWindowType(newWindowType, !isFirstSetup);
        }
    }
}