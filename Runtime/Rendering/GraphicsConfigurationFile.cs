using System;
using UnityEngine;
using WhateverDevs.Core.Runtime.Configuration;

namespace Varguiniano.YAPU.Runtime.Rendering
{
    /// <summary>
    /// Container object for the graphics configuration file.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Rendering/Configuration", fileName = "GraphicsConfiguration")]
    public class
        GraphicsConfigurationFile : ConfigurationScriptableHolderUsingFirstValidPersister<GraphicsConfiguration>
    {
    }

    /// <summary>
    /// Data class containing the graphics configuration.
    /// </summary>
    [Serializable]
    public class GraphicsConfiguration : ConfigurationData
    {
        /// <summary>
        /// Resolution of the application.
        /// </summary>
        public Resolution Resolution = Resolution.R1920X1080;

        /// <summary>
        /// Fullscreen mode of the application.
        /// </summary>
        public WindowType WindowType;

        /// <summary>
        /// Index of the monitor to render on.
        /// </summary>
        public int MonitorIndex;

        /// <summary>
        /// Clone this data into a new instance of the same type.
        /// </summary>
        /// <typeparam name="TConfigurationData">Type of the configuration.</typeparam>
        /// <returns>The cloned config.</returns>
        protected override TConfigurationData Clone<TConfigurationData>() =>
            new GraphicsConfiguration
            {
                Resolution = Resolution,
                WindowType = WindowType,
                MonitorIndex = MonitorIndex
            } as TConfigurationData;
    }
}