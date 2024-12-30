using UnityEngine;
using WhateverDevs.Core.Runtime.DependencyInjection;

namespace Varguiniano.YAPU.Runtime.Monster.Breeding
{
    /// <summary>
    /// Installer for the hatching manager.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Dependency Injection/Hatching Manager", fileName = "HatchingManagerInstaller")]
    public class HatchingManagerInstaller : LazySingletonScriptableInstaller<HatchingManager>
    {
    }
}