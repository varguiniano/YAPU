using UnityEngine;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.UI.Dex;
using Varguiniano.YAPU.Runtime.World.OutOfBattleWeathers;
using WhateverDevs.Core.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Map
{
    /// <summary>
    /// Controller for the panel that shows the encounter info of a monster in the map.
    /// </summary>
    public class MapEncounterInfo : HidableUiElement<MapEncounterInfo>
    {
        /// <summary>
        /// Reference to the monster icon.
        /// </summary>
        [SerializeField]
        private Image MonsterIcon;

        /// <summary>
        /// Reference to the dawn icon.
        /// </summary>
        [SerializeField]
        private HidableUiElement DawnIcon;

        /// <summary>
        /// Reference to the day icon.
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
        private HidableUiElement NoWeatherIcon;

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
        /// Set the data into the panel.
        /// </summary>
        public void SetData(MonsterEntry species, Form form, EncounterSetDexData encounterData)
        {
            DataByFormEntry formData = species[form];

            MonsterIcon.sprite = form.IsShiny ? formData.IconShiny : formData.Icon;

            SetTimes(encounterData);
            SetWeather(encounterData);
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
                NoWeatherIcon.Show();
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

                NoWeatherIcon.Show(availableByTime);
                SunnyIcon.Show(encounterData.AvailableWeathers.Contains(Sunny) || availableByTime);
                RainIcon.Show(encounterData.AvailableWeathers.Contains(Rain) || availableByTime);
                FogIcon.Show(encounterData.AvailableWeathers.Contains(Fog) || availableByTime);
                HailIcon.Show(encounterData.AvailableWeathers.Contains(Hail) || availableByTime);
                SandstormIcon.Show(encounterData.AvailableWeathers.Contains(Sandstorm) || availableByTime);
            }
        }
    }
}