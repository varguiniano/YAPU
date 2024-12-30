using UnityEngine;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Battle.PlayerControl
{
    /// <summary>
    /// Installer for the player control manager in single player battles.
    /// </summary>
    public class SinglePlayerPlayerControlManagerInstaller : MonoInstaller
    {
        /// <summary>
        /// Reference to the player control manager.
        /// </summary>
        [SerializeField]
        private PlayerControlManager PlayerControlManager;

        /// <summary>
        /// Install the reference as a lazy singleton..
        /// </summary>
        public override void InstallBindings() =>
            Container.Bind<IPlayerControlManager>().FromInstance(PlayerControlManager).AsSingle().Lazy();
    }
}