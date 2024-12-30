using System;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Varguiniano.YAPU.Runtime.Characters;
using WhateverDevs.Core.Behaviours;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls;
using WhateverDevs.Core.Runtime.Common;

#if UNITY_EDITOR
using System.Collections.Generic;
using Varguiniano.YAPU.Runtime.Badges;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Global;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Side;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Terrain;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.MonsterDatabase.EggGroups;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Natures;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Ribbons;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Species;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;
using Varguiniano.YAPU.Runtime.World;
using WhateverDevs.Core.Editor.Utils;
#endif

namespace Varguiniano.YAPU.Runtime.MonsterDatabase
{
    /// <summary>
    /// Base class for scriptables that are part of the monster database.
    /// </summary>
    /// <typeparam name="T">The implementation of this class</typeparam>
    public abstract class MonsterDatabaseScriptable<T> : WhateverScriptable<T> where T : MonsterDatabaseScriptable<T>
    {
        #if UNITY_EDITOR
        /// <summary>
        /// Init the inspector.
        /// </summary>
        [OnInspectorInit]
        protected virtual void InspectorInit() => InvalidateInspectorCaches();

        /// <summary>
        /// Invalidate all inspector caches.
        /// </summary>
        protected void InvalidateInspectorCaches()
        {
            allMonsters = null;
            allMonsterTypes = null;
            allMoves = null;
            allDamageMoves = null;
            allTwoTurnMoves = null;
            allFixationMoves = null;
            allAbilities = null;
            allForms = null;
            allSpecies = null;
            allEggGroups = null;
            allNatures = null;
            allStatuses = null;
            allGlobalStatuses = null;
            allVolatileStatuses = null;
            allLayeredVolatileStatuses = null;
            allItemCategories = null;
            allItems = null;
            allHoldableItems = null;
            allUsableInBattleOnTargetItems = null;
            allSideStatuses = null;
            allLayeredSideStatuses = null;
            allTerrains = null;
            allBalls = null;
            allCharacterTypes = null;
            allRibbons = null;
            allBadges = null;
            allSceneTags = null;
        }

        /// <summary>
        /// Get all monsters in the database.
        /// </summary>
        /// <returns></returns>
        protected List<MonsterEntry> GetAllMonsters() =>
            allMonsters ??= AssetManagementUtils.FindAssetsByType<MonsterEntry>();

        /// <summary>
        /// Cached list of all monsters.
        /// </summary>
        private List<MonsterEntry> allMonsters;

        /// <summary>
        /// Retrieve all monster types in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        protected List<MonsterType> GetAllMonsterTypes() =>
            allMonsterTypes ??= AssetManagementUtils.FindAssetsByType<MonsterType>();

        /// <summary>
        /// Cached list of all monster types.
        /// </summary>
        private List<MonsterType> allMonsterTypes;

        /// <summary>
        /// Retrieve all moves in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        protected List<Move> GetAllMoves() => allMoves ??= AssetManagementUtils.FindAssetsByType<Move>();

        /// <summary>
        /// Cached list of all moves.
        /// </summary>
        private List<Move> allMoves;

        /// <summary>
        /// Retrieve all Damage Moves in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        protected List<DamageMove> GetAllDamageMoves() =>
            allDamageMoves ??= AssetManagementUtils.FindAssetsByType<DamageMove>();

        /// <summary>
        /// Cached list of all DamageMoves.
        /// </summary>
        private List<DamageMove> allDamageMoves;

        /// <summary>
        /// Retrieve all two turn moves in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        protected List<TwoTurnMove> GetAllTwoTurnMoves() =>
            allTwoTurnMoves ??= AssetManagementUtils.FindAssetsByType<TwoTurnMove>();

        /// <summary>
        /// Cached list of all TwoTurnMoves.
        /// </summary>
        private List<TwoTurnMove> allTwoTurnMoves;

        /// <summary>
        /// Retrieve all two turn moves in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        protected List<FixationMove> GetAllFixationMoves() =>
            allFixationMoves ??= AssetManagementUtils.FindAssetsByType<FixationMove>();

        /// <summary>
        /// Cached list of all FixationMoves.
        /// </summary>
        private List<FixationMove> allFixationMoves;

        /// <summary>
        /// Retrieve all abilities in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        protected List<Ability> GetAllAbilities() => allAbilities ??= AssetManagementUtils.FindAssetsByType<Ability>();

        /// <summary>
        /// Cached list of all Abilities.
        /// </summary>
        private List<Ability> allAbilities;

        /// <summary>
        /// Retrieve all forms in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        protected List<Form> GetAllForms() => allForms ??= AssetManagementUtils.FindAssetsByType<Form>();

        /// <summary>
        /// Cached list of all Forms.
        /// </summary>
        private List<Form> allForms;

        /// <summary>
        /// Retrieve all species in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        protected List<MonsterSpecies> GetAllSpecies() =>
            allSpecies ??= AssetManagementUtils.FindAssetsByType<MonsterSpecies>();

        /// <summary>
        /// Cached list of all Species.
        /// </summary>
        private List<MonsterSpecies> allSpecies;

        /// <summary>
        /// Retrieve all EggGroups in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        protected List<EggGroup> GetAllEggGroups() =>
            allEggGroups ??= AssetManagementUtils.FindAssetsByType<EggGroup>();

        /// <summary>
        /// Cached list of all EggGroups.
        /// </summary>
        private List<EggGroup> allEggGroups;

        /// <summary>
        /// Retrieve all Natures in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        protected List<Nature> GetAllNatures() => allNatures ??= AssetManagementUtils.FindAssetsByType<Nature>();

        /// <summary>
        /// Cached list of all Natures.
        /// </summary>
        private List<Nature> allNatures;

        /// <summary>
        /// Retrieve all Statuses in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        protected List<Status> GetAllStatuses() => allStatuses ??= AssetManagementUtils.FindAssetsByType<Status>();

        /// <summary>
        /// Cached list of all Statuses.
        /// </summary>
        private List<Status> allStatuses;

        /// <summary>
        /// Retrieve all Statuses in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        protected List<VolatileStatus> GetAllVolatileStatuses() =>
            allVolatileStatuses ??= AssetManagementUtils.FindAssetsByType<VolatileStatus>();

        /// <summary>
        /// Cached list of all VolatileStatuses.
        /// </summary>
        private List<VolatileStatus> allVolatileStatuses;

        /// <summary>
        /// Retrieve all layered volatile statuses in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        protected List<LayeredVolatileStatus> GetAllLayeredVolatileStatuses() =>
            allLayeredVolatileStatuses ??= AssetManagementUtils.FindAssetsByType<LayeredVolatileStatus>();

        /// <summary>
        /// Cached list of all LayeredVolatileStatuses.
        /// </summary>
        private List<LayeredVolatileStatus> allLayeredVolatileStatuses;

        /// <summary>
        /// Retrieve all item categories in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        protected List<ItemCategory> GetAllItemCategories() =>
            allItemCategories ??= AssetManagementUtils.FindAssetsByType<ItemCategory>();

        /// <summary>
        /// Cached list of all ItemCategories.
        /// </summary>
        private List<ItemCategory> allItemCategories;

        /// <summary>
        /// Retrieve all items in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        protected List<Item> GetAllItems() => allItems ??= AssetManagementUtils.FindAssetsByType<Item>();

        /// <summary>
        /// Cached list of all Items.
        /// </summary>
        private List<Item> allItems;

        /// <summary>
        /// Retrieve all holdable items in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        protected List<Item> GetAllHoldableItems() =>
            allHoldableItems ??= GetAllItems().Where(item => item.CanBeHeld).ToList();

        /// <summary>
        /// Cached list of all HoldableItems.
        /// </summary>
        private List<Item> allHoldableItems;

        /// <summary>
        /// Retrieve all items that can be used in battle, useful for inspectors.
        /// </summary>
        [UsedImplicitly]
        protected List<Item> GetAllUsableInBattleOnTargetItems() =>
            allUsableInBattleOnTargetItems ??= GetAllItems().Where(item => item.CanBeUsedInBattleOnTarget).ToList();

        /// <summary>
        /// Cached list of all UsableInBattleOnTargetItems.
        /// </summary>
        private List<Item> allUsableInBattleOnTargetItems;

        /// <summary>
        /// Retrieve all global statuses in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        protected List<GlobalStatus> GetAllGlobalStatuses() =>
            allGlobalStatuses ??= AssetManagementUtils.FindAssetsByType<GlobalStatus>();

        /// <summary>
        /// Cached list of all Global statuses.
        /// </summary>
        private List<GlobalStatus> allGlobalStatuses;

        /// <summary>
        /// Retrieve all side statuses in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        protected List<SideStatus> GetAllSideStatuses() =>
            allSideStatuses ??= AssetManagementUtils.FindAssetsByType<SideStatus>();

        /// <summary>
        /// Cached list of all SideStatuses.
        /// </summary>
        private List<SideStatus> allSideStatuses;

        /// <summary>
        /// Retrieve all layered side statuses in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        protected List<LayeredSideStatus> GetAllLayeredSideStatuses() =>
            allLayeredSideStatuses ??= AssetManagementUtils.FindAssetsByType<LayeredSideStatus>();

        /// <summary>
        /// Cached list of all LayeredSideStatuses.
        /// </summary>
        private List<LayeredSideStatus> allLayeredSideStatuses;

        /// <summary>
        /// Retrieve all Terrain in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        protected List<Terrain> GetAllTerrains() => allTerrains ??= AssetManagementUtils.FindAssetsByType<Terrain>();

        /// <summary>
        /// Cached list of all Terrains.
        /// </summary>
        private List<Terrain> allTerrains;

        /// <summary>
        /// Retrieve all balls in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        protected List<Ball> GetAllBalls() => allBalls ??= AssetManagementUtils.FindAssetsByType<Ball>();

        /// <summary>
        /// Cached list of all Balls.
        /// </summary>
        private List<Ball> allBalls;

        /// <summary>
        /// Retrieve all character types in the database.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        protected List<CharacterType> GetAllCharacterTypes() =>
            allCharacterTypes ??= AssetManagementUtils.FindAssetsByType<CharacterType>();

        /// <summary>
        /// Cached list of all CharacterTypes.
        /// </summary>
        private List<CharacterType> allCharacterTypes;

        /// <summary>
        /// Retrieve all Ribbons in the database.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        protected List<Ribbon> GetAllRibbons() => allRibbons ??= AssetManagementUtils.FindAssetsByType<Ribbon>();

        /// <summary>
        /// Cached list of all Ribbons.
        /// </summary>
        private List<Ribbon> allRibbons;

        /// <summary>
        /// Retrieve all badges in the database.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        protected List<Badge> GetAllBadges() => allBadges ??= AssetManagementUtils.FindAssetsByType<Badge>();

        /// <summary>
        /// Cached list of all Badges.
        /// </summary>
        private List<Badge> allBadges;
        
        /// <summary>
        /// Retrieve all SceneTags in the database.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        protected List<SceneTag> GetAllSceneTags() => allSceneTags ??= AssetManagementUtils.FindAssetsByType<SceneTag>();

        /// <summary>
        /// Cached list of all SceneTags.
        /// </summary>
        private List<SceneTag> allSceneTags;
        #endif
    }

    /// <summary>
    /// Data class to be inherited by other data classes in the database.
    /// </summary>
    [Serializable]
    public class MonsterDatabaseData : MonsterDatabaseData<MonsterDatabaseData>
    {
    }

    /// <summary>
    /// Data class to be inherited by other data classes in the database.
    /// </summary>
    [Serializable]
    public class MonsterDatabaseData<T> : Loggable<T> where T : MonsterDatabaseData<T>
    {
        #if UNITY_EDITOR
        /// <summary>
        /// Init the inspector.
        /// </summary>
        [OnInspectorInit]
        public virtual void InspectorInit() => InvalidateInspectorCaches();

        /// <summary>
        /// Invalidate all inspector caches.
        /// </summary>
        public void InvalidateInspectorCaches()
        {
            allMonsters = null;
            allMonsterTypes = null;
            allMoves = null;
            allDamageMoves = null;
            allTwoTurnMoves = null;
            allFixationMoves = null;
            allAbilities = null;
            allForms = null;
            allSpecies = null;
            allEggGroups = null;
            allNatures = null;
            allStatuses = null;
            allGlobalStatuses = null;
            allVolatileStatuses = null;
            allLayeredVolatileStatuses = null;
            allItemCategories = null;
            allItems = null;
            allHoldableItems = null;
            allUsableInBattleOnTargetItems = null;
            allSideStatuses = null;
            allLayeredSideStatuses = null;
            allTerrains = null;
            allBalls = null;
            allCharacterTypes = null;
            allRibbons = null;
            allBadges = null;
            allSceneTags = null;
        }

        // ReSharper disable StaticMemberInGenericType

        /// <summary>
        /// Get all monsters in the database.
        /// </summary>
        /// <returns></returns>
        public static List<MonsterEntry> GetAllMonsters() =>
            allMonsters ??= AssetManagementUtils.FindAssetsByType<MonsterEntry>();

        /// <summary>
        /// Cached list of all monsters.
        /// </summary>
        private static List<MonsterEntry> allMonsters;

        /// <summary>
        /// Retrieve all monster types in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        public static List<MonsterType> GetAllMonsterTypes() =>
            allMonsterTypes ??= AssetManagementUtils.FindAssetsByType<MonsterType>();

        /// <summary>
        /// Cached list of all monster types.
        /// </summary>
        private static List<MonsterType> allMonsterTypes;

        /// <summary>
        /// Retrieve all moves in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        public static List<Move> GetAllMoves() => allMoves ??= AssetManagementUtils.FindAssetsByType<Move>();

        /// <summary>
        /// Cached list of all moves.
        /// </summary>
        private static List<Move> allMoves;

        /// <summary>
        /// Retrieve all Damage Moves in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        public static List<DamageMove> GetAllDamageMoves() =>
            allDamageMoves ??= AssetManagementUtils.FindAssetsByType<DamageMove>();

        /// <summary>
        /// Cached list of all DamageMoves.
        /// </summary>
        private static List<DamageMove> allDamageMoves;

        /// <summary>
        /// Retrieve all two turn moves in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        public static List<TwoTurnMove> GetAllTwoTurnMoves() =>
            allTwoTurnMoves ??= AssetManagementUtils.FindAssetsByType<TwoTurnMove>();

        /// <summary>
        /// Cached list of all TwoTurnMoves.
        /// </summary>
        private static List<TwoTurnMove> allTwoTurnMoves;

        /// <summary>
        /// Retrieve all two turn moves in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        public static List<FixationMove> GetAllFixationMoves() =>
            allFixationMoves ??= AssetManagementUtils.FindAssetsByType<FixationMove>();

        /// <summary>
        /// Cached list of all FixationMoves.
        /// </summary>
        private static List<FixationMove> allFixationMoves;

        /// <summary>
        /// Retrieve all abilities in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        public static List<Ability> GetAllAbilities() =>
            allAbilities ??= AssetManagementUtils.FindAssetsByType<Ability>();

        /// <summary>
        /// Cached list of all Abilities.
        /// </summary>
        private static List<Ability> allAbilities;

        /// <summary>
        /// Retrieve all forms in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        public static List<Form> GetAllForms() => allForms ??= AssetManagementUtils.FindAssetsByType<Form>();

        /// <summary>
        /// Cached list of all Forms.
        /// </summary>
        private static List<Form> allForms;

        /// <summary>
        /// Retrieve all species in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        public static List<MonsterSpecies> GetAllSpecies() =>
            allSpecies ??= AssetManagementUtils.FindAssetsByType<MonsterSpecies>();

        /// <summary>
        /// Cached list of all Species.
        /// </summary>
        private static List<MonsterSpecies> allSpecies;

        /// <summary>
        /// Retrieve all EggGroups in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        public static List<EggGroup> GetAllEggGroups() =>
            allEggGroups ??= AssetManagementUtils.FindAssetsByType<EggGroup>();

        /// <summary>
        /// Cached list of all EggGroups.
        /// </summary>
        private static List<EggGroup> allEggGroups;

        /// <summary>
        /// Retrieve all Natures in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        public static List<Nature> GetAllNatures() => allNatures ??= AssetManagementUtils.FindAssetsByType<Nature>();

        /// <summary>
        /// Cached list of all Natures.
        /// </summary>
        private static List<Nature> allNatures;

        /// <summary>
        /// Retrieve all Statuses in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        public static List<Status> GetAllStatuses() => allStatuses ??= AssetManagementUtils.FindAssetsByType<Status>();

        /// <summary>
        /// Cached list of all Statuses.
        /// </summary>
        private static List<Status> allStatuses;

        /// <summary>
        /// Retrieve all global statuses in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        protected List<GlobalStatus> GetAllGlobalStatuses() =>
            allGlobalStatuses ??= AssetManagementUtils.FindAssetsByType<GlobalStatus>();

        /// <summary>
        /// Cached list of all Global statuses.
        /// </summary>
        private List<GlobalStatus> allGlobalStatuses;

        /// <summary>
        /// Retrieve all Statuses in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        public static List<VolatileStatus> GetAllVolatileStatuses() =>
            allVolatileStatuses ??= AssetManagementUtils.FindAssetsByType<VolatileStatus>();

        /// <summary>
        /// Cached list of all VolatileStatuses.
        /// </summary>
        private static List<VolatileStatus> allVolatileStatuses;

        /// <summary>
        /// Retrieve all layered volatile statuses in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        protected List<LayeredVolatileStatus> GetAllLayeredVolatileStatuses() =>
            allLayeredVolatileStatuses ??= AssetManagementUtils.FindAssetsByType<LayeredVolatileStatus>();

        /// <summary>
        /// Cached list of all LayeredVolatileStatuses.
        /// </summary>
        private List<LayeredVolatileStatus> allLayeredVolatileStatuses;

        /// <summary>
        /// Retrieve all item categories in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        public static List<ItemCategory> GetAllItemCategories() =>
            allItemCategories ??= AssetManagementUtils.FindAssetsByType<ItemCategory>();

        /// <summary>
        /// Cached list of all ItemCategories.
        /// </summary>
        private static List<ItemCategory> allItemCategories;

        /// <summary>
        /// Retrieve all items in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        public static List<Item> GetAllItems() => allItems ??= AssetManagementUtils.FindAssetsByType<Item>();

        /// <summary>
        /// Cached list of all Items.
        /// </summary>
        private static List<Item> allItems;

        /// <summary>
        /// Retrieve all holdable items in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        public static List<Item> GetAllHoldableItems() =>
            allHoldableItems ??= GetAllItems().Where(item => item.CanBeHeld).ToList();

        /// <summary>
        /// Cached list of all HoldableItems.
        /// </summary>
        private static List<Item> allHoldableItems;

        /// <summary>
        /// Retrieve all items that can be used in battle, useful for inspectors.
        /// </summary>
        [UsedImplicitly]
        public static List<Item> GetAllUsableInBattleOnTargetItems() =>
            allUsableInBattleOnTargetItems ??= GetAllItems().Where(item => item.CanBeUsedInBattleOnTarget).ToList();

        /// <summary>
        /// Cached list of all UsableInBattleOnTargetItems.
        /// </summary>
        private static List<Item> allUsableInBattleOnTargetItems;

        /// <summary>
        /// Retrieve all side statuses in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        public static List<SideStatus> GetAllSideStatuses() =>
            allSideStatuses ??= AssetManagementUtils.FindAssetsByType<SideStatus>();

        /// <summary>
        /// Cached list of all SideStatuses.
        /// </summary>
        private static List<SideStatus> allSideStatuses;

        /// <summary>
        /// Retrieve all layered side statuses in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        public static List<LayeredSideStatus> GetAllLayeredSideStatuses() =>
            allLayeredSideStatuses ??= AssetManagementUtils.FindAssetsByType<LayeredSideStatus>();

        /// <summary>
        /// Cached list of all LayeredSideStatuses.
        /// </summary>
        private static List<LayeredSideStatus> allLayeredSideStatuses;

        /// <summary>
        /// Retrieve all Terrain in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        public static List<Terrain> GetAllTerrains() =>
            allTerrains ??= AssetManagementUtils.FindAssetsByType<Terrain>();

        /// <summary>
        /// Cached list of all Terrains.
        /// </summary>
        private static List<Terrain> allTerrains;

        /// <summary>
        /// Retrieve all balls in the database, useful for inspectors.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        public static List<Ball> GetAllBalls() => allBalls ??= AssetManagementUtils.FindAssetsByType<Ball>();

        /// <summary>
        /// Cached list of all Balls.
        /// </summary>
        private static List<Ball> allBalls;

        /// <summary>
        /// Retrieve all character types in the database.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        public static List<CharacterType> GetAllCharacterTypes() =>
            allCharacterTypes ??= AssetManagementUtils.FindAssetsByType<CharacterType>();

        /// <summary>
        /// Cached list of all CharacterTypes.
        /// </summary>
        private static List<CharacterType> allCharacterTypes;

        /// <summary>
        /// Retrieve all Ribbons in the database.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        public static List<Ribbon> GetAllRibbons() => allRibbons ??= AssetManagementUtils.FindAssetsByType<Ribbon>();

        /// <summary>
        /// Cached list of all Ribbons.
        /// </summary>
        private static List<Ribbon> allRibbons;

        /// <summary>
        /// Retrieve all badges in the database.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        public static List<Badge> GetAllBadges() => allBadges ??= AssetManagementUtils.FindAssetsByType<Badge>();

        /// <summary>
        /// Cached list of all Badges.
        /// </summary>
        private static List<Badge> allBadges;
        
        /// <summary>
        /// Retrieve all SceneTags in the database.
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        protected List<SceneTag> GetAllSceneTags() => allSceneTags ??= AssetManagementUtils.FindAssetsByType<SceneTag>();

        /// <summary>
        /// Cached list of all SceneTags.
        /// </summary>
        private List<SceneTag> allSceneTags;

        // ReSharper restore StaticMemberInGenericType
        #endif
    }
}