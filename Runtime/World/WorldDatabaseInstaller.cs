using UnityEngine;
using WhateverDevs.Core.Runtime.DependencyInjection;

namespace Varguiniano.YAPU.Runtime.World
{
    /// <summary>
    /// Installer for the world database.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Dependency Injection/WorldDatabase", fileName = "WorldDatabase")]
    public class WorldDatabaseInstaller : LazySingletonScriptableInstaller<WorldDatabase>
    {
    }
}