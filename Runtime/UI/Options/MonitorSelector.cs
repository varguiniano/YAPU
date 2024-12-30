using System.Collections.Generic;
using Varguiniano.YAPU.Runtime.Rendering;
using WhateverDevs.Core.Runtime.DataStructures;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Options
{
    /// <summary>
    /// Controller for the menu option to select a monitor.
    /// </summary>
    public class MonitorSelector : OptionsMenuItem
    {
        /// <summary>
        /// Reference to the rendering manager.
        /// </summary>
        private RenderingManager renderingManager;

        /// <summary>
        /// Number of available monitors.
        /// </summary>
        private int numberOfMonitors;

        /// <summary>
        /// Get the current option.
        /// </summary>
        /// <param name="renderingManagerReference">Reference to the rendering manager.</param>
        [Inject]
        public void Construct(RenderingManager renderingManagerReference)
        {
            renderingManager = renderingManagerReference;

            UpdateOptions();
        }

        /// <summary>
        /// Update the displayed options.
        /// </summary>
        private void UpdateOptions()
        {
            numberOfMonitors = renderingManager.GetNumberOfAvailableMonitors();

            Options = new List<ObjectPair<string, string>>();

            for (int i = 0; i < numberOfMonitors; ++i)
                Options.Add(new ObjectPair<string, string>()
                            {
                                Key = i.ToString(),
                                Value = "Options/Monitor/Description"
                            });

            SetOption(renderingManager.GetCurrentMonitorIndex(), true);
        }

        /// <summary>
        /// Update the current monitor.
        /// </summary>
        /// <param name="isFirstSetup"></param>
        protected override void OnOptionSwitched(bool isFirstSetup) => renderingManager.SetCurrentMonitor(CurrentIndex);
    }
}