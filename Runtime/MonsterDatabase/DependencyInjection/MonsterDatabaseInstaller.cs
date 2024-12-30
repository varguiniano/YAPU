using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Experience;
using Zenject;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.DependencyInjection
{
    /// <summary>
    /// DI Installer for the monster database.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Dependency Injection/Database Installer", fileName = "MonsterDatabaseInstaller")]
    public class MonsterDatabaseInstaller : ScriptableObjectInstaller
    {
        /// <summary>
        /// Reference to the database.
        /// </summary>
        [SerializeField]
        private MonsterDatabaseInstance Database;

        /// <summary>
        /// Reference to the experience lookup table.
        /// </summary>
        [SerializeField]
        private ExperienceLookupTable XpLookupTable;

        /// <summary>
        /// Install the references as lazy singletons.
        /// </summary>
        public override void InstallBindings()
        {
            Container.QueueForInject(Database);
            Container.QueueForInject(XpLookupTable);

            Container.Bind<MonsterDatabaseInstance>().FromInstance(Database).AsSingle().Lazy();
            Container.Bind<ExperienceLookupTable>().FromInstance(XpLookupTable).AsSingle().Lazy();
        }
    }
}