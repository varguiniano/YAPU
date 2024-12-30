using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.UI.Dex;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.World;
using Varguiniano.YAPU.Runtime.World.Encounters;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Map
{
    /// <summary>
    /// Scriptable in charge of launching the map scene.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Maps/Launcher", fileName = "MapSceneLauncher")]
    public class MapSceneLauncher : WhateverScriptable<MapSceneLauncher>
    {
        /// <summary>
        /// Reference to the map scene.
        /// </summary>
        [SerializeField]
        private SceneReference MapScene;

        /// <summary>
        /// Reference tot he scene manager.
        /// </summary>
        [Inject]
        private ISceneManager sceneManager;

        /// <summary>
        /// Reference to the input manager.
        /// </summary>
        [Inject]
        private IInputManager inputManager;

        /// <summary>
        /// Reference to the player character.
        /// </summary>
        private PlayerCharacter playerCharacterReference;

        /// <summary>
        /// Encounters to display for a specific monster.
        /// </summary>
        private ((MonsterEntry Species, Form Form), List<(SceneInfoAsset, EncounterType, EncounterSetDexData)>)
            encounters;

        /// <summary>
        /// Current objective of the current quest.
        /// </summary>
        private SceneInfoAsset currentQuestObjective;

        /// <summary>
        /// Show the map.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="monsterSpecies">Monster type to display.</param>
        /// <param name="form">Form to display.</param>
        /// <param name="encounterData">Display the encounters of a specific monster.</param>
        /// <param name="currentObjective">Current objective of the current quest.</param>
        public void ShowMap(PlayerCharacter playerCharacter,
                            MonsterEntry monsterSpecies = null,
                            Form form = null,
                            List<(SceneInfoAsset, EncounterType, EncounterSetDexData)>
                                encounterData = null,
                            SceneInfoAsset currentObjective = null)
        {
            playerCharacterReference = playerCharacter;
            encounters = ((monsterSpecies, form), encounterData);
            currentQuestObjective = currentObjective;

            inputManager.BlockInput();

            TransitionManager.BlackScreenFadeIn(.1f);
            DialogManager.ShowLoadingIcon();

            // Make sure we wait second for the fade in without being a coroutine.
            DOVirtual.DelayedCall(.1f,
                                  () => sceneManager.LoadScene(MapScene,
                                                               _ =>
                                                               {
                                                               },
                                                               success =>
                                                               {
                                                                   if (!success)
                                                                       Logger.Error("Error loading the map scene.");
                                                               }));
        }

        /// <summary>
        /// Close the map.
        /// </summary>
        public IEnumerator CloseMap()
        {
            if (!sceneManager.LoadedScenes.Contains(MapScene.SceneName)) yield break;

            bool unloaded = false;

            sceneManager.UnloadScene(MapScene,
                                     _ =>
                                     {
                                     },
                                     success =>
                                     {
                                         if (success)
                                             unloaded = true;
                                         else
                                             Logger.Error("Error unloading the map scene.");
                                     });

            yield return new WaitUntil(() => unloaded);

            yield return TransitionManager.BlackScreenFadeOutRoutine(.1f);
            DialogManager.ShowLoadingIcon(false);

            inputManager.BlockInput(false);
        }

        /// <summary>
        /// Retrieve the reference to the player character.
        /// </summary>
        public PlayerCharacter GetPlayerCharacter() => playerCharacterReference;

        /// <summary>
        /// Retrieve the encounters to display.
        /// </summary>
        public ((MonsterEntry Species, Form Form), List<(SceneInfoAsset, EncounterType, EncounterSetDexData)>)
            GetEncounters() =>
            encounters;

        /// <summary>
        /// Get the current quest objective.
        /// </summary>
        public SceneInfoAsset GetCurrentQuestObjective() => currentQuestObjective;
    }
}