using TMPro;
using UnityEngine;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.World;
using Varguiniano.YAPU.Runtime.World.Encounters;
using Varguiniano.YAPU.Runtime.World.OutOfBattleWeathers;
using WhateverDevs.Core.Runtime.DependencyInjection;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.Localization.Runtime.Ui;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Dex
{
    /// <summary>
    /// Button to display the encounter data for a monster in a scene.
    /// </summary>
    public class DexEncounterButton : VirtualizedMenuItem, IPlayerDataReceiver
    {
        /// <summary>
        /// Hider for encounter data.
        /// </summary>
        [SerializeField]
        private HidableUiElement EncounterDataHider;

        /// <summary>
        /// Hider for unknown scene.
        /// </summary>
        [SerializeField]
        private HidableUiElement UnknownSceneHider;

        /// <summary>
        /// Scene name.
        /// </summary>
        [SerializeField]
        private TMP_Text SceneName;

        /// <summary>
        /// Encounter type.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro EncounterType;

        /// <summary>
        /// Reference to the dawn icon.
        /// </summary>
        [SerializeField]
        private HidableUiElement DawnIcon;

        /// <summary>
        /// Reference to the dat icon.
        /// </summary>
        [SerializeField]
        private HidableUiElement DayIcon;

        /// <summary>
        /// Reference to the dusk icon.
        /// </summary>
        [SerializeField]
        private HidableUiElement DuskIcon;

        /// <summary>
        /// Reference to the night icon.
        /// </summary>
        [SerializeField]
        private HidableUiElement NightIcon;

        /// <summary>
        /// Reference to the no weather icon.
        /// </summary>
        [SerializeField]
        private HidableUiElement NowWeatherIcon;

        /// <summary>
        /// Reference to the sunny icon.
        /// </summary>
        [SerializeField]
        private HidableUiElement SunnyIcon;

        /// <summary>
        /// Reference to the rain icon.
        /// </summary>
        [SerializeField]
        private HidableUiElement RainIcon;

        /// <summary>
        /// Reference to the fog icon.
        /// </summary>
        [SerializeField]
        private HidableUiElement FogIcon;

        /// <summary>
        /// Reference to the hail icon.
        /// </summary>
        [SerializeField]
        private HidableUiElement HailIcon;

        /// <summary>
        /// Reference to the sandstorm icon.
        /// </summary>
        [SerializeField]
        private HidableUiElement SandstormIcon;

        /// <summary>
        /// Reference to the sunny weather.
        /// </summary>
        [SerializeField]
        private OutOfBattleWeather Sunny;

        /// <summary>
        /// Reference to the rain weather.
        /// </summary>
        [SerializeField]
        private OutOfBattleWeather Rain;

        /// <summary>
        /// Reference to the fog weather.
        /// </summary>
        [SerializeField]
        private OutOfBattleWeather Fog;

        /// <summary>
        /// Reference to the hail weather.
        /// </summary>
        [SerializeField]
        private OutOfBattleWeather Hail;

        /// <summary>
        /// Reference to the sandstorm weather.
        /// </summary>
        [SerializeField]
        private OutOfBattleWeather Sandstorm;

        /// <summary>
        /// Is this scene known?
        /// </summary>
        public bool SceneKnown { get; private set; }

        /// <summary>
        /// Reference to the global game data.
        /// </summary>
        [Inject]
        private GlobalGameData globalGameData;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Set the data into the button.
        /// </summary>
        public void UpdateData(SceneInfoAsset scene, EncounterType encounterType, EncounterSetDexData encounterData)
        {
            SceneName.SetText(scene.GetLocalizedRegionPlusSceneName(localizer));
            EncounterType.SetValue(encounterType.ToLocalizableString());

            SetTimes(encounterData);
            SetWeather(encounterData);

            SceneKnown = globalGameData.VisitedScenes.Contains(scene);

            EncounterDataHider.Show(SceneKnown);
            UnknownSceneHider.Show(!SceneKnown);
        }

        /// <summary>
        /// Set up the time icons.
        /// </summary>
        private void SetTimes(EncounterSetDexData encounterData)
        {
            if (encounterData.AvailableAtAnyTime)
            {
                DawnIcon.Show();
                DayIcon.Show();
                NightIcon.Show();
                DuskIcon.Show();
            }
            else
            {
                bool availableByWeather = encounterData.AvailableMoments.Count == 0
                                       && encounterData.AvailableWeathers.Count > 0;

                DawnIcon.Show(encounterData.AvailableMoments.Contains(DayMoment.Dawn) || availableByWeather);
                DayIcon.Show(encounterData.AvailableMoments.Contains(DayMoment.Day) || availableByWeather);
                NightIcon.Show(encounterData.AvailableMoments.Contains(DayMoment.Night) || availableByWeather);
                NightIcon.Show(encounterData.AvailableMoments.Contains(DayMoment.Night) || availableByWeather);
            }
        }

        /// <summary>
        /// Wet up the weather icons.
        /// </summary>
        private void SetWeather(EncounterSetDexData encounterData)
        {
            if (encounterData.AvailableOnAnyWeather)
            {
                NowWeatherIcon.Show();
                SunnyIcon.Show();
                RainIcon.Show();
                FogIcon.Show();
                HailIcon.Show();
                SandstormIcon.Show();
            }
            else
            {
                bool availableByTime = encounterData.AvailableWeathers.Count == 0
                                    && encounterData.AvailableMoments.Count > 0;

                NowWeatherIcon.Show(availableByTime);
                SunnyIcon.Show(encounterData.AvailableWeathers.Contains(Sunny) || availableByTime);
                RainIcon.Show(encounterData.AvailableWeathers.Contains(Rain) || availableByTime);
                FogIcon.Show(encounterData.AvailableWeathers.Contains(Fog) || availableByTime);
                HailIcon.Show(encounterData.AvailableWeathers.Contains(Hail) || availableByTime);
                SandstormIcon.Show(encounterData.AvailableWeathers.Contains(Sandstorm) || availableByTime);
            }
        }

        /// <summary>
        /// Factory class used for instantiation.
        /// </summary>
        public class Factory : GameObjectFactory<DexEncounterButton>
        {
        }
    }
}