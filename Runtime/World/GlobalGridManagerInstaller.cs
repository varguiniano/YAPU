using UnityEngine;
using Zenject;

namespace Varguiniano.YAPU.Runtime.World
{
    /// <summary>
    /// Installer for the global grid manager.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Dependency Injection/GlobalGridManagerInstaller",
                     fileName = "GlobalGridManagerInstaller")]
    public class GlobalGridManagerInstaller : ScriptableObjectInstaller
    {
        /// <summary>
        /// Instantiate the Prefab and inject the instance.
        /// </summary>
        public override void InstallBindings()
        {
            GlobalGridManager manager = GlobalGridManager.Instance;

            Container.QueueForInject(manager);

            Container.Bind<GlobalGridManager>().FromInstance(manager).AsSingle().Lazy();
        }
    }
}