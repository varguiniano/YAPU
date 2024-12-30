using UnityEngine;
using WhateverDevs.Core.Runtime.DependencyInjection;

namespace Varguiniano.YAPU.Runtime.Monster.Trade
{
    /// <summary>
    /// Installer for the trade manager.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Dependency Injection/Trade Manager", fileName = "TradeManagerInstaller")]
    public class TradeManagerInstaller : LazySingletonScriptableInstaller<TradeManager>
    {
    }
}