using UnityEngine;
using WhateverDevs.Core.Runtime.DependencyInjection;

namespace Varguiniano.YAPU.Runtime.GameFlow
{
    /// <summary>
    /// Installer for the game flow classes.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Dependency Injection/GameFlowInstaller",
                     fileName = "GameFlowInstaller")]
    public class GameFlowInstaller : LazySingletonScriptableInstaller<NewGameInitializer>
    {
        /// <summary>
        /// Reference to the player teleporter.
        /// </summary>
        [SerializeField]
        private PlayerTeleporter PlayerTeleporter;

        /// <summary>
        /// Reference to the time manager.
        /// </summary>
        [SerializeField]
        private TimeManager TimeManager;

        /// <summary>
        /// Install all the components.
        /// </summary>
        public override void InstallBindings()
        {
            base.InstallBindings();

            Container.QueueForInject(PlayerTeleporter);

            Container.Bind<PlayerTeleporter>().FromInstance(PlayerTeleporter).AsSingle().Lazy();
            
            Container.QueueForInject(TimeManager);

            Container.Bind<TimeManager>().FromInstance(TimeManager).AsSingle().Lazy();
        }
    }
}