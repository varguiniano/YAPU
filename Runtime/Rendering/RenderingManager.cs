using System;
using System.Collections;
using System.Collections.Generic;
// ReSharper disable once RedundantUsingDirective
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
// ReSharper disable twice RedundantUsingDirective
using Varguiniano.YAPU.Runtime.Initialization;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Configuration;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Rendering
{
    /// <summary>
    /// Class in charge of changing graphic settings.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Rendering/Manager", fileName = "RenderingManager")]
    public class RenderingManager : WhateverScriptable<RenderingManager>
    {
        /// <summary>
        /// Called when updated the resolution of the application.
        /// </summary>
        public Action ResolutionUpdated;

        /// <summary>
        /// Reference to the configuration manager.
        /// </summary>
        private IConfigurationManager configurationManager;

        /// <summary>
        /// Cached copy of the graphics configuration.
        /// </summary>
        private GraphicsConfiguration configuration;

        /// <summary>
        /// Retrieve the configuration.
        /// </summary>
        [Inject]
        public void Construct(IConfigurationManager configurationManagerReference)
        {
            configurationManager = configurationManagerReference;

            if (!configurationManager.GetConfiguration(out configuration))
                Logger.Error("Couldn´t retrieve configuration.");
        }

        /// <summary>
        /// Get the current monitor index the game is being displayed on.
        /// </summary>
        public int GetCurrentMonitorIndex() => configuration.MonitorIndex;

        /// <summary>
        /// Get the current monitor the game is being displayed on.
        /// </summary>
        private Display GetCurrentMonitor()
        {
            // ReSharper disable once InvertIf
            if (Display.displays.Length <= GetCurrentMonitorIndex())
            {
                Logger.Error("There aren't enough displays to activate display "
                           + GetCurrentMonitorIndex()
                           + ", defaulting to "
                           + (Display.displays.Length - 1)
                           + ".");

                SetCurrentMonitor(Display.displays.Length - 1);
            }

            return Display.displays[GetCurrentMonitorIndex()];
        }

        /// <summary>
        /// Get the number of available monitors.
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfAvailableMonitors()
        {
            #if !UNITY_STANDALONE_WIN
            return 1;
            #endif

            #pragma warning disable CS0162
            // ReSharper disable once HeuristicUnreachableCode
            List<DisplayInfo> displayInfo = new();
            Screen.GetDisplayLayout(displayInfo);
            return displayInfo.Count;
            #pragma warning restore CS0162
        }

        /// <summary>
        /// Set the current monitor.
        /// </summary>
        /// <param name="target">Target monitor.</param>
        // ReSharper disable once RedundantAssignment
        public void SetCurrentMonitor(int target)
        {
            #if !UNITY_STANDALONE_WIN
            target = 0;
            #endif

            if (Display.displays.Length <= target)
            {
                Logger.Error("There aren't enough displays to activate display "
                           + target
                           + ", defaulting to "
                           + (Display.displays.Length - 1)
                           + ".");

                target = Display.displays.Length - 1;
            }

            configuration.MonitorIndex = target;
            SaveConfiguration();

            #if UNITY_EDITOR || UNITY_WEBGL
            Logger.Info("Not updating monitor on this platform.");
            return;
            #endif

            #pragma warning disable 162
            // ReSharper disable HeuristicUnreachableCode

            UpdateToDisplayOnCurrentMonitor();

            CoroutineRunner.Instance.StartCoroutine(UpdateResolutionAndFullscreen(false));

            // Do it twice in case resolution changed.
            UpdateToDisplayOnCurrentMonitor();

            // ReSharper restore HeuristicUnreachableCode
            #pragma warning restore CS0162
        }

        /// <summary>
        /// Move the screen to render on the current monitor.
        /// </summary>
        public void UpdateToDisplayOnCurrentMonitor()
        {
            #if UNITY_STANDALONE_WIN || UNITY_EDITOR
            List<DisplayInfo> displayInfo = new();
            Screen.GetDisplayLayout(displayInfo);
            Screen.MoveMainWindowTo(displayInfo[GetCurrentMonitorIndex()], GetCurrentMonitorCenter());
            #endif
        }

        /// <summary>
        /// Get the current monitor's center.
        /// </summary>
        private Vector2Int GetCurrentMonitorCenter()
        {
            int nativeWidth = GetCurrentMonitor().systemWidth;
            int nativeHeight = GetCurrentMonitor().systemHeight;
            int currentWidth = Screen.width;
            int currentHeight = Screen.height;

            return new Vector2Int(nativeWidth / 2 - currentWidth / 2, nativeHeight / 2 - currentHeight / 2);
        }

        /// <summary>
        /// Get the current resolution.
        /// </summary>
        /// <returns>The current resolution.</returns>
        public Resolution GetCurrentResolution() => configuration.Resolution;

        /// <summary>
        /// Set a new resolution.
        /// </summary>
        /// <param name="resolution">Resolution to set.</param>
        public void SetResolution(Resolution resolution)
        {
            configuration.Resolution = resolution;

            SaveConfiguration();
            CoroutineRunner.Instance.StartCoroutine(UpdateResolutionAndFullscreen(false));
        }

        /// <summary>
        /// Get a list of the resolutions available for the current monitor.
        /// </summary>
        /// <returns>A list of those resolutions.</returns>
        public List<Resolution> GetAvailableResolutionsForThisMonitor()
        {
            List<Resolution> available = new();

            StringBuilder builder = new();

            builder.AppendLine("Monitor of size "
                             + Display.main.systemWidth
                             + "x"
                             + Display.main.systemHeight
                             + ".");

            builder.AppendLine("Compatible resolutions:");

            foreach (Resolution resolution in Utils.GetAllItems<Resolution>())
                if (IsResolutionCompatibleWithCurrentMonitor(resolution))
                {
                    builder.AppendLine(resolution.ToString());
                    available.Add(resolution);
                }

            Logger.Info(builder);

            return available;
        }

        /// <summary>
        /// Get the fullscreen mode of the app.
        /// </summary>
        /// <returns>The fullscreen mode.</returns>
        public WindowType GetWindowType() => configuration.WindowType;

        /// <summary>
        /// Set the fullscreen mode of the app.
        /// </summary>
        /// <param name="mode">Mode to set.</param>
        /// <param name="showRebootWarning">Show a warning to reboot if switched to borderless?</param>
        public void SetWindowType(WindowType mode, bool showRebootWarning = true)
        {
            configuration.WindowType = mode;

            SaveConfiguration();
            CoroutineRunner.Instance.StartCoroutine(UpdateResolutionAndFullscreen(showRebootWarning));
        }

        /// <summary>
        /// Update the screen with the resolution and fullscreen set on the config.
        /// </summary>
        public IEnumerator UpdateResolutionAndFullscreen(bool showRebootWarning = true)
        {
            yield return WaitAFrame;

            QualitySettings.vSyncCount = 0;

            Application.targetFrameRate = 120;

            #if UNITY_EDITOR || UNITY_WEBGL
            Logger.Info("Not updating resolution on this platform.");
            yield break;
            #endif

            // BUG: For some reason Exclusive Fullscreen is crashing the game on some laptops with external monitors.
            // It's bullshit anyway, so a little hack to avoid it.
            if (GetWindowType() == WindowType.ExclusiveFullscreen)
            {
                configuration.WindowType = WindowType.FullscreenWindow;

                SaveConfiguration();
            }

            #pragma warning disable 162

            // ReSharper disable HeuristicUnreachableCode

            Vector2 resolution;

            if (IsResolutionCompatibleWithCurrentMonitor(configuration.Resolution))
                resolution = configuration.Resolution.ToVector2();
            else
            {
                Logger.Info("Resolution "
                          + configuration.Resolution
                          + " in config not compatible, looking for highest compatible resolution.");

                resolution = GetAvailableResolutionsForThisMonitor().Last().ToVector2();
            }

            Logger.Info("Setting screen to "
                      + resolution.x
                      + "x"
                      + resolution.y
                      + " at "
                      + configuration.WindowType
                      + ".");

            #if UNITY_STANDALONE_WIN && !UNITY_EDITOR
                if(!BorderlessWindow.framed)
                    BorderlessWindow.SetFramedWindow();
            #endif

            yield return WaitAFrame;

            Screen.SetResolution((int) resolution.x,
                                 (int) resolution.y,
                                 configuration.WindowType.ToFullScreenMode());

            yield return WaitAFrame;

            #if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            if (configuration.WindowType != WindowType.BorderlessWindow
             && configuration.WindowType != WindowType.FullscreenWindow)
                BorderlessWindow.SetFramedWindow();
            else if (configuration.WindowType == WindowType.BorderlessWindow)
            {
                BorderlessWindow.SetFramelessWindow();

                yield return WaitAFrame;

                if (!File.Exists(ShortcutHelper.BorderlessShortcutPath)) ShortcutHelper.CreateBorderlessShortcut();

                if (showRebootWarning)
                    DialogManager.ShowDialog("Resolution/BorderlessGameRebootWarning",
                                             modifiers: ShortcutHelper.BorderlessShortcutPath);
            }

            #else

            if (configuration.WindowType is WindowType.BorderlessWindow)
                Logger.Warn("Borderless window only works on Windows Standalone platforms.");

            #endif

            ResolutionUpdated?.Invoke();

            // ReSharper restore HeuristicUnreachableCode
            #pragma warning restore 162
        }

        /// <summary>
        /// Save the configuration.
        /// </summary>
        private void SaveConfiguration()
        {
            if (!configurationManager.SetConfiguration(configuration)) Logger.Error("Couldn't save configuration.");
        }

        /// <summary>
        /// Check if a resolution is compatible with the current monitor.
        /// </summary>
        /// <param name="resolution">Resolution to check.</param>
        /// <returns>True if it is.</returns>
        private bool IsResolutionCompatibleWithCurrentMonitor(Resolution resolution)
        {
            Vector2 resolutionVector = resolution.ToVector2();
            Display monitor = GetCurrentMonitor();

            if (monitor.systemWidth < resolutionVector.x) return false;

            return monitor.systemHeight >= resolutionVector.y;
        }
    }
}