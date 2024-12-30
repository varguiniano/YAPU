using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Scenarios;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.Saves;
using Varguiniano.YAPU.Runtime.UI.Dex;
using Varguiniano.YAPU.Runtime.World.Encounters;
using Varguiniano.YAPU.Runtime.World.OutOfBattleWeathers;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.World
{
    /// <summary>
    /// Data class for a scene information.
    /// </summary>
    public class SceneInfoAsset : GameVariableHolder
    {
        /// <summary>
        /// Reference to this asset's scene.
        /// </summary>
        [ReadOnly]
        public SceneReference Scene;

        /// <summary>
        /// Localizable name key for this scene.
        /// </summary>
        [FoldoutGroup("Localization")]
        public string LocalizableNameKey;

        /// <summary>
        /// Scene neighbours.
        /// </summary>
        public List<SceneInfoAsset> Neighbours;

        /// <summary>
        /// Background audio when in this scene.
        /// </summary>
        [FoldoutGroup("Scenario")]
        public AudioReference BackgroundMusic;

        /// <summary>
        /// Is this scene affected by day/night light and weather?
        /// </summary>
        [FormerlySerializedAs("IsAffectedByDayNightLight")]
        [FoldoutGroup("Scenario")]
        [Tooltip("Both day-night light and weather.")]
        public bool IsAffectedBySky;

        /// <summary>
        /// Does this scene have a default weather?
        /// </summary>
        public bool HasDefaultWeather =>
            DefaultWeather != null && (IsAffectedBySky || AllowedWeathers.Contains(DefaultWeather));

        /// <summary>
        /// Default weather for this scene.
        /// </summary>
        [FoldoutGroup("Scenario")]
        public OutOfBattleWeather DefaultWeather;

        /// <summary>
        /// Weathers allowed even if it's not affected by the sky.
        /// For example, fog inside a cave.
        /// </summary>
        [FoldoutGroup("Scenario")]
        [HideIf(nameof(IsAffectedBySky))]
        [Tooltip("Weathers allowed even if it's not affected by the sky.\nFor example, fog inside a cave.")]
        public List<OutOfBattleWeather> AllowedWeathers;

        /// <summary>
        /// Can the player open the storage here?
        /// </summary>
        [FoldoutGroup("Scenario")]
        public bool CanOpenStorageHere = true;

        /// <summary>
        /// Can the player dig from here?
        /// </summary>
        [FoldoutGroup("Scenario")]
        public bool CanEscapeRopeFromHere;

        /// <summary>
        /// Position to teleport to when using an escape rope.
        /// </summary>
        [FoldoutGroup("Scenario")]
        [ShowIf(nameof(CanEscapeRopeFromHere))]
        public SceneLocation EscapeRopePosition;

        /// <summary>
        /// Can the player fly from here?
        /// </summary>
        [FoldoutGroup("Scenario")]
        public bool CanFlyFromHere;

        /// <summary>
        /// Can the player teleport from here?
        /// </summary>
        [FoldoutGroup("Scenario")]
        public bool CanTeleportFromHere = true;

        /// <summary>
        /// Can the player fish here?
        /// </summary>
        [FoldoutGroup("Scenario")]
        public bool CanFishHere;

        /// <summary>
        /// Is this scene a dungeon?
        /// </summary>
        [FoldoutGroup("Scenario")]
        public bool IsDungeon;

        /// <summary>
        /// Is this a dark dungeon?
        /// </summary>
        [FoldoutGroup("Scenario")]
        [ShowIf(nameof(IsDungeon))]
        public bool IsDark;

        /// <summary>
        /// Region this scene belongs to.
        /// </summary>
        [FoldoutGroup("Map")]
        public Region Region;

        /// <summary>
        /// Tags this scene has.
        /// </summary>
        [FoldoutGroup("Map")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllSceneTags))]
        #endif
        private List<SceneTag> Tags;

        /// <summary>
        /// Battle scenarios to be used in each encounter type.
        /// </summary>
        [FoldoutGroup("Encounters")]
        public SerializableDictionary<EncounterType, BattleScenario> BattleScenariosPerEncounter = new();

        /// <summary>
        /// Encounters that can show up for each encounter type.
        /// </summary>
        [FoldoutGroup("Encounters")]
        [SerializeField]
        private SerializableDictionary<EncounterType, WildEncountersSet> Encounters;

        /// <summary>
        /// Get the localized names of the region plus the scene name.
        /// </summary>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <returns>The two names concatenated.</returns>
        public string GetLocalizedRegionPlusSceneName(ILocalizer localizer) =>
            localizer[Region.LocalizableName] + " - " + localizer[LocalizableNameKey];

        /// <summary>
        /// Does this scene have the given tag?
        /// </summary>
        /// <param name="tag">Tag to check.</param>
        /// <returns>True if it has it.</returns>
        public bool HasTag(SceneTag tag) => Tags.Contains(tag);

        /// <summary>
        /// Get the encounters possible for a specific type, moment and weather.
        /// </summary>
        /// <param name="encounterType">Encounter type to check.</param>
        /// <param name="moment">Current day moment.</param>
        /// <param name="weather">Current weather.</param>
        /// <returns>A list of all the possible encounters.</returns>
        public List<WildEncounter> GetWildEncounters(EncounterType encounterType,
                                                     DayMoment moment,
                                                     OutOfBattleWeather weather)
        {
            if (Encounters == null) return new List<WildEncounter>();

            if (Encounters.TryGetValue(encounterType, out WildEncountersSet encountersSet) && encountersSet != null)
                return encountersSet.GetWildEncounters(moment, weather);

            return new List<WildEncounter>();
        }

        /// <summary>
        /// Get the dex encounter data for displaying.
        /// </summary>
        public Dictionary<(MonsterEntry, Form), Dictionary<EncounterType, EncounterSetDexData>>
            GetPossibleDexEncounters()
        {
            Dictionary<(MonsterEntry, Form), Dictionary<EncounterType, EncounterSetDexData>> encounters = new();

            foreach (KeyValuePair<EncounterType, WildEncountersSet> encounterSlot in Encounters)
            {
                if (encounterSlot.Value == null) continue;

                foreach (KeyValuePair<(MonsterEntry, Form), EncounterSetDexData> setSlot in encounterSlot.Value
                            .GetPossibleDexEncounters())
                {
                    if (!encounters.ContainsKey(setSlot.Key))
                        encounters[setSlot.Key] = new Dictionary<EncounterType, EncounterSetDexData>();

                    encounters[setSlot.Key][encounterSlot.Key] = setSlot.Value;
                }
            }

            return encounters;
        }

        #if UNITY_EDITOR

        /// <summary>
        /// Initialize the inspector.
        /// </summary>
        [OnInspectorInit]
        protected override void InspectorInit()
        {
            base.InspectorInit();

            if (LocalizableNameKey.IsNullEmptyOrWhiteSpace()) LocalizableNameKey = "Scenes/" + name + "/Name";

            foreach (EncounterType encounterType in Utils.GetAllItems<EncounterType>())
                if (!BattleScenariosPerEncounter.ContainsKey(encounterType))
                    BattleScenariosPerEncounter[encounterType] = null;

            foreach (EncounterType encounterType in Utils.GetAllItems<EncounterType>())
                if (!Encounters.ContainsKey(encounterType))
                    Encounters[encounterType] = null;
        }

        #endif
    }
}