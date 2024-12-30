using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.Saves;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Core.Runtime.Serialization;
using Zenject;

namespace Varguiniano.YAPU.Runtime.GameFlow
{
    /// <summary>
    /// Class in charge of the time management of the game.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/GameFlow/TimeManager", fileName = "TimeManager")]
    [Serializable]
    public class TimeManager : SavableObject
    {
        /// <summary>
        /// Seconds the game has been running.
        /// </summary>
        [ReadOnly]
        public double ElapsedGameSeconds;

        /// <summary>
        /// Current time of day.
        /// Measured between 0 and 1.
        /// </summary>
        [PropertyRange(0, 1)]
        public float DayProgress;

        /// <summary>
        /// Event raised each time the day ends.
        /// </summary>
        public Action OnDayEnded;

        /// <summary>
        /// Icon corresponding to each day moment.
        /// </summary>
        [FormerlySerializedAs("SayMomentIcons")]
        [SerializeField]
        private SerializableDictionary<DayMoment, Sprite> DayMomentIcons;

        /// <summary>
        /// Current moment of the day.
        /// </summary>
        public DayMoment DayMoment =>
            DayProgress switch
            {
                < DawnStart => DayMoment.Night,
                < DayStart => DayMoment.Dawn,
                < DuskStart => DayMoment.Day,
                < NightStart => DayMoment.Dusk,
                _ => DayMoment.Night
            };

        /// <summary>
        /// Retrieve the icon corresponding to the current day moment.
        /// </summary>
        public Sprite DayMomentIcon => DayMomentIcons[DayMoment];

        /// <summary>
        /// Get the hour in a string format.
        /// </summary>
        public string Hour => // 86400 is the amount of seconds in a day.
            TimeSpan.FromSeconds(DayProgress * 86400).ToString(@"hh\:mm");

        /// <summary>
        /// Time at which the dawn starts.
        /// </summary>
        public const float DawnStart = 5 / 24f;

        /// <summary>
        /// Time at which the day starts.
        /// </summary>
        public const float DayStart = 7 / 24f;

        /// <summary>
        /// Time at which the dusk starts.
        /// </summary>
        public const float DuskStart = 17 / 24f;

        /// <summary>
        /// Time at which the night starts.
        /// </summary>
        public const float NightStart = 19 / 24f;

        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        private YAPUSettings yapuSettings;

        /// <summary>
        /// Variable used to freeze the time of the day.
        /// </summary>
        private bool freezeDay;

        /// <summary>
        /// Initialize.
        /// </summary>
        [Inject]
        public void Construct(YAPUSettings settings, IBattleLauncher battleLauncher)
        {
            yapuSettings = settings;
            freezeDay = false;
            AppEventsListener.Instance.AppUpdate += OnAppUpdate;
            battleLauncher.SubscribeToBattleLaunched(() => freezeDay = true);
            battleLauncher.SubscribeToBattleEnded(_ => freezeDay = false);
        }

        /// <summary>
        /// Called on update.
        /// </summary>
        /// <param name="delta">Delta time.</param>
        private void OnAppUpdate(float delta)
        {
            ElapsedGameSeconds += delta;

            if (freezeDay) return;

            DayProgress += delta / yapuSettings.DayLengthInSeconds;

            while (DayProgress > 1)
            {
                DayProgress--;
                OnDayEnded?.Invoke();
            }
        }

        /// <summary>
        /// Switch to a new day moment.
        /// </summary>
        /// <param name="dayMoment">Moment to switch to.</param>
        public void SetNewDayMoment(DayMoment dayMoment) =>
            DayProgress = dayMoment switch
            {
                DayMoment.Night => NightStart,
                DayMoment.Dawn => DawnStart,
                DayMoment.Day => DayStart,
                DayMoment.Dusk => DuskStart,
                _ => DayProgress
            };

        /// <summary>
        /// Save to a persistable string.
        /// </summary>
        /// <param name="serializer">Serializer to use.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <returns>The serialized string.</returns>
        public override string SaveToText(ISerializer<string> serializer, PlayerCharacter playerCharacter) =>
            serializer.To(new SavableTimeManager(this));

        /// <summary>
        /// Load the object from a persistable text.
        /// </summary>
        /// <param name="serializer">Serializer to use when loading.</param>
        /// <param name="data">Text containing the data to load.</param>
        /// <param name="settings"></param>
        /// <param name="monsterDatabase">Reference to the monster database.</param>
        public override IEnumerator LoadFromText(ISerializer<string> serializer,
                                                 string data,
                                                 YAPUSettings settings,
                                                 MonsterDatabaseInstance monsterDatabase)
        {
            SavableTimeManager readData = serializer.From<SavableTimeManager>(data);

            yield return WaitAFrame;

            readData.LoadTimeManager(this);
        }

        /// <summary>
        /// Reset the game seconds.
        /// </summary>
        public override IEnumerator ResetSave()
        {
            ElapsedGameSeconds = 0;
            DayProgress = 0.5f;
            yield break;
        }

        /// <summary>
        /// Version of the Time manager that can be saved to a string.
        /// </summary>
        [Serializable]
        public class SavableTimeManager
        {
            /// <summary>
            /// Seconds the game has been running.
            /// </summary>
            public double ElapsedGameSeconds;

            /// <summary>
            /// Current time of day.
            /// Measured between 0 and 1.
            /// </summary>
            public float DayProgress;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="timeManager">Original</param>
            public SavableTimeManager(TimeManager timeManager)
            {
                ElapsedGameSeconds = timeManager.ElapsedGameSeconds;
                DayProgress = timeManager.DayProgress;
            }

            /// <summary>
            /// Load the data back into the time manager.
            /// </summary>
            /// <param name="timeManager">Time manager reference.</param>
            public void LoadTimeManager(TimeManager timeManager)
            {
                timeManager.ElapsedGameSeconds = ElapsedGameSeconds;
                timeManager.DayProgress = DayProgress;
            }
        }
    }
}