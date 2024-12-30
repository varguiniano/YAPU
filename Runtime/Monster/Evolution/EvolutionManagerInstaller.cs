using UnityEngine;
using WhateverDevs.Core.Runtime.DependencyInjection;

namespace Varguiniano.YAPU.Runtime.Monster.Evolution
{
    /// <summary>
    /// Installer for the evolution manager.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Dependency Injection/Evolution Manager", fileName = "EvolutionManagerInstaller")]
    public class EvolutionManagerInstaller : LazySingletonScriptableInstaller<EvolutionManager>
    {
    }
}