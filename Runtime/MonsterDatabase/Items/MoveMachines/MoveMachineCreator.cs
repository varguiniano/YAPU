using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.OnTarget;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using WhateverDevs.Core.Editor.Utils;
#endif

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.MoveMachines
{
    /// <summary>
    /// Tool to generate all move machines from the moves.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/MoveMachineCreator", fileName = "MoveMachineCreator")]
    public class MoveMachineCreator : MonsterDatabaseScriptable<MoveMachineCreator>
    {
        /// <summary>
        /// Path to create the items on.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [FolderPath]
        [SerializeField]
        private string TargetPath;

        /// <summary>
        /// The TMs category.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllItemCategories))]
        #endif
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private ItemCategory TMCategory;

        /// <summary>
        /// Localized name for TMs.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private string TMLocalizedName;

        /// <summary>
        /// Localized name for TRs.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private string TRLocalizedName;

        /// <summary>
        /// Default price for these items.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private uint TMDefaultPrice;

        /// <summary>
        /// Default price for these items.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private uint TRDefaultPrice;

        /// <summary>
        /// Sprites to use for each type when creating TMs.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private SerializedDictionary<MonsterType, Sprite> TMSpritesPerType;

        /// <summary>
        /// Sprites to use for each type when creating TRs.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private SerializedDictionary<MonsterType, Sprite> TRSpritesPerType;

        /// <summary>
        /// Reference to the move machine effect.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private MoveMachineEffect MoveMachineEffect;

        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        [FoldoutGroup("Configuration")]
        [SerializeField]
        private YAPUSettings YAPUSettings;

        #if UNITY_EDITOR

        /// <summary>
        /// Title to use in the progress bar.
        /// </summary>
        private const string ProgressBarTitle = "Generating move machines";

        /// <summary>
        /// Extension for asset files.
        /// </summary>
        private const string AssetExtension = ".asset";

        /// <summary>
        /// Generate the machines for each move.
        /// </summary>
        [Button]
        [PropertyOrder(-1)]
        private void GenerateMachines()
        {
            try
            {
                EditorUtility.DisplayProgressBar(ProgressBarTitle, "Looking for monster database", .1f);

                MonsterDatabaseInstance database =
                    AssetManagementUtils.FindAssetsByType<MonsterDatabaseInstance>().First();

                database.UpdateAll();

                List<Move> moves = database.GetMoves(false);

                for (int i = 0; i < moves.Count; i++)
                {
                    Move move = moves[i];
                    string tmObjectName = "TM-" + move.name;
                    string trObjectName = "TR-" + move.name;

                    if (!File.Exists(TargetPath + "/" + tmObjectName + AssetExtension))
                    {
                        Logger.Info("No TM found for " + move.name + ", creating one.");

                        EditorUtility.DisplayProgressBar(ProgressBarTitle, tmObjectName, (float)i / moves.Count);

                        MoveMachine moveMachine = CreateInstance<MoveMachine>();

                        moveMachine.DocsURL = "https://bulbapedia.bulbagarden.net/wiki/TM";
                        moveMachine.Move = move;
                        moveMachine.IsSpentOnUse = false;

                        moveMachine.LocalizableName = TMLocalizedName;
                        moveMachine.LocalizableDescription = "NotUsed";

                        moveMachine.ItemCategory = TMCategory;

                        moveMachine.DefaultPrice = TMDefaultPrice;
                        moveMachine.CanBeSold = true;

                        moveMachine.Icon = TMSpritesPerType[move.GetMoveType(null, YAPUSettings)];

                        moveMachine.UseOnTargetEffects.Add(MoveMachineEffect);

                        AssetDatabase.CreateAsset(moveMachine, TargetPath + "/" + tmObjectName + AssetExtension);

                        AssetDatabase.SaveAssets();
                    }

                    // ReSharper disable once InvertIf
                    if (!File.Exists(TargetPath + "/" + trObjectName + AssetExtension))
                    {
                        Logger.Info("No TR found for " + move.name + ", creating one.");

                        EditorUtility.DisplayProgressBar(ProgressBarTitle, trObjectName, (float)i / moves.Count);

                        MoveMachine moveMachine = CreateInstance<MoveMachine>();

                        moveMachine.DocsURL = "https://bulbapedia.bulbagarden.net/wiki/TM";
                        moveMachine.Move = move;
                        moveMachine.IsSpentOnUse = true;

                        moveMachine.LocalizableName = TRLocalizedName;
                        moveMachine.LocalizableDescription = "NotUsed";

                        moveMachine.ItemCategory = TMCategory;

                        moveMachine.DefaultPrice = TRDefaultPrice;
                        moveMachine.CanBeSold = true;

                        moveMachine.Icon = TRSpritesPerType[move.GetMoveType(null, YAPUSettings)];

                        moveMachine.UseOnTargetEffects.Add(MoveMachineEffect);

                        AssetDatabase.CreateAsset(moveMachine, TargetPath + "/" + trObjectName + AssetExtension);

                        AssetDatabase.SaveAssets();
                    }
                }

                EditorUtility.DisplayProgressBar(ProgressBarTitle, "Saving assets", .9f);

                database.UpdateAll();
                AssetDatabase.SaveAssets();
            }
            finally
            {
                EditorUtility.ClearProgressBar();

                Logger.Info("Finished creating move machines.");
            }
        }

        #endif
    }
}