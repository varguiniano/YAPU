using UnityEngine;
using WhateverDevs.Core.Runtime.DependencyInjection;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Installer for the Battle Launcher.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Dependency Injection/Battle Launcher Installer",
                     fileName = "BattleLauncherInstaller")]
    public class BattleLauncherInstaller : LazySingletonScriptableInstaller<BattleLauncher>
    {
        /// <summary>
        /// Queue for reference and then install.
        /// </summary>
        public override void InstallBindings()
        {
            Reference.Reset();
            
            Container.QueueForInject(Reference);

            Container.Bind<IBattleLauncher>().FromInstance(Reference).AsSingle().Lazy();
        }
    }
}