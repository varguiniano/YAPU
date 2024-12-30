using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Badges;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using Varguiniano.YAPU.Runtime.MonsterDatabase.EggGroups;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Natures;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Ribbons;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Localization.Runtime;
using Zenject;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Varguiniano.YAPU.Runtime.MonsterDatabase
{
    /// <summary>
    /// Instance of the monster data base that can be queried for data.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Instance", fileName = "MonsterDatabaseInstance")]
    public class MonsterDatabaseInstance : MonsterDatabaseScriptable<MonsterDatabaseInstance>
    {
        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Load the lookup tables that link all saved objects to their names.
        /// </summary>
        public void LoadLookupTables()
        {
            Logger.Info("Initializing lookup tables.");

            InitMonstersLookupTable();
            InitFormsLookupTable();
            InitMovesLookupTable();
            InitAbilitiesLookupTable();
            InitItemsLookupTable();
            InitCharacterTypesLookupTable();
            InitRibbonsLookupTable();
            InitBadgesLookupTable();

            Logger.Info("Finished initializing lookup tables.");
        }

        #if UNITY_EDITOR

        /// <summary>
        /// Update all the lists in the database.
        /// </summary>
        [Button]
        [PropertyOrder(-1)]
        public void UpdateAll()
        {
            InvalidateInspectorCaches();

            LoadAllMonsters();
            LoadAllForms();
            LoadAllTypes();
            LoadAllMoves();
            LoadAllAbilities();
            LoadAllStatus();
            LoadAllNatures();
            LoadAllEggGroups();
            LoadAllItems();
            LoadCharacterTypes();
            LoadRibbons();
            LoadBadges();

            EditorUtility.SetDirty(this);
        }

        #endif

        /// <summary>
        /// List of all monsters in the database.
        /// </summary>
        [TitleGroup("Monsters")]
        [SerializeField]
        private List<MonsterEntry> Monsters;

        /// <summary>
        /// Lookup table for the monsters and their names.
        /// </summary>
        private Dictionary<int, MonsterEntry> monstersLookupTable;

        /// <summary>
        /// Get all the monsters in the database.
        /// </summary>
        /// <param name="alphabetically">Ordered alphabetically for the current language?</param>
        /// <returns>A list of all the monster entries.</returns>
        public List<MonsterEntry> GetMonsterEntries(bool alphabetically = true) =>
            alphabetically ? Monsters.OrderBy(entry => localizer[entry.LocalizableName]).ToList() : Monsters;

        /// <summary>
        /// Get a monster from the database by its name hash.
        /// </summary>
        /// <param name="monsterHash">Hash to check.</param>
        /// <returns>Found monster, if any.</returns>
        public MonsterEntry GetMonsterByHash(int monsterHash) =>
            !monstersLookupTable.ContainsKey(monsterHash) ? null : monstersLookupTable[monsterHash];

        /// <summary>
        /// Mean size of the monsters in the database.
        /// </summary>
        public float MeanMonsterSize { get; private set; }

        /// <summary>
        /// Initialize the monster's lookup table.
        /// </summary>
        private void InitMonstersLookupTable()
        {
            monstersLookupTable = new Dictionary<int, MonsterEntry>();

            MeanMonsterSize = 0;

            int collectedSizes = 0;

            foreach (MonsterEntry entry in GetMonsterEntries())
            {
                monstersLookupTable[entry.name.GetHashCode()] = entry;

                foreach (Form form in entry.AvailableForms)
                {
                    MeanMonsterSize += entry[form].Height;
                    collectedSizes++;
                }
            }

            MeanMonsterSize /= collectedSizes;
        }

        #if UNITY_EDITOR

        /// <summary>
        /// Retrieve all monsters.
        /// </summary>
        [TitleGroup("Monsters")]
        [Button("Refresh")]
        private void LoadAllMonsters() => Monsters = GetAllMonsters().OrderBy(entry => entry.name).ToList();

        #endif

        /// <summary>
        /// List of all Forms in the database.
        /// </summary>
        [TitleGroup("Forms")]
        [SerializeField]
        private List<Form> Forms;

        /// <summary>
        /// Lookup table for the Forms and their names.
        /// </summary>
        private Dictionary<int, Form> formsLookupTable;

        /// <summary>
        /// Get all the Forms in the database.
        /// </summary>
        /// <param name="alphabetically">Ordered alphabetically for the current language?</param>
        /// <returns>A list of all the Form entries.</returns>
        public List<Form> GetFormEntries(bool alphabetically = true) =>
            alphabetically ? Forms.OrderBy(entry => localizer[entry.LocalizableName]).ToList() : Forms;

        /// <summary>
        /// Get a Form from the database by its name hash.
        /// </summary>
        /// <param name="formHash">Name to check.</param>
        /// <returns>Found Form, if any.</returns>
        public Form GetFormByHash(int formHash) =>
            !formsLookupTable.ContainsKey(formHash) ? null : formsLookupTable[formHash];

        /// <summary>
        /// Initialize the Form's lookup table.
        /// </summary>
        private void InitFormsLookupTable()
        {
            formsLookupTable = new Dictionary<int, Form>();

            foreach (Form entry in GetFormEntries()) formsLookupTable[entry.name.GetHashCode()] = entry;
        }

        #if UNITY_EDITOR

        /// <summary>
        /// Retrieve all Forms.
        /// </summary>
        [TitleGroup("Forms")]
        [Button("Refresh")]
        private void LoadAllForms() => Forms = GetAllForms().OrderBy(entry => entry.name).ToList();

        #endif

        /// <summary>
        /// List of all types in the database.
        /// </summary>
        [TitleGroup("Types")]
        [SerializeField]
        private List<MonsterType> Types;

        /// <summary>
        /// Get all the monster types in the database.
        /// </summary>
        public List<MonsterType> GetMonsterTypes() => Types;

        #if UNITY_EDITOR

        /// <summary>
        /// Retrieve all Types.
        /// </summary>
        [TitleGroup("Types")]
        [Button("Refresh")]
        private void LoadAllTypes() => Types = GetAllMonsterTypes().Where(type => type.Index).ToList();

        #endif

        /// <summary>
        /// List of all Moves in the database.
        /// </summary>
        [TitleGroup("Moves")]
        [SerializeField]
        private List<Move> Moves;

        /// <summary>
        /// Lookup table for the moves and their names.
        /// </summary>
        private Dictionary<int, Move> movesLookupTable;

        /// <summary>
        /// Get all the monster Moves in the database.
        /// <param name="alphabetically">Ordered alphabetically for the current language?</param>
        /// </summary>
        public List<Move> GetMoves(bool alphabetically = true) =>
            alphabetically ? Moves.OrderBy(entry => localizer[entry.LocalizableName]).ToList() : Moves;

        /// <summary>
        /// Get a move from the database by its name.
        /// </summary>
        /// <param name="moveHash">The hashed name.</param>
        /// <returns>Found move, if any.</returns>
        public Move GetMoveByHashKey(int moveHash) =>
            !movesLookupTable.ContainsKey(moveHash) ? null : movesLookupTable[moveHash];

        /// <summary>
        /// Initialize the monster's lookup table.
        /// </summary>
        private void InitMovesLookupTable()
        {
            movesLookupTable = new Dictionary<int, Move>();

            foreach (Move entry in GetMoves()) movesLookupTable[entry.name.GetHashCode()] = entry;
        }

        #if UNITY_EDITOR

        /// <summary>
        /// Retrieve all Moves.
        /// </summary>
        [TitleGroup("Moves")]
        [Button("Refresh")]
        private void LoadAllMoves() =>
            Moves = GetAllMoves().Where(move => move.ShouldIndex).OrderBy(move => move.name).ToList();

        #endif

        /// <summary>
        /// List of all Abilities in the database.
        /// </summary>
        [TitleGroup("Abilities")]
        [SerializeField]
        private List<Ability> Abilities;

        /// <summary>
        /// Lookup table for the abilities and their names.
        /// </summary>
        private Dictionary<int, Ability> abilitiesLookupTable;

        /// <summary>
        /// Get all the monster Abilities in the database.
        /// <param name="alphabetically">Ordered alphabetically for the current language?</param>
        /// </summary>
        public List<Ability> GetAbilities(bool alphabetically = true) =>
            alphabetically ? Abilities.OrderBy(entry => localizer[entry.LocalizableName]).ToList() : Abilities;

        /// <summary>
        /// Get an Ability from the database by its name hash.
        /// </summary>
        /// <param name="abilityHash">Hash to check.</param>
        /// <returns>Found Ability, if any.</returns>
        public Ability GetAbilityByHash(int abilityHash) =>
            !abilitiesLookupTable.ContainsKey(abilityHash) ? null : abilitiesLookupTable[abilityHash];

        /// <summary>
        /// Initialize the abilities lookup table.
        /// </summary>
        private void InitAbilitiesLookupTable()
        {
            abilitiesLookupTable = new Dictionary<int, Ability>();

            foreach (Ability entry in GetAbilities()) abilitiesLookupTable[entry.name.GetHashCode()] = entry;
        }

        #if UNITY_EDITOR

        /// <summary>
        /// Retrieve all Moves.
        /// </summary>
        [TitleGroup("Abilities")]
        [Button("Refresh")]
        private void LoadAllAbilities() => Abilities = GetAllAbilities().OrderBy(ability => ability.name).ToList();

        #endif

        /// <summary>
        /// List of all Status in the database.
        /// </summary>
        [TitleGroup("Status")]
        [SerializeField]
        private List<Status> Status;

        /// <summary>
        /// Get all the monster Status in the database.
        /// <param name="alphabetically">Ordered alphabetically for the current language?</param>
        /// </summary>
        public List<Status> GetStatus(bool alphabetically = true) =>
            alphabetically ? Status.OrderBy(entry => localizer[entry.LocalizableName]).ToList() : Status;

        #if UNITY_EDITOR

        /// <summary>
        /// Retrieve all Status.
        /// </summary>
        [TitleGroup("Status")]
        [Button("Refresh")]
        private void LoadAllStatus() => Status = GetAllStatuses().OrderBy(status => status.name).ToList();

        #endif

        /// <summary>
        /// List of all natures.
        /// </summary>
        [TitleGroup("Natures")]
        [SerializeField]
        private List<Nature> Natures;

        /// <summary>
        /// Get all the Natures in the database.
        /// <param name="alphabetically">Ordered alphabetically for the current language?</param>
        /// </summary>
        public List<Nature> GetNatures(bool alphabetically = true) =>
            alphabetically ? Natures.OrderBy(entry => localizer[entry.LocalizableName]).ToList() : Natures;

        /// <summary>
        /// Get a random nature from the database.
        /// </summary>
        /// <returns>A random nature.</returns>
        public Nature GetRandomNature() => Natures.Random();

        #if UNITY_EDITOR

        /// <summary>
        /// Load all natures in the database.
        /// </summary>
        [TitleGroup("Natures")]
        [Button("Refresh")]
        private void LoadAllNatures() => Natures = GetAllNatures().OrderBy(nature => nature.name).ToList();

        #endif

        /// <summary>
        /// List of all EggGroups in the database.
        /// </summary>
        [TitleGroup("EggGroups")]
        [SerializeField]
        private List<EggGroup> EggGroups;

        /// <summary>
        /// Get all the monster EggGroups in the database.
        /// <param name="alphabetically">Ordered alphabetically for the current language?</param>
        /// </summary>
        public List<EggGroup> GetEggGroups(bool alphabetically = true) =>
            alphabetically ? EggGroups.OrderBy(entry => localizer[entry.LocalizableName]).ToList() : EggGroups;

        #if UNITY_EDITOR

        /// <summary>
        /// Retrieve all Moves.
        /// </summary>
        [TitleGroup("EggGroups")]
        [Button("Refresh")]
        private void LoadAllEggGroups() => EggGroups = GetAllEggGroups().OrderBy(group => group.name).ToList();

        #endif

        /// <summary>
        /// List of all Items in the database.
        /// </summary>
        [TitleGroup("Items")]
        [SerializeField]
        private List<Item> Items;

        /// <summary>
        /// Lookup table for the items and their names.
        /// </summary>
        private Dictionary<int, Item> itemsLookupTable;

        /// <summary>
        /// Get all the items in the database.
        /// <param name="alphabetically">Ordered alphabetically for the current language?</param>
        /// </summary>
        public List<Item> GetItems(bool alphabetically = true) =>
            alphabetically ? Items.OrderBy(entry => entry.GetName(localizer)).ToList() : Items;

        /// <summary>
        /// Get an item from the database by its name.
        /// </summary>
        /// <param name="itemHash">Hash to check.</param>
        /// <returns>Found item, if any.</returns>
        public Item GetItemByHash(int itemHash) =>
            !itemsLookupTable.ContainsKey(itemHash) ? null : itemsLookupTable[itemHash];

        /// <summary>
        /// Initialize the item's lookup table.
        /// </summary>
        private void InitItemsLookupTable()
        {
            itemsLookupTable = new Dictionary<int, Item>();

            foreach (Item entry in GetItems()) itemsLookupTable[entry.name.GetHashCode()] = entry;
        }

        #if UNITY_EDITOR

        /// <summary>
        /// Retrieve all Items.
        /// </summary>
        [TitleGroup("Items")]
        [Button("Refresh")]
        private void LoadAllItems() => Items = GetAllItems().OrderBy(item => item.name).ToList();

        #endif

        /// <summary>
        /// List of all CharacterTypes in the database.
        /// </summary>
        [TitleGroup("Characters")]
        [SerializeField]
        private List<CharacterType> CharacterTypes;

        /// <summary>
        /// Lookup table for the CharacterTypes and their names.
        /// </summary>
        private Dictionary<string, CharacterType> characterTypesLookupTable;

        /// <summary>
        /// Get all the CharacterTypes in the database.
        /// <param name="alphabetically">Ordered alphabetically for the current language?</param>
        /// </summary>
        public List<CharacterType> GetCharacterTypes(bool alphabetically = true) =>
            alphabetically
                ? CharacterTypes.OrderBy(entry => localizer[entry.LocalizableName]).ToList()
                : CharacterTypes;

        /// <summary>
        /// Get an CharacterType from the database by its name.
        /// </summary>
        /// <param name="characterTypeName">Name to check.</param>
        /// <returns>Found CharacterType, if any.</returns>
        public CharacterType GetCharacterTypeByName(string characterTypeName) =>
            !characterTypesLookupTable.ContainsKey(characterTypeName)
                ? null
                : characterTypesLookupTable[characterTypeName];

        /// <summary>
        /// Initialize the CharacterTypes lookup table.
        /// </summary>
        private void InitCharacterTypesLookupTable()
        {
            characterTypesLookupTable = new Dictionary<string, CharacterType>();

            foreach (CharacterType entry in GetCharacterTypes()) characterTypesLookupTable[entry.name] = entry;
        }

        #if UNITY_EDITOR

        /// <summary>
        /// Retrieve all Items.
        /// </summary>
        [TitleGroup("Characters")]
        [Button("Refresh")]
        private void LoadCharacterTypes() =>
            CharacterTypes = GetAllCharacterTypes().OrderBy(item => item.name).ToList();

        #endif

        /// <summary>
        /// List of all Ribbons in the database.
        /// </summary>
        [TitleGroup("Ribbons")]
        [SerializeField]
        private List<Ribbon> Ribbons;

        /// <summary>
        /// Lookup table for the Ribbons and their names.
        /// </summary>
        private Dictionary<string, Ribbon> ribbonsLookupTable;

        /// <summary>
        /// Get all the Ribbons in the database.
        /// <param name="alphabetically">Ordered alphabetically for the current language?</param>
        /// </summary>
        public List<Ribbon> GetRibbons(bool alphabetically = true) =>
            alphabetically
                ? Ribbons.OrderBy(entry => localizer[entry.LocalizableName]).ToList()
                : Ribbons;

        /// <summary>
        /// Get an Ribbon from the database by its name.
        /// </summary>
        /// <param name="ribbonName">Name to check.</param>
        /// <returns>Found Ribbon, if any.</returns>
        public Ribbon GetRibbonByName(string ribbonName) =>
            !ribbonsLookupTable.ContainsKey(ribbonName) ? null : ribbonsLookupTable[ribbonName];

        /// <summary>
        /// Initialize the CharacterTypes lookup table.
        /// </summary>
        private void InitRibbonsLookupTable()
        {
            ribbonsLookupTable = new Dictionary<string, Ribbon>();

            foreach (Ribbon entry in GetRibbons()) ribbonsLookupTable[entry.name] = entry;
        }

        #if UNITY_EDITOR

        /// <summary>
        /// Retrieve all Ribbons.
        /// </summary>
        [TitleGroup("Ribbons")]
        [Button("Refresh")]
        private void LoadRibbons() => Ribbons = GetAllRibbons().OrderBy(item => item.name).ToList();

        #endif

        /// <summary>
        /// List of all Badges in the database.
        /// </summary>
        [TitleGroup("Badges")]
        [SerializeField]
        private List<Badge> Badges;

        /// <summary>
        /// Lookup table for the Badges and their names.
        /// </summary>
        private Dictionary<string, Badge> badgesLookupTable;

        /// <summary>
        /// Get all the Badges in the database.
        /// <param name="alphabetically">Ordered alphabetically for the current language?</param>
        /// </summary>
        public List<Badge> GetBadges(bool alphabetically = true) =>
            alphabetically
                ? Badges.OrderBy(entry => localizer[entry.LocalizableName]).ToList()
                : Badges;

        /// <summary>
        /// Get an Badge from the database by its name.
        /// </summary>
        /// <param name="ribbonName">Name to check.</param>
        /// <returns>Found Badge, if any.</returns>
        public Badge GetBadgeByName(string ribbonName) =>
            !badgesLookupTable.ContainsKey(ribbonName) ? null : badgesLookupTable[ribbonName];

        /// <summary>
        /// Initialize the Badges lookup table.
        /// </summary>
        private void InitBadgesLookupTable()
        {
            badgesLookupTable = new Dictionary<string, Badge>();

            foreach (Badge entry in GetBadges()) badgesLookupTable[entry.name] = entry;
        }

        #if UNITY_EDITOR

        /// <summary>
        /// Retrieve all Badges.
        /// </summary>
        [TitleGroup("Badges")]
        [Button("Refresh")]
        private void LoadBadges() => Badges = GetAllBadges().OrderBy(item => item.name).ToList();

        #endif
    }
}