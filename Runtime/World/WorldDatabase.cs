using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.Quests;
using Varguiniano.YAPU.Runtime.UI.Dex;
using Varguiniano.YAPU.Runtime.World.Encounters;
using WhateverDevs.Core.Behaviours;
using Zenject;

#if UNITY_EDITOR
using WhateverDevs.Core.Editor.Utils;
using UnityEditor;
#endif

namespace Varguiniano.YAPU.Runtime.World
{
    /// <summary>
    /// Scriptable that stores all the world locations of the game.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Maps/WorldDatabase", fileName = "WorldDatabase")]
    public class WorldDatabase : WhateverScriptable<WorldDatabase>
    {
        /// <summary>
        /// Reference to the monster database.
        /// </summary>
        [Inject]
        private MonsterDatabaseInstance monsterDatabase;

        /// <summary>
        /// Load the lookup tables that link all saved objects to their names.
        /// </summary>
        public void LoadLookupTables()
        {
            Logger.Info("Initializing lookup tables.");

            InitScenesLookupTable();
            InitRegionsLookupTable();
            InitQuestsLookupTable();
            InitEncountersLookupTable();

            Logger.Info("Finished initializing lookup tables.");
        }

        /// <summary>
        /// List of all scenes in the game.
        /// </summary>
        public List<SceneInfoAsset> Scenes;

        /// <summary>
        /// Lookup table for the scenes and their names.
        /// </summary>
        private Dictionary<string, SceneInfoAsset> scenesLookupTable;

        /// <summary>
        /// Get a Scene from the database by its name.
        /// </summary>
        /// <param name="sceneName">Name to check.</param>
        /// <returns>Found Scene, if any.</returns>
        public SceneInfoAsset GetSceneByName(string sceneName) =>
            !scenesLookupTable.ContainsKey(sceneName) ? null : scenesLookupTable[sceneName];

        /// <summary>
        /// Initialize the Scenes lookup table.
        /// </summary>
        private void InitScenesLookupTable()
        {
            scenesLookupTable = new Dictionary<string, SceneInfoAsset>();

            foreach (SceneInfoAsset entry in Scenes) scenesLookupTable[entry.name] = entry;
        }

        /// <summary>
        /// List of all regions in the game.
        /// </summary>
        public List<Region> Regions;

        /// <summary>
        /// Lookup table for the Regions and their names.
        /// </summary>
        private Dictionary<string, Region> regionsLookupTable;

        /// <summary>
        /// Get a Region from the database by its name.
        /// </summary>
        /// <param name="sceneName">Name to check.</param>
        /// <returns>Found Region, if any.</returns>
        public Region GetRegionByName(string sceneName) =>
            !regionsLookupTable.ContainsKey(sceneName) ? null : regionsLookupTable[sceneName];

        /// <summary>
        /// Initialize the Regions lookup table.
        /// </summary>
        private void InitRegionsLookupTable()
        {
            regionsLookupTable = new Dictionary<string, Region>();

            foreach (Region entry in Regions) regionsLookupTable[entry.name] = entry;
        }

        /// <summary>
        /// List of all quests in the game.
        /// </summary>
        public List<Quest> Quests;

        /// <summary>
        /// Lookup table for the quests and their name.
        /// </summary>
        private Dictionary<string, Quest> questsLookupTable;

        /// <summary>
        /// Get a quest from the database by the hash of its name.
        /// </summary>
        /// <param name="questName">Name of the quest's name.</param>
        /// <returns>The quest if it's in the database.</returns>
        public Quest GetQuestByName(string questName) =>
            !questsLookupTable.ContainsKey(questName) ? null : questsLookupTable[questName];

        /// <summary>
        /// Initialize the quests lookup table.
        /// </summary>
        private void InitQuestsLookupTable()
        {
            questsLookupTable = new Dictionary<string, Quest>();

            foreach (Quest quest in Quests) questsLookupTable[quest.name] = quest;
        }

        /// <summary>
        /// Cached lookup table to display possible encounters in the dex.
        /// </summary>
        private Dictionary<(MonsterEntry, Form), SceneDexEncounterData> dexEncountersLookupTable;

        /// <summary>
        /// Init the lookup table for possible dex encounters.
        /// </summary>
        private void InitEncountersLookupTable()
        {
            dexEncountersLookupTable = new Dictionary<(MonsterEntry, Form), SceneDexEncounterData>();

            foreach (SceneInfoAsset scene in Scenes)
            {
                foreach (KeyValuePair<(MonsterEntry, Form), Dictionary<EncounterType, EncounterSetDexData>>
                             encounterTypeSlot in scene.GetPossibleDexEncounters())
                {
                    SceneDexEncounterData sceneData =
                        dexEncountersLookupTable.TryGetValue(encounterTypeSlot.Key, out SceneDexEncounterData value)
                            ? value
                            : new SceneDexEncounterData();

                    sceneData.Encounters[scene] = encounterTypeSlot.Value;

                    dexEncountersLookupTable[encounterTypeSlot.Key] = sceneData;
                }
            }
        }

        /// <summary>
        /// Get the dex encounter data for a specific monster and form.
        /// </summary>
        public SceneDexEncounterData GetDexEncounterDataForMonster(MonsterEntry monster, Form form)
        {
            if (form.IsShiny)
                form = monsterDatabase.GetFormEntries()
                                      .FirstOrDefault(candidate =>
                                                          candidate.HasShinyVersion && candidate.ShinyVersion == form);

            return dexEncountersLookupTable.ContainsKey((monster, form))
                       ? dexEncountersLookupTable[(monster, form)]
                       : null;
        }

        #if UNITY_EDITOR

        /// <summary>
        /// Update the database.
        /// </summary>
        [Button]
        public void UpdateAll()
        {
            Scenes = AssetManagementUtils.FindAssetsByType<SceneInfoAsset>();
            Regions = AssetManagementUtils.FindAssetsByType<Region>();
            Quests = AssetManagementUtils.FindAssetsByType<Quest>();

            EditorUtility.SetDirty(this);
        }

        #endif
    }
}