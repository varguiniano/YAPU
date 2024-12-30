using UnityEngine;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Characters
{
    /// <summary>
    /// Installer for the player character prefab.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Dependency Injection/PlayerCharacterInstaller",
                     fileName = "PlayerCharacterInstaller")]
    public class PlayerCharacterInstaller : ScriptableObjectInstaller
    {
        /// <summary>
        /// Reference to the prefab.
        /// </summary>
        [SerializeField]
        private PlayerCharacter Prefab;

        /// <summary>
        /// Install the factory.
        /// </summary>
        public override void InstallBindings() =>
            Container.BindFactory<PlayerCharacter, PlayerCharacter.Factory>()
                     .FromComponentInNewPrefab(Prefab)
                     .AsSingle()
                     .Lazy();
    }
}