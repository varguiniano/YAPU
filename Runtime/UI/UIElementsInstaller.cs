using UnityEngine;
using Varguiniano.YAPU.Runtime.UI.Bags;
using Varguiniano.YAPU.Runtime.UI.Dex;
using Varguiniano.YAPU.Runtime.UI.Dialogs.MoveTutor;
using Varguiniano.YAPU.Runtime.UI.Items;
using Varguiniano.YAPU.Runtime.UI.MainMenu;
using Varguiniano.YAPU.Runtime.UI.Monsters.MonstersMenu;
using Varguiniano.YAPU.Runtime.UI.Monsters.MonstersMenu.Filters;
using Varguiniano.YAPU.Runtime.UI.Moves;
using Varguiniano.YAPU.Runtime.UI.Profile;
using Varguiniano.YAPU.Runtime.UI.Quests;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI
{
    /// <summary>
    /// Installer for all UI related dependencies.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Dependency Injection/UI", fileName = "UIElementsInstaller")]
    public class UIElementsInstaller : ScriptableObjectInstaller
    {
        /// <summary>
        /// Reference to the item button prefab.
        /// </summary>
        [SerializeField]
        private ItemButton BagItemButtonPrefab;

        /// <summary>
        /// Reference to the item button prefab.
        /// </summary>
        [SerializeField]
        private ItemButton QuickAccessItemButtonPrefab;

        /// <summary>
        /// Reference to the shop item button prefab.
        /// </summary>
        [SerializeField]
        private ItemButton ShopItemButtonPrefab;

        /// <summary>
        /// Prefab for a storage monster button.
        /// </summary>
        [SerializeField]
        private StorageMonsterButton StorageMonsterButtonPrefab;

        /// <summary>
        /// Prefab for a species button used when filtering.
        /// </summary>
        [SerializeField]
        private SpeciesButton SpeciesFilterButtonPrefab;

        /// <summary>
        /// Prefab for a move button used normally.
        /// </summary>
        [SerializeField]
        private MoveButton StandardMoveButton;

        /// <summary>
        /// Prefab for a move button used when filtering.
        /// </summary>
        [SerializeField]
        private MoveButton MoveFilterButtonPrefab;

        /// <summary>
        /// Prefab for buttons that display a move learnt by level.
        /// </summary>
        [SerializeField]
        private MoveByLevelButton MoveByLevelButton;

        /// <summary>
        /// Prefab for a ability button used when filtering.
        /// </summary>
        [SerializeField]
        private AbilityButton AbilityButtonPrefab;

        /// <summary>
        /// Prefab for a Nature button used when filtering.
        /// </summary>
        [SerializeField]
        private NatureButton NatureButtonPrefab;

        /// <summary>
        /// Prefab for an EggGroup Button used when filtering.
        /// </summary>
        [SerializeField]
        private EggGroupButton EggGroupButtonPrefab;

        /// <summary>
        /// Prefab for a monster relationship button that displays relationships in the dex.
        /// </summary>
        [SerializeField]
        private MonsterRelationshipButton MonsterRelationshipButton;

        /// <summary>
        /// Prefab for a button representing a savegame.
        /// </summary>
        [SerializeField]
        private SavegameButton SavegameButton;

        /// <summary>
        /// Prefab for a button representing a badge.
        /// </summary>
        [SerializeField]
        private BadgeButton BadgeButton;

        /// <summary>
        /// Prefab for a button representing a monster in the dex.
        /// </summary>
        [SerializeField]
        private DexMonsterButton DexMonsterButton;

        /// <summary>
        /// Prefab for a button representing an encounter in the dex.
        /// </summary>
        [SerializeField]
        private DexEncounterButton DexEncounterButton;

        /// <summary>
        /// Prefab for a button representing a quest.
        /// </summary>
        [SerializeField]
        private QuestButton QuestButton;

        /// <summary>
        /// Install the dependencies.
        /// </summary>
        public override void InstallBindings()
        {
            Container.BindFactory<ItemButton, ItemButton.Factory>()
                     .FromComponentInNewPrefab(BagItemButtonPrefab)
                     .AsCached()
                     .WhenInjectedInto<BagTab>()
                     .Lazy();

            Container.BindFactory<ItemButton, QuickAccessItemButtonFactory>()
                     .FromComponentInNewPrefab(QuickAccessItemButtonPrefab)
                     .AsCached()
                     .WhenInjectedInto<QuickAccessItemsMenu>()
                     .Lazy();

            Container.BindFactory<ItemButton, ItemButton.Factory>()
                     .FromComponentInNewPrefab(ShopItemButtonPrefab)
                     .AsCached()
                     .WhenInjectedInto<ShopItemSelector>()
                     .Lazy();

            Container.BindFactory<StorageMonsterButton, StorageMonsterButton.Factory>()
                     .FromComponentInNewPrefab(StorageMonsterButtonPrefab)
                     .AsCached()
                     .WhenInjectedInto<StorageMenu>()
                     .Lazy();

            Container.BindFactory<SpeciesButton, SpeciesButton.Factory>()
                     .FromComponentInNewPrefab(SpeciesFilterButtonPrefab)
                     .AsCached()
                     .WhenInjectedInto<SpeciesFilterMenu>()
                     .Lazy();

            Container.BindFactory<MoveButton, MoveButton.Factory>()
                     .FromComponentInNewPrefab(MoveFilterButtonPrefab)
                     .AsCached()
                     .WhenInjectedInto<MoveFilterMenu>()
                     .Lazy();

            Container.BindFactory<MoveButton, MoveButton.Factory>()
                     .FromComponentInNewPrefab(StandardMoveButton)
                     .AsCached()
                     .WhenInjectedInto<MovesMenu>()
                     .Lazy();

            Container.BindFactory<MoveByLevelButton, MoveByLevelButton.MoveByLevelFactory>()
                     .FromComponentInNewPrefab(MoveByLevelButton)
                     .AsCached()
                     .WhenInjectedInto<MovesByLevelMenu>()
                     .Lazy();

            Container.BindFactory<AbilityButton, AbilityButton.Factory>()
                     .FromComponentInNewPrefab(AbilityButtonPrefab)
                     .AsCached()
                     .WhenInjectedInto<AbilityFilterMenu>()
                     .Lazy();

            Container.BindFactory<NatureButton, NatureButton.Factory>()
                     .FromComponentInNewPrefab(NatureButtonPrefab)
                     .AsCached()
                     .WhenInjectedInto<NatureFilterMenu>()
                     .Lazy();

            Container.BindFactory<EggGroupButton, EggGroupButton.Factory>()
                     .FromComponentInNewPrefab(EggGroupButtonPrefab)
                     .AsCached()
                     .WhenInjectedInto<EggGroupFilterMenu>()
                     .Lazy();

            Container.BindFactory<MonsterRelationshipButton, MonsterRelationshipButton.Factory>()
                     .FromComponentInNewPrefab(MonsterRelationshipButton)
                     .AsCached()
                     .WhenInjectedInto<MonsterRelationshipMenu>()
                     .Lazy();

            Container.BindFactory<MoveButton, MoveButton.Factory>()
                     .FromComponentInNewPrefab(MoveFilterButtonPrefab)
                     .AsCached()
                     .WhenInjectedInto<MoveSelectionMenu>()
                     .Lazy();

            Container.BindFactory<SavegameButton, SavegameButton.Factory>()
                     .FromComponentInNewPrefab(SavegameButton)
                     .AsCached()
                     .WhenInjectedInto<SavegamesMenu>()
                     .Lazy();

            Container.BindFactory<BadgeButton, BadgeButton.Factory>()
                     .FromComponentInNewPrefab(BadgeButton)
                     .AsCached()
                     .WhenInjectedInto<BadgeList>()
                     .Lazy();

            Container.BindFactory<DexMonsterButton, DexMonsterButton.Factory>()
                     .FromComponentInNewPrefab(DexMonsterButton)
                     .AsCached()
                     .WhenInjectedInto<DexScreen>()
                     .Lazy();

            Container.BindFactory<DexEncounterButton, DexEncounterButton.Factory>()
                     .FromComponentInNewPrefab(DexEncounterButton)
                     .AsCached()
                     .WhenInjectedInto<DexEncounterMenu>()
                     .Lazy();

            Container.BindFactory<QuestButton, QuestButton.Factory>()
                     .FromComponentInNewPrefab(QuestButton)
                     .AsCached()
                     .WhenInjectedInto<QuestsScreen>()
                     .Lazy();
        }
    }
}