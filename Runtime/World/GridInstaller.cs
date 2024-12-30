using UnityEngine;
using Zenject;

namespace Varguiniano.YAPU.Runtime.World
{
    /// <summary>
    /// Installer for the grid data.
    /// </summary>
    public class GridInstaller : MonoInstaller
    {
        /// <summary>
        /// Reference to the grid adata.
        /// </summary>
        [SerializeField]
        private GridController GridController;

        /// <summary>
        /// Inject the reference as a lazy singleton to all ISceneMembers.
        /// </summary>
        public override void InstallBindings() =>
            Container.Bind<GridController>().FromInstance(GridController).AsSingle().WhenInjectedInto<ISceneMember>().Lazy();
    }
}