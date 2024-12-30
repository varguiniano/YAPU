using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Scenarios;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Saves;
using Varguiniano.YAPU.Runtime.World.Encounters;
using Varguiniano.YAPU.Runtime.World.OutOfBattleWeathers;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

#if UNITY_EDITOR
using WhateverDevs.Core.Editor.Utils;
#endif

namespace Varguiniano.YAPU.Runtime.World
{
    /// <summary>
    /// Behaviour that contains a reference to the scene's info.
    /// </summary>
    public class SceneInfo : WhateverBehaviour<SceneInfo>
    {
        /// <summary>
        /// Reference to the scene info asset.
        /// </summary>
        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
        public SceneInfoAsset Asset;

        /// <summary>
        /// Reference to the grid controller.
        /// </summary>
        public GridController Grid { get; private set; }

        #region Property Access

        /// <summary>
        /// Localizable name key for this scene.
        /// </summary>
        public string LocalizableNameKey => Asset.LocalizableNameKey;

        /// <summary>
        /// Reference to this asset's scene.
        /// </summary>
        public SceneReference Scene => Asset.Scene;

        /// <summary>
        /// Neighbours of this scene.
        /// </summary>
        public List<SceneInfoAsset> Neighbours => Asset.Neighbours;

        /// <summary>
        /// Background audio when in this scene.
        /// </summary>
        public AudioReference BackgroundMusic => Asset.BackgroundMusic;

        /// <summary>
        /// Is this scene affected by day/night light?
        /// </summary>
        public bool IsAffectedBySky => Asset.IsAffectedBySky;
        
        /// <summary>
        /// Does this scene have a default weather?
        /// </summary>
        public bool HasDefaultWeather => Asset.HasDefaultWeather;
        
        /// <summary>
        /// Default weather for this scene.
        /// </summary>
        public OutOfBattleWeather DefaultWeather => Asset.DefaultWeather;
        
        /// <summary>
        /// Weathers allowed even if it's not affected by the sky.
        /// </summary>
        public List<OutOfBattleWeather> AllowedWeathers => Asset.AllowedWeathers;

        /// <summary>
        /// Can the player open the storage here?
        /// </summary>
        public bool CanOpenStorage => Asset.CanOpenStorageHere;

        /// <summary>
        /// Can the player dig from here?
        /// </summary>
        public bool CanEscapeRopeFromHere => Asset.CanEscapeRopeFromHere;

        /// <summary>
        /// Can the player fly from here?
        /// </summary>
        public bool CanFlyFromHere => Asset.CanFlyFromHere;

        /// <summary>
        /// Can the player teleport from here?
        /// </summary>
        public bool CanTeleportFromHere => Asset.CanTeleportFromHere;

        /// <summary>
        /// Can the player fish here?
        /// </summary>
        public bool CanFishHere => Asset.CanFishHere;

        /// <summary>
        /// Is this scene a dungeon?
        /// </summary>
        public bool IsDungeon => Asset.IsDungeon;

        /// <summary>
        /// Position to teleport to when using an escape rope.
        /// </summary>
        public SceneLocation EscapeRopePosition => Asset.EscapeRopePosition;

        /// <summary>
        /// Is this a dark dungeon?
        /// </summary>
        public bool IsDark => Asset.IsDark;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Region this scene belongs to.
        /// </summary>
        public Region Region => Asset.Region;

        /// <summary>
        /// Battle scenarios to be used in each encounter type.
        /// </summary>
        public SerializableDictionary<EncounterType, BattleScenario> BattleScenariosPerEncounter =>
            Asset.BattleScenariosPerEncounter;

        /// <summary>
        /// Get the localized names of the region plus the scene name.
        /// </summary>
        /// <returns>The two names concatenated.</returns>
        public string GetLocalizedRegionPlusSceneName() => Asset.GetLocalizedRegionPlusSceneName(localizer);

        /// <summary>
        /// Does this scene have the given tag?
        /// </summary>
        /// <param name="sceneTag">Tag to check.</param>
        /// <returns>True if it has it.</returns>
        public bool HasTag(SceneTag sceneTag) => Asset.HasTag(sceneTag);

        /// <summary>
        /// Get the encounters possible for a specific type, moment and weather.
        /// </summary>
        /// <param name="encounterType">Encounter type to check.</param>
        /// <param name="moment">Current day moment.</param>
        /// <param name="weather">Current weather.</param>
        /// <returns>A list of all the possible encounters.</returns>
        public List<WildEncounter> GetWildEncounters(EncounterType encounterType,
                                                     DayMoment moment,
                                                     OutOfBattleWeather weather) =>
            Asset.GetWildEncounters(encounterType, moment, weather);

        /// <summary>
        /// Get a variable of the given type with the given reference.
        /// </summary>
        /// <param name="variable">Variable reference.</param>
        /// <typeparam name="T">Type of variable to get.</typeparam>
        /// <returns>The variable value.</returns>
        public T GetVariable<T>(GameVariableReference variable) => Asset.GetVariable<T>(variable);

        /// <summary>
        /// Get a variable of the given type with the given name.
        /// </summary>
        /// <param name="variableName">Name of the variable to get.</param>
        /// <typeparam name="T">Type of variable to get.</typeparam>
        /// <returns>The variable value.</returns>
        public T GetVariable<T>(string variableName) => Asset.GetVariable<T>(variableName);

        /// <summary>
        /// Set the value of a variable with the given name.
        /// </summary>
        /// <param name="variable">Reference of the variable to set.</param>
        /// <param name="value">Value to set.</param>
        /// <typeparam name="T">Type of the variable.</typeparam>
        public void SetVariable<T>(GameVariableReference variable, T value) => Asset.SetVariable(variable, value);

        /// <summary>
        /// Set the value of a variable with the given name.
        /// </summary>
        /// <param name="variableName">Name of the variable to set.</param>
        /// <param name="value">Value to set.</param>
        /// <typeparam name="T">Type of the variable.</typeparam>
        public void SetVariable<T>(string variableName, T value) => Asset.SetVariable(variableName, value);

        #endregion

        /// <summary>
        /// Flag to mark that we are missing the scene manager.
        /// </summary>
        [InfoBox("Scene manager is not found in project! Create one first.", InfoMessageType.Error)]
        [ReadOnly]
        [ShowInInspector]
        [ShowIf(nameof(missingSceneManager))]
        private bool missingSceneManager;

        /// <summary>
        /// Reference to the scene manager.
        /// </summary>
        private SceneManager sceneManager;

        /// <summary>
        /// Flag to mark that the scene is not in the scene manager.
        /// </summary>
        [InfoBox("Scene is not added to the scene manager! Add it first.", InfoMessageType.Error)]
        [ReadOnly]
        [ShowInInspector]
        [ShowIf(nameof(missingSceneInSceneManager))]
        private bool missingSceneInSceneManager;

        /// <summary>
        /// Allow a grid to register itself to this scene info.
        /// </summary>
        /// <param name="grid">Grid that will register.</param>
        public void RegisterGrid(GridController grid)
        {
            if (Grid != null) Logger.Warn("There is already a grid registered to this scene, it will be overriden.");

            Grid = grid;
        }

        /// <summary>
        /// Compare with an info asset.
        /// </summary>
        /// <param name="asset">Asset to compare to.</param>
        /// <returns>True if this scene's info asset is the same.</returns>
        public bool Equals(SceneInfoAsset asset) => Asset == asset;

        #if UNITY_EDITOR

        /// <summary>
        /// Reset the asset reference.
        /// </summary>
        [Button("Reset info reference")]
        [FoldoutGroup("Debug")]
        private void ResetInfo()
        {
            Asset = null;
            InitializeAssetReference();
        }

        /// <summary>
        /// Different method for the button because Button attribute doesn't get along with the OnInspectorInit attribute.
        /// </summary>
        [Button("Rescan", ButtonSizes.Large)]
        [ShowIf(nameof(missingSceneManager))]
        private void InitializeAssetReferenceButton() => InitializeAssetReference();

        /// <summary>
        /// Open the scene manager.
        /// </summary>
        [Button("Scene Manager", ButtonSizes.Large)]
        [ShowIf(nameof(missingSceneInSceneManager))]
        private void OpenSceneManager() => Selection.activeObject = sceneManager;

        /// <summary>
        /// Initialize the asset reference for the first time.
        /// </summary>
        [OnInspectorInit]
        private void InitializeAssetReference()
        {
            if (Asset != null) return;

            sceneManager = null;

            EditorUtility.DisplayProgressBar("Creating scene info", "Searching for scene manager...", .5f);

            try
            {
                List<SceneManager> sceneManagers = AssetManagementUtils.FindAssetsByType<SceneManager>();

                if (sceneManagers.Count > 0) sceneManager = sceneManagers.First();
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            missingSceneManager = sceneManager == null;

            if (missingSceneManager) return;

            string currentScene = gameObject.scene.name;

            SceneInfoAsset asset = ScriptableObject.CreateInstance<SceneInfoAsset>();

            // ReSharper disable once PossibleNullReferenceException
            missingSceneInSceneManager = !sceneManager.SceneNamesList.Contains(currentScene);

            if (missingSceneInSceneManager) return;

            asset.Scene = new SceneReference { SceneName = currentScene };

            string path = gameObject.scene.path.Remove(gameObject.scene.path.Length - 6) + "Info.asset";

            AssetDatabase.CreateAsset(asset, path);

            AssetDatabase.SaveAssets();

            Asset = AssetDatabase.LoadAssetAtPath<SceneInfoAsset>(path);

            EditorUtility.SetDirty(this);

            AssetDatabase.SaveAssets();

            Logger.Info("Scene info created for scene " + currentScene + ", auto adding to the save game manager.");

            SavegameManager savegameManager;

            try
            {
                savegameManager = AssetManagementUtils.FindAssetsByType<SavegameManager>().First();
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            if (savegameManager == null)
            {
                Logger.Error("Couldn't find save game manager.");
                return;
            }

            savegameManager.SavegameObjects.Add(Asset);

            EditorUtility.SetDirty(savegameManager);

            AssetDatabase.SaveAssets();
        }
        #endif
    }
}