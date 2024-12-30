using System;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Varguiniano.YAPU.Runtime.Badges;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls;
using Varguiniano.YAPU.Runtime.Saves;
using Varguiniano.YAPU.Runtime.World;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.Player
{
    /// <summary>
    /// Version of the global game data class that can be serialized to a string.
    /// </summary>
    [Serializable]
    public class SavableGlobalGameData
    {
        /// <summary>
        /// Date at which the game was last save.
        /// </summary>
        public string LastSaveDate;

        /// <summary>
        /// Version of the game when the game was last saved.
        /// </summary>
        public string LastGameVersion;

        /// <summary>
        /// Version of YAPU when the game was last saved.
        /// </summary>
        public string LastYAPUVersion;

        /// <summary>
        /// Version of the YAPU assets when the game was last saved.
        /// </summary>
        public string LastYAPUAssetsVersion;

        /// <summary>
        /// Difficulty for this game.
        /// </summary>
        public GameDifficulty GameDifficulty;

        /// <summary>
        /// Does the player have the dex?
        /// </summary>
        public bool HasDex;

        /// <summary>
        /// Last location in which the player healed.
        /// </summary>
        public SavableSceneLocation LastHealLocation;

        /// <summary>
        /// Last location in which the player was.
        /// </summary>
        public SavableSceneLocation LastPlayerLocation;

        /// <summary>
        /// Number of steps the player has taken.
        /// </summary>
        public uint StepsTaken;

        /// <summary>
        /// List of obtained badges per region.
        /// </summary>
        public List<ObjectPair<string, string>> ObtainedBadges;

        /// <summary>
        /// List of scenes the player has visited.
        /// </summary>
        public List<string> VisitedScenes;

        /// <summary>
        /// Last ball used by the player.
        /// </summary>
        public int LastUsedBall;

        /// <summary>
        /// Number of caught monsters.
        /// This is only used for the savegame preview.
        /// </summary>
        public uint DexNumber;

        /// <summary>
        /// Money the player has.
        /// This is only used for the savegame preview.
        /// </summary>
        public uint Money;

        /// <summary>
        /// Player roster.
        /// This is only used for the savegame preview.
        /// </summary>
        public List<SavableMonsterPreview> RosterPreview;

        /// <summary>
        /// Global game variables.
        /// </summary>
        public GameVariables GameVariables;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="globalGameData">Original global game data.</param>
        public SavableGlobalGameData(GlobalGameData globalGameData)
        {
            LastSaveDate = globalGameData.LastSaveDate;
            LastGameVersion = globalGameData.LastGameVersion;
            LastYAPUVersion = globalGameData.LastYAPUVersion;
            LastYAPUAssetsVersion = globalGameData.LastYAPUAssetsVersion;

            GameDifficulty = globalGameData.GameDifficulty;

            HasDex = globalGameData.HasDex;
            LastHealLocation = new SavableSceneLocation(globalGameData.LastHealLocation);
            LastPlayerLocation = new SavableSceneLocation(globalGameData.LastPlayerLocation);
            StepsTaken = globalGameData.StepsTaken;

            ObtainedBadges = new List<ObjectPair<string, string>>();

            foreach (KeyValuePair<Region, List<Badge>> pair in globalGameData.ObtainedBadges)
            {
                foreach (Badge badge in pair.Value)
                    ObtainedBadges.Add(new ObjectPair<string, string>
                                       {
                                           Key = pair.Key.name,
                                           Value = badge.name
                                       });
            }

            VisitedScenes = new List<string>();

            foreach (SceneInfoAsset visitedScene in globalGameData.VisitedScenes) VisitedScenes.Add(visitedScene.name);

            LastUsedBall = globalGameData.LastUsedBall == null
                               ? "Null".GetHashCode()
                               : globalGameData.LastUsedBall.name.GetHashCode();

            Money = globalGameData.Money;
            DexNumber = globalGameData.DexNumber;

            RosterPreview = new List<SavableMonsterPreview>();

            foreach (GlobalGameData.MonsterPreview preview in globalGameData.RosterPreview)
                RosterPreview.Add(new SavableMonsterPreview(preview));

            GameVariables = globalGameData.GameVariables;
        }

        /// <summary>
        /// Load back to the global game data.
        /// </summary>
        /// <param name="globalGameData">Reference to the global game data.</param>
        /// <param name="monsterDatabase">Reference to the monster database.</param>
        /// <param name="worldDatabase">Reference to the world database.</param>
        public void LoadGlobalGameData(GlobalGameData globalGameData,
                                       MonsterDatabaseInstance monsterDatabase,
                                       WorldDatabase worldDatabase)
        {
            globalGameData.LastSaveDate = LastSaveDate;
            globalGameData.LastGameVersion = LastGameVersion;
            globalGameData.LastYAPUVersion = LastYAPUVersion;
            globalGameData.LastYAPUAssetsVersion = LastYAPUAssetsVersion;

            globalGameData.GameDifficulty = GameDifficulty;

            globalGameData.HasDex = HasDex;
            globalGameData.LastHealLocation = LastHealLocation.ToSceneLocation(worldDatabase);
            globalGameData.LastPlayerLocation = LastPlayerLocation.ToSceneLocation(worldDatabase);
            globalGameData.StepsTaken = StepsTaken;

            globalGameData.ObtainedBadges = new SerializableDictionary<Region, List<Badge>>();

            foreach (ObjectPair<string, string> pair in ObtainedBadges)
            {
                Region region = worldDatabase.GetRegionByName(pair.Key);

                if (!globalGameData.ObtainedBadges.ContainsKey(region))
                    globalGameData.ObtainedBadges[region] = new List<Badge>();

                globalGameData.ObtainedBadges[region].Add(monsterDatabase.GetBadgeByName(pair.Value));
            }

            globalGameData.VisitedScenes = new List<SceneInfoAsset>();

            foreach (string visitedScene in VisitedScenes)
                globalGameData.VisitedScenes.Add(worldDatabase.GetSceneByName(visitedScene));

            globalGameData.LastUsedBall = LastUsedBall == "Null".GetHashCode()
                                              ? null
                                              : (Ball) monsterDatabase.GetItemByHash(LastUsedBall);

            globalGameData.Money = Money;
            globalGameData.DexNumber = DexNumber;

            globalGameData.RosterPreview = new List<GlobalGameData.MonsterPreview>();

            foreach (SavableMonsterPreview monsterPreview in RosterPreview)
                globalGameData.RosterPreview.Add(monsterPreview.ToMonsterPreview(monsterDatabase));

            globalGameData.GameVariables = GameVariables;
        }

        /// <summary>
        /// Class that stores a savable version of a monster preview, used in previewing the saves.
        /// </summary>
        [Serializable]
        public class SavableMonsterPreview
        {
            /// <summary>
            /// Is this monster null?
            /// </summary>
            public bool IsNull;

            /// <summary>
            /// Monster species.
            /// </summary>
            public int Species;

            /// <summary>
            /// Monster form.
            /// </summary>
            public int Form;

            /// <summary>
            /// Gender this monster has.
            /// </summary>
            public MonsterGender Gender;

            /// <summary>
            /// Is this monster an egg?
            /// </summary>
            public bool IsEgg;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="preview">In memory preview.</param>
            public SavableMonsterPreview(GlobalGameData.MonsterPreview preview)
            {
                IsNull = preview.IsNull;

                if (IsNull) return;

                Species = preview.Species.name.GetHashCode();
                Form = preview.Form.name.GetHashCode();
                Gender = preview.Gender;
                IsEgg = preview.IsEgg;
            }

            /// <summary>
            /// Load the data back into a monster instance.
            /// </summary>
            /// <param name="database">Database reference.</param>
            /// <returns>A full monster instance.</returns>
            public GlobalGameData.MonsterPreview ToMonsterPreview(MonsterDatabaseInstance database)
            {
                GlobalGameData.MonsterPreview preview = new()
                                                        {
                                                            IsNull = IsNull
                                                        };

                if (IsNull) return preview;

                preview.Species = database.GetMonsterByHash(Species);
                preview.Form = database.GetFormByHash(Form);
                preview.Gender = Gender;
                preview.IsEgg = IsEgg;

                return preview;
            }
        }
    }
}