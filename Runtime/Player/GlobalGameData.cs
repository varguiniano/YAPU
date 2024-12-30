using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Badges;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.Saves;
using Varguiniano.YAPU.Runtime.World;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Core.Runtime.Serialization;
using Zenject;
using Version = WhateverDevs.Core.Runtime.Build.Version;

namespace Varguiniano.YAPU.Runtime.Player
{
    /// <summary>
    /// Class that stores data for the whole game.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Player/GlobalGameData", fileName = "GlobalGameData")]
    public class GlobalGameData : GameVariableHolder
    {
        /// <summary>
        /// Date at which the game was last save.
        /// </summary>
        [FoldoutGroup("Versioning")]
        [ReadOnly]
        public string LastSaveDate;

        /// <summary>
        /// Version of the game when the game was last saved.
        /// </summary>
        [FoldoutGroup("Versioning")]
        [ReadOnly]
        public string LastGameVersion;

        /// <summary>
        /// Version of YAPU when the game was last saved.
        /// </summary>
        [FoldoutGroup("Versioning")]
        [ReadOnly]
        public string LastYAPUVersion;

        /// <summary>
        /// Version of the YAPU assets when the game was last saved.
        /// </summary>
        [FoldoutGroup("Versioning")]
        [ReadOnly]
        public string LastYAPUAssetsVersion;

        /// <summary>
        /// Difficulty for this game.
        /// </summary>
        [FoldoutGroup("Game Options")]
        public GameDifficulty GameDifficulty;
        
        /// <summary>
        /// Difficulty for catching.
        /// </summary>
        [FoldoutGroup("Game Options")]
        public CatchDifficulty CatchDifficulty;

        /// <summary>
        /// Does the player have the dex?
        /// </summary>
        [FoldoutGroup("General")]
        public bool HasDex;

        /// <summary>
        /// Last location in which the player healed.
        /// </summary>
        [FoldoutGroup("General")]
        public SceneLocation LastHealLocation;

        /// <summary>
        /// Last location in which the player was.
        /// </summary>
        [FoldoutGroup("Player Location")]
        public SceneLocation LastPlayerLocation;

        /// <summary>
        /// Was the player on a bridge on its last location?
        /// </summary>
        [FoldoutGroup("Player Location")]
        public bool WasPlayerOnBridgeOnLastLocation;

        /// <summary>
        /// Number of steps the player has taken.
        /// </summary>
        [FoldoutGroup("General")]
        public uint StepsTaken;

        /// <summary>
        /// List of badges obtained by the player, classified by the region.
        /// </summary>
        [FoldoutGroup("General")]
        [ReadOnly]
        public SerializableDictionary<Region, List<Badge>> ObtainedBadges;

        /// <summary>
        /// Scenes the player has visited.
        /// </summary>
        [FoldoutGroup("General")]
        [ReadOnly]
        public List<SceneInfoAsset> VisitedScenes;

        /// <summary>
        /// Last ball used by the player.
        /// </summary>
        [FoldoutGroup("Battle")]
        [ReadOnly]
        public Ball LastUsedBall;

        /// <summary>
        /// Number of caught monsters.
        /// This is only used for the savegame preview.
        /// </summary>
        [FoldoutGroup("Preview")]
        [ReadOnly]
        public uint DexNumber;

        /// <summary>
        /// Money the player has.
        /// This is only used for the savegame preview.
        /// </summary>
        [FoldoutGroup("Preview")]
        [ReadOnly]
        public uint Money;

        /// <summary>
        /// Player roster.
        /// This is only used for the savegame preview.
        /// </summary>
        [FoldoutGroup("Preview")]
        [ReadOnly]
        public List<MonsterPreview> RosterPreview = new();

        /// <summary>
        /// Reference to the YAPU version.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private YAPUInstaller.Runtime.Version YAPUVersion;

        /// <summary>
        /// Reference to the YAPU assets version.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private YAPUInstaller.Runtime.Version YAPUAssetsVersion;

        /// <summary>
        /// Reference to the new game initializer.
        /// </summary>
        [Inject]
        private NewGameInitializer newGameInitializer;

        /// <summary>
        /// Reference to the game version.
        /// </summary>
        [Inject]
        private Version gameVersion;

        /// <summary>
        /// Reference to the world database.
        /// </summary>
        [Inject]
        private WorldDatabase worldDatabase;

        /// <summary>
        /// Mask to use on dates.
        /// </summary>
        private const string DatetimeMask = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// Adds a new badge to the given region.
        /// </summary>
        /// <param name="badge">Badge to add.</param>
        /// <param name="region">Region to add it to.</param>
        public void AddBadge(Badge badge, Region region)
        {
            if (!ObtainedBadges.ContainsKey(region)) ObtainedBadges[region] = new List<Badge>();

            ObtainedBadges[region].Add(badge);
        }

        /// <summary>
        /// Adds a new badge to the given region.
        /// </summary>
        /// <param name="badge">Badge to add.</param>
        /// <param name="region">Region to add it to.</param>
        public bool HasBadge(Badge badge, Region region) =>
            ObtainedBadges.ContainsKey(region) && ObtainedBadges[region].Contains(badge);

        /// <summary>
        /// Get the badges for the given region.
        /// </summary>
        /// <param name="region">Region to check.</param>
        /// <returns>The list of badges.</returns>
        public List<Badge> GetBadges(Region region) =>
            ObtainedBadges.ContainsKey(region) ? ObtainedBadges[region] : new List<Badge>();

        /// <summary>
        /// Get the total number of badges.
        /// </summary>
        /// <returns>A int with the number of badges.</returns>
        public int GetTotalNumberOfBadges() =>
            ObtainedBadges.Sum<KeyValuePair<Region, List<Badge>>>(pair => pair.Value.Count);

        /// <summary>
        /// Get the multiplier to use based on the catch difficulty.
        /// </summary>
        public float GetCatchDifficultyMultiplier() =>
            CatchDifficulty switch
            {
                CatchDifficulty.Guaranteed => -1,
                CatchDifficulty.Easy => 1.5f,
                CatchDifficulty.Normal => 1f,
                CatchDifficulty.Hard => .5f,
                _ => throw new ArgumentOutOfRangeException()
            };

        /// <summary>
        /// Save the data to a persistable string.
        /// </summary>
        /// <param name="serializer">Serializer to be used to save to strings.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <returns>A serialized string.</returns>
        public override string SaveToText(ISerializer<string> serializer, PlayerCharacter playerCharacter)
        {
            UpdateVersions();

            LastPlayerLocation = playerCharacter.CurrentLocation;
            WasPlayerOnBridgeOnLastLocation = playerCharacter.CharacterController.IsOverBridge;

            Money = playerCharacter.PlayerBag.Money;
            DexNumber = playerCharacter.PlayerDex.NumberCaughtInAtLeastOneForm;

            RosterPreview = new List<MonsterPreview>();

            foreach (MonsterInstance monsterInstance in playerCharacter.PlayerRoster)
                RosterPreview.Add(new MonsterPreview(monsterInstance));

            return serializer.To(new SavableGlobalGameData(this));
        }

        /// <summary>
        /// Load the object from a persistable text.
        /// </summary>
        /// <param name="serializer">Serializer to use when loading.</param>
        /// <param name="data">Text containing the data to load.</param>
        /// <param name="yapuSettings"></param>
        /// <param name="monsterDatabase">Reference to the monster database.</param>
        public override IEnumerator LoadFromText(ISerializer<string> serializer,
                                                 string data,
                                                 YAPUSettings yapuSettings,
                                                 MonsterDatabaseInstance monsterDatabase)
        {
            SavableGlobalGameData readData = serializer.From<SavableGlobalGameData>(data);

            yield return WaitAFrame;

            readData.LoadGlobalGameData(this, monsterDatabase, worldDatabase);
        }

        /// <summary>
        /// Reset the last heal location.
        /// </summary>
        public override IEnumerator ResetSave()
        {
            yield return base.ResetSave();

            UpdateVersions();

            GameDifficulty = GameDifficulty.Normal;
            CatchDifficulty = CatchDifficulty.Normal;

            HasDex = false;
            LastHealLocation = newGameInitializer.InitialPosition;
            StepsTaken = 0;
            ObtainedBadges = new SerializableDictionary<Region, List<Badge>>();
            VisitedScenes = new List<SceneInfoAsset>();
            LastUsedBall = null;
            Money = 0;
            DexNumber = 0;
        }

        /// <summary>
        /// Update the versions.
        /// </summary>
        private void UpdateVersions()
        {
            LastSaveDate = DateTime.Now.ToString(DatetimeMask);
            LastGameVersion = gameVersion.FullVersion;
            LastYAPUVersion = YAPUVersion.FullVersion;
            LastYAPUAssetsVersion = YAPUAssetsVersion.FullVersion;
        }

        /// <summary>
        /// Class that stores a savable version of a monster preview, used in previewing the saves.
        /// </summary>
        [Serializable]
        public class MonsterPreview
        {
            /// <summary>
            /// Is this monster null?
            /// </summary>
            public bool IsNull;

            /// <summary>
            /// Monster species.
            /// </summary>
            public MonsterEntry Species;

            /// <summary>
            /// Monster form.
            /// </summary>
            public Form Form;

            /// <summary>
            /// Gender this monster has.
            /// </summary>
            public MonsterGender Gender;

            /// <summary>
            /// Is this monster an egg?
            /// </summary>
            public bool IsEgg;

            /// <summary>
            /// Default constructor.
            /// </summary>
            public MonsterPreview()
            {
            }

            /// <summary>
            /// Constructor from a monster instance.
            /// </summary>
            public MonsterPreview(MonsterInstance monsterInstance)
            {
                IsNull = monsterInstance == null || monsterInstance.IsNullEntry;

                if (IsNull) return;

                // ReSharper disable PossibleNullReferenceException
                Species = monsterInstance.Species;
                Form = monsterInstance.Form;
                Gender = monsterInstance.PhysicalData.Gender;
                IsEgg = monsterInstance.EggData.IsEgg;
                // ReSharper restore PossibleNullReferenceException
            }
        }
    }
}