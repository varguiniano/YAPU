using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.UI.Map;
using Varguiniano.YAPU.Runtime.World;
using Varguiniano.YAPU.Runtime.World.Encounters;
using Varguiniano.YAPU.Runtime.World.OutOfBattleWeathers;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Dex
{
    /// <summary>
    /// Controller for the tab that shows the habitats monsters live in.
    /// </summary>
    public class HabitatTab : MonsterDexTab, IPlayerDataReceiver
    {
        /// <summary>
        /// Hider for a text to show when there are no habitats.
        /// </summary>
        [SerializeField]
        private HidableUiElement NoHabitatsText;

        /// <summary>
        /// Reference to the encounters menu.
        /// </summary>
        [SerializeField]
        private DexEncounterMenu EncounterMenu;

        /// <summary>
        /// Audio to play when entering the menu.
        /// </summary>
        [SerializeField]
        private AudioReference EnterMenuAudio;

        /// <summary>
        /// Reference to the tips shower.
        /// </summary>
        [SerializeField]
        private SingleDexTipsShower TipsShower;

        /// <summary>
        /// Reference to the player character.
        /// </summary>
        public PlayerCharacter PlayerCharacter { get; private set; }

        /// <summary>
        /// Current monster data.
        /// </summary>
        private (MonsterEntry Species, Form Form) monsterData;

        /// <summary>
        /// Reference to the world database.
        /// </summary>
        [Inject]
        private WorldDatabase worldDatabase;

        /// <summary>
        /// Reference to the audio manager.
        /// </summary>
        [Inject]
        private IAudioManager audioManager;

        /// <summary>
        /// Reference to the map scene launcher.
        /// </summary>
        [Inject]
        private MapSceneLauncher mapSceneLauncher;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Reference to the global game data.
        /// </summary>
        [Inject]
        private GlobalGameData globalGameData;

        /// <summary>
        /// Subscribe to menu events.
        /// </summary>
        private void OnEnable()
        {
            EncounterMenu.OnButtonSelected += OnEncounterSelected;
            EncounterMenu.OnBackSelected += OnBackSelected;
        }

        /// <summary>
        /// Unsubscribe from menu events.
        /// </summary>
        private void OnDisable()
        {
            EncounterMenu.OnButtonSelected -= OnEncounterSelected;
            EncounterMenu.OnBackSelected -= OnBackSelected;
        }

        /// <summary>
        /// Display the list of habitats.
        /// </summary>
        public override void SetData(MonsterDexEntry entry,
                                     FormDexEntry formEntry,
                                     MonsterGender gender,
                                     PlayerCharacter playerCharacter)
        {
            monsterData = (entry.Species, formEntry.Form);
            PlayerCharacter = playerCharacter;

            SceneDexEncounterData encounterData =
                worldDatabase.GetDexEncounterDataForMonster(entry.Species, formEntry.Form);

            NoHabitatsText.Show(encounterData == null);

            List<(SceneInfoAsset, EncounterType, EncounterSetDexData)> menuData = new();

            if (encounterData != null)
            {
                StringBuilder verboseEncounters = new("Encounters for ");
                verboseEncounters.Append(entry.Species.name);
                verboseEncounters.Append("-");
                verboseEncounters.AppendLine(formEntry.Form.name);

                foreach (KeyValuePair<SceneInfoAsset, Dictionary<EncounterType, EncounterSetDexData>> sceneSlot in
                         encounterData.Encounters)
                {
                    verboseEncounters.Append("-");
                    verboseEncounters.AppendLine(sceneSlot.Key.name);

                    foreach (KeyValuePair<EncounterType, EncounterSetDexData> typeSlot in sceneSlot.Value)
                    {
                        menuData.Add((sceneSlot.Key, typeSlot.Key, typeSlot.Value));

                        verboseEncounters.Append("--");
                        verboseEncounters.AppendLine(typeSlot.Key.ToString());

                        if (typeSlot.Value.AvailableAtAnyTime)
                        {
                            verboseEncounters.Append("---");
                            verboseEncounters.AppendLine("Available at any time.");
                        }
                        else if (typeSlot.Value.AvailableAtSpecificTime)
                        {
                            verboseEncounters.Append("---");
                            verboseEncounters.AppendLine("Available times");

                            foreach (DayMoment moment in typeSlot.Value.AvailableMoments)
                            {
                                verboseEncounters.Append("----");
                                verboseEncounters.AppendLine(moment.ToString());
                            }
                        }

                        if (typeSlot.Value.AvailableOnAnyWeather)
                        {
                            verboseEncounters.Append("---");
                            verboseEncounters.AppendLine("Available under any weather.");
                        }
                        else if (typeSlot.Value.AvailableUnderSpecificWeather)
                        {
                            verboseEncounters.Append("---");
                            verboseEncounters.AppendLine("Available weathers");

                            foreach (OutOfBattleWeather weather in typeSlot.Value.AvailableWeathers)
                            {
                                verboseEncounters.Append("----");
                                verboseEncounters.AppendLine(weather.ToString());
                            }
                        }
                    }
                }

                Logger.Info(verboseEncounters);
            }

            List<(string, EncounterType)> addedEncounters = new();
            List<(SceneInfoAsset, EncounterType, EncounterSetDexData)> filteredData = new();

            // Filter out the encounters on scenes that have the same name and type.
            // Get the visited scenes first.
            foreach ((SceneInfoAsset Scene, EncounterType Type, EncounterSetDexData) data in
                     menuData.OrderByDescending(data => globalGameData.VisitedScenes.Contains(data.Item1)))
            {
                string sceneName = data.Scene.GetLocalizedRegionPlusSceneName(localizer);

                if (addedEncounters.Contains((sceneName, data.Type))) continue;

                filteredData.Add(data);
                addedEncounters.Add((sceneName, data.Type));
            }

            EncounterMenu.SetButtons(filteredData.OrderBy(data => data.Item1.GetLocalizedRegionPlusSceneName(localizer))
                                                 .ToList());
        }

        /// <summary>
        /// Enter the moves menu when the select button is pressed.
        /// </summary>
        public override void OnSelectPressedOnParentScreen()
        {
            if (EncounterMenu.Data.Count == 0)
            {
                DialogManager.ShowDialog("Dex/Habitat/NoHabitats");
                return;
            }

            audioManager.PlayAudio(EnterMenuAudio);
            EncounterMenu.RequestInput();
            TipsShower.SwitchToSubmenu(true, "Dex/Habitat/ShowMap");
        }

        /// <summary>
        /// Open the map when an encounter is selected.
        /// </summary>
        private void OnEncounterSelected(int index)
        {
            audioManager.PlayAudio(EnterMenuAudio);

            if (!((DexEncounterButton)EncounterMenu.CurrentSelectedButton).SceneKnown) return;

            Region regionToDisplay = EncounterMenu.Data[index].Item1.Region;

            List<(SceneInfoAsset, EncounterType, EncounterSetDexData)> encountersToDisplay =
                EncounterMenu.Data.Where(candidate =>
                                             candidate.Item1.Region == regionToDisplay
                                          && globalGameData.VisitedScenes.Contains(candidate.Item1))
                             .ToList();

            mapSceneLauncher.ShowMap(PlayerCharacter, monsterData.Species, monsterData.Form, encountersToDisplay);
        }

        /// <summary>
        /// Called when back is selected in the menu.
        /// </summary>
        private void OnBackSelected()
        {
            EncounterMenu.ReleaseInput();
            TipsShower.SwitchToGeneral();
        }
    }
}