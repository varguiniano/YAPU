using UnityEngine;
using WhateverDevs.Core.Runtime.DependencyInjection;
using WhateverDevs.Core.Runtime.Serialization;

namespace Varguiniano.YAPU.Runtime.Saves
{
    /// <summary>
    /// Installer for the savegame manager.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Dependency Injection/Savegame", fileName = "SavegameInstaller")]
    public class SavegameInstaller : LazySingletonScriptableInstaller<SavegameManager>
    {
        /// <summary>
        /// Inject the json serializer and persister into the savegame manager.
        /// </summary>
        public override void InstallBindings()
        {
            base.InstallBindings();

            Container.Bind<ISerializer<string>>()
                     .To<YAPUSavesSerializer>()
                     .AsSingle()
                     .WhenInjectedInto<SavegameManager>()
                     .Lazy();
        }
    }
}