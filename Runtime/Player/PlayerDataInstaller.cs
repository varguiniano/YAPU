using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Bags;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.Quests;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Player
{
    /// <summary>
    /// Installer for the player data.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Dependency Injection/Player Installer",
                     fileName = "PlayerDataInstaller")]
    public class PlayerDataInstaller : ScriptableObjectInstaller
    {
        /// <summary>
        /// Reference to the player roster.
        /// </summary>
        [SerializeField]
        private Roster PlayerRoster;

        /// <summary>
        /// Reference to the player bag.
        /// </summary>
        [SerializeField]
        private Bag PlayerBag;

        /// <summary>
        /// Reference to the monster storage.
        /// </summary>
        [SerializeField]
        private MonsterStorage MonsterStorage;

        /// <summary>
        /// Reference to the player's dex.
        /// </summary>
        [SerializeField]
        private Dex Dex;

        /// <summary>
        /// Reference to the player character data.
        /// </summary>
        [SerializeField]
        private CharacterData PlayerCharacterData;

        /// <summary>
        /// References to the global game data.
        /// </summary>
        [SerializeField]
        private GlobalGameData GlobalGameData;

        /// <summary>
        /// Reference to the player settings.
        /// </summary>
        [SerializeField]
        private PlayerSettings PlayerSettings;

        /// <summary>
        /// Reference to the quests tracker.
        /// </summary>
        [SerializeField]
        private QuestManager QuestManager;

        /// <summary>
        /// Inject all player data on the IPlayerDataReceivers.
        /// </summary>
        public override void InstallBindings()
        {
            Container.Bind<Roster>()
                     .FromInstance(PlayerRoster)
                     .AsSingle()
                     .WhenInjectedInto<IPlayerDataReceiver>()
                     .Lazy();

            Container.Bind<Bag>()
                     .FromInstance(PlayerBag)
                     .AsSingle()
                     .WhenInjectedInto<IPlayerDataReceiver>()
                     .Lazy();

            Container.QueueForInject(MonsterStorage);

            Container.Bind<MonsterStorage>()
                     .FromInstance(MonsterStorage)
                     .AsSingle()
                     .WhenInjectedInto<IPlayerDataReceiver>()
                     .Lazy();

            Container.QueueForInject(Dex);

            Container.Bind<Dex>()
                     .FromInstance(Dex)
                     .AsSingle()
                     .WhenInjectedInto<IPlayerDataReceiver>()
                     .Lazy();

            Container.Bind<CharacterData>()
                     .FromInstance(PlayerCharacterData)
                     .AsSingle()
                     .WhenInjectedInto<IPlayerDataReceiver>()
                     .Lazy();

            Container.QueueForInject(GlobalGameData);

            Container.Bind<GlobalGameData>()
                     .FromInstance(GlobalGameData)
                     .AsSingle()
                     .WhenInjectedInto<IPlayerDataReceiver>()
                     .Lazy();

            Container.Bind<PlayerSettings>()
                     .FromInstance(PlayerSettings)
                     .AsSingle()
                     .WhenInjectedInto<IPlayerDataReceiver>()
                     .Lazy();

            Container.QueueForInject(QuestManager);

            Container.Bind<QuestManager>()
                     .FromInstance(QuestManager)
                     .AsSingle()
                     .WhenInjectedInto<IPlayerDataReceiver>()
                     .Lazy();
        }
    }
}