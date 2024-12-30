using UnityEngine;
using WhateverDevs.Core.Runtime.DependencyInjection;

namespace Varguiniano.YAPU.Runtime.World
{
    /// <summary>
    /// Installer for the tile data.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Dependency Injection/TileData", fileName = "TileData")]
    public class TileDataInstaller : LazySingletonScriptableInstaller<TileData>
    {
    }
}