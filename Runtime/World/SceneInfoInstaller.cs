using UnityEngine;
using Zenject;

namespace Varguiniano.YAPU.Runtime.World
{
    /// <summary>
    /// Mono installer for a scene's info.
    /// </summary>
    public class SceneInfoInstaller : MonoInstaller
    {
        /// <summary>
        /// Reference to the scene info.
        /// </summary>
        [SerializeField]
        private SceneInfo sceneInfo;

        /// <summary>
        /// Inject the reference as a lazy singleton to all ISceneMembers.
        /// </summary>
        public override void InstallBindings() =>
            Container.Bind<SceneInfo>().FromInstance(sceneInfo).AsSingle().WhenInjectedInto<ISceneMember>().Lazy();
    }
}