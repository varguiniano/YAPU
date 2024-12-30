using UnityEngine;
using WhateverDevs.Core.Runtime.Configuration;
using WhateverDevs.Core.Runtime.Persistence;

namespace Varguiniano.YAPU.Runtime.Configuration
{
    /// <summary>
    /// Extenject installer for the Configuration Manager.
    /// We inject persisters for only persistent data path so it´s compatible with all platforms.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Dependency Injection/Configuration Manager Installer",
                     fileName = "ConfigurationManagerInstaller")]
    public class ConfigurationManagerInstaller : WhateverDevs.Core.Runtime.Configuration.ConfigurationManagerInstaller
    {
        /// <summary>
        /// Reference to the yapu settings.
        /// </summary>
        [SerializeField]
        private YAPUSettings YAPUSettings;
        
        /// <summary>
        /// Inject the YAPU Settings reference.
        /// We inject persisters for only persistent data path so it´s compatible with all platforms.
        /// </summary>
        public override void InstallBindings()
        {
            Container.Bind<YAPUSettings>().FromInstance(YAPUSettings).AsSingle().Lazy();
            
            Container.Bind<IPersister>()
                     .To<ConfigurationJsonFilePersisterOnPersistentDataPath>()
                     .AsCached()
                     .WhenInjectedInto<IConfiguration>()
                     .Lazy();
            
            base.InstallBindings();
        }
    }
}