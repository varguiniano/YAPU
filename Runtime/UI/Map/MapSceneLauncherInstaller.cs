using UnityEngine;
using WhateverDevs.Core.Runtime.DependencyInjection;

namespace Varguiniano.YAPU.Runtime.UI.Map
{
    /// <summary>
    /// Installer for the Map Scene Launcher.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Dependency Injection/Map Scene Launcher", fileName = "MapSceneLauncherInstaller")]
    public class MapSceneLauncherInstaller : LazySingletonScriptableInstaller<MapSceneLauncher>
    {
    }
}