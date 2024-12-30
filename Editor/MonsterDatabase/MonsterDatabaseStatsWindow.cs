using System.Linq;
using UnityEditor;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.World;
using WhateverDevs.Core.Editor.Utils;

namespace Varguiniano.YAPU.Editor.MonsterDatabase
{
    /// <summary>
    /// Editor window to display monster database stats.
    /// </summary>
    public class MonsterDatabaseStatsWindow : EditorWindow
    {
        /// <summary>
        /// Flag to know if the stats have been loaded at least once.
        /// </summary>
        private bool loaded;

        /// <summary>
        /// Number of monster species.
        /// </summary>
        private int monsterSpecies;

        /// <summary>
        /// Number of monster species including forms, shinies and gender differences.
        /// </summary>
        private int monsterSpeciesWithFormsShiniesAndGenderDifferences;

        /// <summary>
        /// Number of abilities.
        /// </summary>
        private int abilities;

        /// <summary>
        /// Number of moves.
        /// </summary>
        private int moves;

        /// <summary>
        /// Number of items.
        /// </summary>
        private int items;

        /// <summary>
        /// Number of characters types.
        /// </summary>
        private int characterTypes;

        /// <summary>
        /// Number of tiles.
        /// </summary>
        private int tiles;

        /// <summary>
        /// Initialize the window.
        /// </summary>
        [MenuItem("YAPU/Database Stats")]
        private static void Init()
        {
            MonsterDatabaseStatsWindow window =
                (MonsterDatabaseStatsWindow)GetWindow(typeof(MonsterDatabaseStatsWindow),
                                                      false,
                                                      "YAPU Database Stats");

            window.Show();
        }

        /// <summary>
        /// Paint the checklist.
        /// </summary>
        private void OnGUI()
        {
            GUIStyle style = GUI.skin.GetStyle("label");
            style.richText = true;

            int defaultSize = style.fontSize;

            style.fontSize = 24;

            GUILayout.Label(titleContent.text, style);

            style.fontSize = defaultSize;

            if (EditorApplication.isCompiling)
                EditorGUILayout.HelpBox("Please wait for compilation to end...", MessageType.Info);
            else
            {
                if (!loaded) LoadStats();

                HorizontalLine();

                EditorGUILayout.LabelField("Monsters: " + monsterSpecies + ".");

                EditorGUILayout.LabelField("Including forms, shinies and gender differences: "
                                         + monsterSpeciesWithFormsShiniesAndGenderDifferences
                                         + ".");

                HorizontalLine();

                EditorGUILayout.LabelField("Abilities: " + abilities + ".");
                EditorGUILayout.LabelField("Moves: " + moves + ".");

                HorizontalLine();
                EditorGUILayout.LabelField("Items: " + items + ".");

                HorizontalLine();
                EditorGUILayout.LabelField("Characters: " + characterTypes + ".");

                HorizontalLine();
                EditorGUILayout.LabelField("Tiles: " + tiles + ".");

                HorizontalLine();

                if (GUILayout.Button("Refresh")) LoadStats();
            }
        }

        /// <summary>
        /// Load the database stats.
        /// </summary>
        private void LoadStats()
        {
            MonsterDatabaseInstance database = AssetManagementUtils.FindAssetsByType<MonsterDatabaseInstance>().First();
            TileData tileData = AssetManagementUtils.FindAssetsByType<TileData>().First();

            monsterSpecies = 0;
            monsterSpeciesWithFormsShiniesAndGenderDifferences = 0;

            foreach (MonsterEntry entry in database.GetMonsterEntries(false))
            {
                monsterSpecies++;

                foreach (Form form in entry.AvailableForms)
                {
                    monsterSpeciesWithFormsShiniesAndGenderDifferences++;

                    bool genderDifference = entry[form].HasMaleMaterialOverride;

                    if (genderDifference) monsterSpeciesWithFormsShiniesAndGenderDifferences++;

                    if (!form.HasShinyVersion) continue;

                    monsterSpeciesWithFormsShiniesAndGenderDifferences++;
                    if (genderDifference) monsterSpeciesWithFormsShiniesAndGenderDifferences++;
                }
            }

            abilities = database.GetAbilities(false).Count;
            moves = database.GetMoves(false).Count;
            items = database.GetItems(false).Count;

            characterTypes = AssetManagementUtils.FindAssetsByType<CharacterType>().Count;

            tiles = tileData.TypeCount;

            loaded = true;
        }

        /// <summary>
        /// Paint a horizontal line.
        /// </summary>
        private static void HorizontalLine() => EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }
}