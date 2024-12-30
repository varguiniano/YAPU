using UnityEngine;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Input
{
    /// <summary>
    /// Installer for the input manager.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Dependency Injection/Input Manager Installer",
                     fileName = "InputManagerInstaller")]
    public class InputManagerInstaller : ScriptableObjectInstaller
    {
        /// <summary>
        /// Create a singleton instance and inject it.
        /// </summary>
        public override void InstallBindings()
        {
            IInputManager inputManager = InputManager.Instance;

            Container.QueueForInject(inputManager);

            Container.Bind<IInputManager>().FromInstance(inputManager).AsSingle().Lazy();
        }
    }
}