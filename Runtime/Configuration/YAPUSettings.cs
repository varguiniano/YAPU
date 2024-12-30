using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.World.Encounters;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;

namespace Varguiniano.YAPU.Runtime.Configuration
{
    /// <summary>
    /// Class to hold some internal YAPU settings to be chosen by the designer.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Settings", fileName = "YAPUSettings")]
    public class YAPUSettings : WhateverScriptable<YAPUSettings>
    {
        /// <summary>
        /// Main menu scene.
        /// </summary>
        [FoldoutGroup("General")]
        public SceneReference MainMenuScene;

        /// <summary>
        /// Allow the player to choose their character when starting a new game?
        /// </summary>
        [FoldoutGroup("New game")]
        public bool AllowPlayerToChooseCharacterOnNewGame = true;

        /// <summary>
        /// The length of a day in seconds.
        /// </summary>
        [FoldoutGroup("Time")]
        public float DayLengthInSeconds = 1440;

        /// <summary>
        /// Chance of encountering monsters when walking on tiles.
        /// </summary>
        [FoldoutGroup("Encounters")]
        public SerializedDictionary<EncounterType, float> TileEncounterChance;

        /// <summary>
        /// Encounter types that can have their chances modified by items and abilities.
        /// </summary>
        [FoldoutGroup("Encounters")]
        [Tooltip("Encounter types that can have their chances modified by items and abilities.")]
        public List<EncounterType> EncountersWithModifiableChances;

        /// <summary>
        /// Chance a wild shinny monster appears.
        /// </summary>
        [FoldoutGroup("Encounters")]
        public float WildShinyChance = 1f / 512;

        /// <summary>
        /// Max number of EV points a monster can have.
        /// </summary>
        [FoldoutGroup("Experience")]
        public uint MaxEVPointsPerMonster = 510;

        /// <summary>
        /// Max sheen number a monster can have.
        /// </summary>
        [FoldoutGroup("Experience")]
        public uint MaxSheen = 255;

        /// <summary>
        /// Steps to tick friendship boosts.
        /// </summary>
        [FoldoutGroup("Friendship")]
        public uint FriendshipTickSteps = 128;

        /// <summary>
        /// Chance to tick friendship.
        /// </summary>
        [FoldoutGroup("Friendship")]
        [PropertyRange(0, 1)]
        public float FriendshipTickChance = .5f;

        /// <summary>
        /// Move to use when there are no PP left.
        /// </summary>
        [FoldoutGroup("Battle")]
        public Move NoPPMove;

        /// <summary>
        /// Monster types that can only affect grounded battlers.
        /// </summary>
        [FoldoutGroup("Battle")]
        public List<MonsterType> TypesThatCanOnlyAffectGrounded;

        /// <summary>
        /// Chance of becoming infected after a battle.
        /// </summary>
        [FoldoutGroup("Battle")]
        public float AfterBattleVirusChance = 3f / 65536;

        /// <summary>
        /// Minimum and maximum scales of monsters in battle.
        /// 1 is the mean of all monsters.
        /// </summary>
        [FoldoutGroup("Battle")]
        public Vector2 SizeLimitsInBattle;

        /// <summary>
        /// Origin games of the generated monsters.
        /// </summary>
        [FoldoutGroup("Monster generation")]
        public string OriginGame;

        /// <summary>
        /// Percentage height can vary when generating a monster.
        /// </summary>
        [FoldoutGroup("Monster generation")]
        [Range(0, .99f)]
        public float HeightVariability = .1f;

        /// <summary>
        /// Percentage weight can vary when generating a monster.
        /// </summary>
        [FoldoutGroup("Monster generation")]
        [Range(0, .99f)]
        public float WeightVariability = .1f;

        /// <summary>
        /// Ball to be used by default when generating a monster.
        /// </summary>
        [FoldoutGroup("Monster generation")]
        public Ball DefaultBall;

        /// <summary>
        /// Max character size for nicknames.
        /// </summary>
        [FoldoutGroup("Monster generation")]
        public byte MaxNicknameSize = 20;

        /// <summary>
        /// Level monsters are born at.
        /// </summary>
        [FoldoutGroup("Monster generation")]
        [Range(1, 100)]
        public byte LevelMonstersAreBornAt = 1;

        /// <summary>
        /// Monster type of eggs.
        /// </summary>
        [FoldoutGroup("Monster generation")]
        public MonsterType EggsType;

        /// <summary>
        /// Steps to be taken to trigger an egg cycle.
        /// </summary>
        [FoldoutGroup("Monster generation")]
        public uint StepsPerEggCycle = 256;

        /// <summary>
        /// Container for an empty sprite that can be retrieved as an implementation of IWorldDataContainer.
        /// </summary>
        [FoldoutGroup("Actors")]
        public EmptySpriteContainer EmptySpriteContainer = new();

        /// <summary>
        /// Currency symbol position to use per language.
        /// </summary>
        [FoldoutGroup("Currency")]
        public SerializableDictionary<string, CurrencySymbolPosition> CurrencySymbolPositions;

        /// <summary>
        /// Default position for other languages.
        /// </summary>
        [FoldoutGroup("Currency")]
        public CurrencySymbolPosition DefaultCurrencySymbolPosition;

        /// <summary>
        /// Different positions the currency symbol can have.
        /// </summary>
        public enum CurrencySymbolPosition
        {
            Left,
            Right
        }
    }
}