using UnityEngine;
using WhateverDevs.Core.Runtime.DependencyInjection;

namespace Varguiniano.YAPU.Runtime.Rendering
{
    /// <summary>
    /// Installer for the rendering system.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Dependency Injection/Rendering", fileName = "RenderingInstaller")]
    public class RenderingInstaller : LazySingletonScriptableInstaller<RenderingManager>
    {
    }
}