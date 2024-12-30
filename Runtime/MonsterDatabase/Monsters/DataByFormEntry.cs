using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.Monster.Breeding;
using Varguiniano.YAPU.Runtime.Monster.Evolution;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using Varguiniano.YAPU.Runtime.MonsterDatabase.EggGroups;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Experience;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Species;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.MonsterDex;
using Varguiniano.YAPU.Runtime.UI.Dex;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.TwoDAudio.Runtime;
using Move = Varguiniano.YAPU.Runtime.MonsterDatabase.Moves.Move;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters
{
    /// <summary>
    /// Class to store the data for a specific form.
    /// </summary>
    [Serializable]
    public class DataByFormEntry : MonsterDatabaseData
    {
        /// <summary>
        /// Group title for this entry.
        /// </summary>
        [UsedImplicitly]
        private string GroupTitle => Form == null ? "Data" : Form.name;

        /// <summary>
        /// Form for this entry.
        /// </summary>
        [FoldoutGroup("$GroupTitle")]
        [ReadOnly]
        [PropertyOrder(-2)]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllForms))]
        #endif
        public Form Form;

        /// <summary>
        /// Developer comments for this form.
        /// </summary>
        [PropertySpace(SpaceBefore = 30, SpaceAfter = 30)]
        [FoldoutGroup("$GroupTitle")]
        [SerializeField]
        [Multiline(4)]
        [UsedImplicitly]
        [PropertyOrder(-2)]
        public string DeveloperComments;

        /// <summary>
        /// Open the helper that simplifies the creation of this monster.
        /// </summary>
        [PropertySpace(SpaceAfter = 30)]
        [FoldoutGroup("$GroupTitle")]
        [Button(ButtonSizes.Gigantic)]
        [PropertyOrder(-1)]
        private void OpenHelper()
        {
            #if UNITY_EDITOR
            EditorApplication.ExecuteMenuItem("YAPU/Monster Creation Helper");
            #endif
        }

        /// <summary>
        /// Does this monster have an override material on male gender?
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites")]
        [ShowIf(nameof(HasBinaryGender))]
        public bool HasMaleMaterialOverride;

        /// <summary>
        /// Front sprite.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites")]
        [HorizontalGroup("$GroupTitle" + "/Graphics/Sprites/BattleMaterials")]
        [PreviewField(100)]
        public Material Front;

        /// <summary>
        /// Back sprite.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites")]
        [HorizontalGroup("$GroupTitle" + "/Graphics/Sprites/BattleMaterials")]
        [PreviewField(100)]
        public Material Back;

        /// <summary>
        /// Material override for front male.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites")]
        [HorizontalGroup("$GroupTitle" + "/Graphics/Sprites/BattleMaleMaterials")]
        [HideIf("@!HasBinaryGender || !HasMaleMaterialOverride")]
        [PreviewField(100)]
        public Material FrontMale;

        /// <summary>
        /// Material override for back male.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites")]
        [HorizontalGroup("$GroupTitle" + "/Graphics/Sprites/BattleMaleMaterials")]
        [HideIf("@!HasBinaryGender || !HasMaleMaterialOverride")]
        [PreviewField(100)]
        public Material BackMale;

        /// <summary>
        /// Front sprite.
        /// </summary>
        [FormerlySerializedAs("FrontShinny")]
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites")]
        [HorizontalGroup("$GroupTitle" + "/Graphics/Sprites/BattleShinnyMaterials")]
        [ShowIf("@Form.HasShinyVersion")]
        [PreviewField(100)]
        public Material FrontShiny;

        /// <summary>
        /// Back sprite.
        /// </summary>
        [FormerlySerializedAs("BackShinny")]
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites")]
        [HorizontalGroup("$GroupTitle" + "/Graphics/Sprites/BattleShinnyMaterials")]
        [ShowIf("@Form.HasShinyVersion")]
        [PreviewField(100)]
        public Material BackShiny;

        /// <summary>
        /// Material override for front shinny male.
        /// </summary>
        [FormerlySerializedAs("FrontShinnyMale")]
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites")]
        [HorizontalGroup("$GroupTitle" + "/Graphics/Sprites/BattleShinnyMaleMaterials")]
        [HideIf("@!Form.HasShinyVersion || !HasBinaryGender || !HasMaleMaterialOverride")]
        [PreviewField(100)]
        public Material FrontShinyMale;

        /// <summary>
        /// Material override for back shinny male.
        /// </summary>
        [FormerlySerializedAs("BackShinnyMale")]
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites")]
        [HorizontalGroup("$GroupTitle" + "/Graphics/Sprites/BattleShinnyMaleMaterials")]
        [HideIf("@!Form.HasShinyVersion || !HasBinaryGender || !HasMaleMaterialOverride")]
        [PreviewField(100)]
        public Material BackShinyMale;

        /// <summary>
        /// Icon sprite.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites")]
        [HorizontalGroup("$GroupTitle" + "/Graphics/Sprites/IconsFemale")]
        [PreviewField(100)]
        public Sprite Icon;

        /// <summary>
        /// Icon sprite.
        /// </summary>
        [FormerlySerializedAs("IconShinny")]
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites")]
        [HorizontalGroup("$GroupTitle" + "/Graphics/Sprites/IconsFemale")]
        [HideIf("@!Form.HasShinyVersion")]
        [PreviewField(100)]
        public Sprite IconShiny;

        /// <summary>
        /// Icon sprite.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites")]
        [HorizontalGroup("$GroupTitle" + "/Graphics/Sprites/IconsMale")]
        [HideIf("@!HasBinaryGender || !HasMaleMaterialOverride")]
        [PreviewField(100)]
        public Sprite IconMale;

        /// <summary>
        /// Icon sprite.
        /// </summary>
        [FormerlySerializedAs("IconShinnyMale")]
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites")]
        [HorizontalGroup("$GroupTitle" + "/Graphics/Sprites/IconsMale")]
        [HideIf("@!Form.HasShinyVersion || !HasBinaryGender || !HasMaleMaterialOverride")]
        [PreviewField(100)]
        public Sprite IconShinyMale;

        /// <summary>
        /// Material to use when it's an egg.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites")]
        [VerticalGroup("$GroupTitle" + "/Graphics/Sprites/Egg")]
        [PreviewField(50)]
        public Material EggMaterial;

        /// <summary>
        /// Icon to use when it's an egg.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites")]
        [VerticalGroup("$GroupTitle" + "/Graphics/Sprites/Egg")]
        [PreviewField(50)]
        public Sprite EggIcon;

        /// <summary>
        /// Does this form have world sprites?
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        public bool HasWorldSprites;

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [ShowIf("@HasWorldSprites")]
        [PreviewField(100)]
        public Sprite WorldIdleSpriteDown;

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [ShowIf("@HasWorldSprites")]
        [PreviewField(100)]
        public Sprite WorldIdleSpriteUp;

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [ShowIf("@HasWorldSprites")]
        [PreviewField(100)]
        public Sprite WorldIdleSpriteLeft;

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [ShowIf("@HasWorldSprites")]
        [PreviewField(100)]
        public Sprite WorldIdleSpriteRight;

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [HideIf("@!HasBinaryGender || !HasMaleMaterialOverride || !HasWorldSprites")]
        [PreviewField(100)]
        public Sprite WorldIdleSpriteDownMale;

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [HideIf("@!HasBinaryGender || !HasMaleMaterialOverride || !HasWorldSprites")]
        [PreviewField(100)]
        public Sprite WorldIdleSpriteUpMale;

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [HideIf("@!HasBinaryGender || !HasMaleMaterialOverride || !HasWorldSprites")]
        [PreviewField(100)]
        public Sprite WorldIdleSpriteLeftMale;

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [HideIf("@!HasBinaryGender || !HasMaleMaterialOverride || !HasWorldSprites")]
        [PreviewField(100)]
        public Sprite WorldIdleSpriteRightMale;

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [ShowIf("@HasWorldSprites")]
        [PreviewField(100)]
        public List<Sprite> WorldWalkingSpriteDown = new();

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [ShowIf("@HasWorldSprites")]
        [PreviewField(100)]
        public List<Sprite> WorldWalkingSpriteUp = new();

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [ShowIf("@HasWorldSprites")]
        [PreviewField(100)]
        public List<Sprite> WorldWalkingSpriteLeft = new();

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [ShowIf("@HasWorldSprites")]
        [PreviewField(100)]
        public List<Sprite> WorldWalkingSpriteRight = new();

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [HideIf("@!HasBinaryGender || !HasMaleMaterialOverride || !HasWorldSprites")]
        [PreviewField(100)]
        public List<Sprite> WorldWalkingSpriteDownMale = new();

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [HideIf("@!HasBinaryGender || !HasMaleMaterialOverride || !HasWorldSprites")]
        [PreviewField(100)]
        public List<Sprite> WorldWalkingSpriteUpMale = new();

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [HideIf("@!HasBinaryGender || !HasMaleMaterialOverride || !HasWorldSprites")]
        [PreviewField(100)]
        public List<Sprite> WorldWalkingSpriteLeftMale = new();

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [HideIf("@!HasBinaryGender || !HasMaleMaterialOverride || !HasWorldSprites")]
        [PreviewField(100)]
        public List<Sprite> WorldWalkingSpriteRightMale = new();

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FormerlySerializedAs("WorldIdleShinnySpriteDown")]
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [ShowIf("@Form.HasShinyVersion && HasWorldSprites")]
        [PreviewField(100)]
        public Sprite WorldIdleShinySpriteDown;

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FormerlySerializedAs("WorldIdleShinnySpriteUp")]
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [ShowIf("@Form.HasShinyVersion && HasWorldSprites")]
        [PreviewField(100)]
        public Sprite WorldIdleShinySpriteUp;

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FormerlySerializedAs("WorldIdleShinnySpriteLeft")]
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [ShowIf("@Form.HasShinyVersion && HasWorldSprites")]
        [PreviewField(100)]
        public Sprite WorldIdleShinySpriteLeft;

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FormerlySerializedAs("WorldIdleShinnySpriteRight")]
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [ShowIf("@Form.HasShinyVersion && HasWorldSprites")]
        [PreviewField(100)]
        public Sprite WorldIdleShinySpriteRight;

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FormerlySerializedAs("WorldIdleShinnySpriteDownMale")]
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [HideIf("@!Form.HasShinyVersion || !HasBinaryGender || !HasMaleMaterialOverride || !HasWorldSprites")]
        [PreviewField(100)]
        public Sprite WorldIdleShinySpriteDownMale;

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FormerlySerializedAs("WorldIdleShinnySpriteUpMale")]
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [HideIf("@!Form.HasShinyVersion || !HasBinaryGender || !HasMaleMaterialOverride || !HasWorldSprites")]
        [PreviewField(100)]
        public Sprite WorldIdleShinySpriteUpMale;

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FormerlySerializedAs("WorldIdleShinnySpriteLeftMale")]
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [HideIf("@!Form.HasShinyVersion || !HasBinaryGender || !HasMaleMaterialOverride || !HasWorldSprites")]
        [PreviewField(100)]
        public Sprite WorldIdleShinySpriteLeftMale;

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FormerlySerializedAs("WorldIdleShinnySpriteRightMale")]
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [HideIf("@!Form.HasShinyVersion || !HasBinaryGender || !HasMaleMaterialOverride || !HasWorldSprites")]
        [PreviewField(100)]
        public Sprite WorldIdleShinySpriteRightMale;

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FormerlySerializedAs("WorldWalkingShinnySpriteDown")]
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [ShowIf("@Form.HasShinyVersion && HasWorldSprites")]
        [PreviewField(100)]
        public List<Sprite> WorldWalkingShinySpriteDown = new();

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FormerlySerializedAs("WorldWalkingShinnySpriteUp")]
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [ShowIf("@Form.HasShinyVersion && HasWorldSprites")]
        [PreviewField(100)]
        public List<Sprite> WorldWalkingShinySpriteUp = new();

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FormerlySerializedAs("WorldWalkingShinnySpriteLeft")]
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [ShowIf("@Form.HasShinyVersion && HasWorldSprites")]
        [PreviewField(100)]
        public List<Sprite> WorldWalkingShinySpriteLeft = new();

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FormerlySerializedAs("WorldWalkingShinnySpriteRight")]
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [ShowIf("@Form.HasShinyVersion && HasWorldSprites")]
        [PreviewField(100)]
        public List<Sprite> WorldWalkingShinySpriteRight = new();

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FormerlySerializedAs("WorldWalkingShinnySpriteDownMale")]
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [HideIf("@!Form.HasShinyVersion || !HasBinaryGender || !HasMaleMaterialOverride || !HasWorldSprites")]
        [PreviewField(100)]
        public List<Sprite> WorldWalkingShinySpriteDownMale = new();

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FormerlySerializedAs("WorldWalkingShinnySpriteUpMale")]
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [HideIf("@!Form.HasShinyVersion || !HasBinaryGender || !HasMaleMaterialOverride || !HasWorldSprites")]
        [PreviewField(100)]
        public List<Sprite> WorldWalkingShinySpriteUpMale = new();

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FormerlySerializedAs("WorldWalkingShinnySpriteLeftMale")]
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [HideIf("@!Form.HasShinyVersion || !HasBinaryGender || !HasMaleMaterialOverride || !HasWorldSprites")]
        [PreviewField(100)]
        public List<Sprite> WorldWalkingShinySpriteLeftMale = new();

        /// <summary>
        /// WorldIdle sprite.
        /// </summary>
        [FormerlySerializedAs("WorldWalkingShinnySpriteRightMale")]
        [FoldoutGroup("$GroupTitle" + "/Graphics")]
        [TitleGroup("$GroupTitle" + "/Graphics/Sprites/World Sprites")]
        [HideIf("@!Form.HasShinyVersion || !HasBinaryGender || !HasMaleMaterialOverride || !HasWorldSprites")]
        [PreviewField(100)]
        public List<Sprite> WorldWalkingShinySpriteRightMale = new();

        /// <summary>
        /// First monster type.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Types")]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsterTypes))]
        #endif
        public MonsterType FirstType;

        /// <summary>
        /// Second monster type.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Types")]
        [HorizontalGroup("$GroupTitle" + "/Types/Second")]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsterTypes))]
        #endif
        public MonsterType SecondType;

        /// <summary>
        /// Localization key for the dex description.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Dex")]
        public string DexDescriptionKey;

        /// <summary>
        /// Monster species.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Dex")]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllSpecies))]
        #endif
        public MonsterSpecies Species;

        /// <summary>
        /// Height in m.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Dex")]
        [Tooltip("In m.")]
        [Unit(Units.Meter)]
        public float Height;

        /// <summary>
        /// Wight in Kg.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Dex")]
        [Tooltip("In Kg.")]
        [Unit(Units.Kilogram)]
        public float Weight;

        /// <summary>
        /// Cry of this monster.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Dex")]
        public AudioReference Cry;

        /// <summary>
        /// Dictionary that stores the abilities a monster can have and if they are hidden.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Abilities")]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllAbilities))]
        #endif
        public List<Ability> Abilities;

        /// <summary>
        /// Dictionary that stores the abilities a monster can have and if they are hidden.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Abilities")]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllAbilities))]
        #endif
        public List<Ability> HiddenAbilities;

        /// <summary>
        /// All the available abilities for this form.
        /// </summary>
        public List<Ability> AllAvailableAbilities
        {
            get
            {
                List<Ability> abilities = new();

                foreach (Ability ability in Abilities.Where(ability => !abilities.Contains(ability)))
                    abilities.Add(ability);

                foreach (Ability ability in HiddenAbilities.Where(ability => !abilities.Contains(ability)))
                    abilities.Add(ability);

                return abilities;
            }
        }

        /// <summary>
        /// This monster's EV yield.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Training")]
        public List<StatByteValuePair> EVYield;

        /// <summary>
        /// Catch rate of this monster.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Training")]
        public byte CatchRate;

        /// <summary>
        /// Base friendship of this monster.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Training")]
        public byte BaseFriendship;

        /// <summary>
        /// Base experience of this monster.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Training")]
        public uint BaseExperience;

        /// <summary>
        /// Growth rate of this monster.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Training")]
        public GrowthRate GrowthRate;

        /// <summary>
        /// Egg groups for this monster.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Breeding")]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllEggGroups))]
        #endif
        public List<EggGroup> EggGroups;

        /// <summary>
        /// Eggs cycles for this monster.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Breeding")]
        public byte EggCycles;

        /// <summary>
        /// Monsters and forms this monster's eggs can hatch.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Breeding")]
        [SerializeReference]
        public List<BreedingData> BreedingData = new();

        /// <summary>
        /// Does this monster have binary gender.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Breeding")]
        public bool HasBinaryGender;

        /// <summary>
        /// Does this form have gender variations?
        /// </summary>
        public bool HasGenderVariations => HasBinaryGender && HasMaleMaterialOverride;

        /// <summary>
        /// Ratio of female eggs.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Breeding")]
        [ShowIf(nameof(HasBinaryGender))]
        [Range(0, 1)]
        public float FemaleRatio = .5f;

        /// <summary>
        /// Ratio of male eggs.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Breeding")]
        [ShowIf(nameof(HasBinaryGender))]
        [ShowInInspector]
        [ReadOnly]
        [PropertyRange(0, 1)]
        public float MaleRatio => 1 - FemaleRatio;

        /// <summary>
        /// Chance of wild monsters running.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Wild")]
        [Range(0, 1)]
        public float WildRunChance;

        /// <summary>
        /// Items wild monsters can appear holding and the chance that happens.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Wild")]
        public SerializableDictionary<Item, float> WildHeldItems;

        /// <summary>
        /// Collection of level moves duplicated.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Moves")]
        [ShowInInspector]
        [ShowIf("@" + nameof(levelMoveDupes) + ".Count > 0")]
        [InfoBox("The following moves are learnt at multiples levels. Is that okay?",
                 InfoMessageType.Warning)]
        [ReadOnly]
        private List<Move> levelMoveDupes = new();

        /// <summary>
        /// List that allows to easy edit moves learnt by level.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Moves")]
        #if UNITY_EDITOR
        [OnCollectionChanged(nameof(ValidateMoves))]
        #endif
        public List<MoveLevelPair> MovesByLevel;

        /// <summary>
        /// Collection of evolution moves duplicated.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Moves")]
        [ShowInInspector]
        [ShowIf("@" + nameof(evolutionMoveDupes) + ".Count > 0")]
        [InfoBox("The following moves are duplicated in the evolution category.",
                 InfoMessageType.Error)]
        [ReadOnly]
        private List<Move> evolutionMoveDupes = new();

        /// <summary>
        /// Moves it can learn after evolving.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Moves")]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMoves))]
        [OnCollectionChanged(nameof(ValidateMoves))]
        #endif
        public List<Move> OnEvolutionMoves;

        /// <summary>
        /// Collection of egg moves duplicated.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Moves")]
        [ShowInInspector]
        [ShowIf("@" + nameof(eggMoveDupes) + ".Count > 0")]
        [InfoBox("The following moves are duplicated in the egg category.",
                 InfoMessageType.Error)]
        [ReadOnly]
        private List<Move> eggMoveDupes = new();

        /// <summary>
        /// Moves it can learn from breeding.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Moves")]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMoves))]
        [OnCollectionChanged(nameof(ValidateMoves))]
        #endif
        public List<Move> EggMoves;

        /// <summary>
        /// Collection of other moves duplicated with level or eggs.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Moves")]
        [ShowInInspector]
        [ShowIf("@" + nameof(otherWithEggOrLevelMoveDupes) + ".Count > 0")]
        [InfoBox("The following moves are in the other category but are also in level, evolution or egg, so there is no need to have them on other.",
                 InfoMessageType.Error)]
        [ReadOnly]
        private List<Move> otherWithEggOrLevelMoveDupes = new();

        /// <summary>
        /// Collection of other moves duplicated.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Moves")]
        [ShowInInspector]
        [ShowIf("@" + nameof(otherMoveDupes) + ".Count > 0")]
        [InfoBox("The following moves are duplicated in the other category.",
                 InfoMessageType.Error)]
        [ReadOnly]
        private List<Move> otherMoveDupes = new();

        /// <summary>
        /// Moves it can learn from other methods.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Moves")]
        [HideIf(nameof(CanLearnAnyMove))]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMoves))]
        [OnCollectionChanged(nameof(ValidateMoves))]
        #endif
        private List<Move> OtherLearnMoves;

        #if UNITY_EDITOR

        /// <summary>
        /// Get the other learn moves.
        /// To be used by editor tools.
        /// </summary>
        public List<Move> GetOtherLearnMovesInEditor() => OtherLearnMoves;

        /// <summary>
        /// Clear the other learn moves.
        /// To be used by editor tools.
        /// </summary>
        public void ClearOtherLearnMoves()
        {
            OtherLearnMoves.Clear();
            ValidateMoves();
        }

        /// <summary>
        /// Add a move to the other learn moves.
        /// To be used by editor tools.
        /// </summary>
        /// <param name="move">Move to add.</param>
        public void AddOtherLearnMove(Move move)
        {
            OtherLearnMoves.Add(move);
            ValidateMoves();
        }

        /// <summary>
        /// Remove the provided moves from the other learn moves.
        /// To be used by editor tools.
        /// </summary>
        /// <param name="moves">Moves to remove.</param>
        public void RemoveOtherLearnMoves(List<Move> moves)
        {
            foreach (Move move in moves) OtherLearnMoves.Remove(move);

            ValidateMoves();
        }

        #endif

        /// <summary>
        /// Base stats of this monster.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Base Stats")]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true)]
        public SerializableDictionary<Stat, byte> BaseStats;

        /// <summary>
        /// Get the total base stats of this monster.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Base Stats")]
        [ReadOnly]
        [ShowInInspector]
        public uint TotalBaseStats
        {
            get
            {
                uint count = 0;

                foreach (KeyValuePair<Stat, byte> pair in BaseStats) count += pair.Value;

                return count;
            }
        }

        /// <summary>
        /// If a stat has a total limit, it should be set here.
        /// Example: Shedinja's HP is always 1.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Base Stats")]
        [ShowInInspector]
        [Tooltip("If a stat has a total limit, it should be set here.\nExample: Shedinja's HP is always 1.")]
        public SerializableDictionary<Stat, uint> StatLimits;

        /// <summary>
        /// Evolutions this monster has.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Evolution")]
        [SerializeReference]
        public List<EvolutionData> Evolutions = new();

        /// <summary>
        /// Mega evolutions this monster has and the item needed to evolve into them.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Battle")]
        public SerializableDictionary<Item, Form> MegaEvolutions;

        /// <summary>
        /// Is it an ultra beast?
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Other")]
        public bool IsUltraBeast;

        /// <summary>
        /// Is it legendary?
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Other")]
        public bool IsLegendary;

        /// <summary>
        /// Is it mythical?
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Other")]
        public bool IsMythical;

        /// <summary>
        /// Can this monster learn any move?
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Other")]
        public bool CanLearnAnyMove;

        /// <summary>
        /// Does this mon use the Spinda shader?
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Other")]
        public bool UsesSpindaShader;

        /// <summary>
        /// Check if this entry is of the given type.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns>True if any of the two types matches.</returns>
        public bool IsOfType(MonsterType type) => FirstType == type || SecondType == type;

        /// <summary>
        /// Check if the monster is of any type on a list.
        /// </summary>
        /// <param name="monsterType">Types to check.</param>
        /// <returns>True if it is.</returns>
        public bool IsOfAnyType(IEnumerable<MonsterType> monsterType) => monsterType.Any(IsOfType);

        /// <summary>
        /// Can this monster have the given ability?
        /// </summary>
        /// <param name="ability">Ability to check.</param>
        /// <returns>True if it can have it.</returns>
        public bool CanHaveAbility(Ability ability) => Abilities.Contains(ability) || HiddenAbilities.Contains(ability);

        /// <summary>
        /// List of all available moves for this form.
        /// </summary>
        public List<Move> GetAllAvailableMoves(MonsterDatabaseInstance database)
        {
            if (CanLearnAnyMove) return database.GetMoves();

            List<Move> availableMoves = new();

            foreach (MoveLevelPair pair in MovesByLevel.Where(pair => !availableMoves.Contains(pair.Move)))
                availableMoves.Add(pair.Move);

            foreach (Move move in OnEvolutionMoves.Where(move => !availableMoves.Contains(move)))
                availableMoves.Add(move);

            foreach (Move move in EggMoves.Where(move => !availableMoves.Contains(move))) availableMoves.Add(move);

            foreach (Move move in OtherLearnMoves.Where(move => !availableMoves.Contains(move)))
                availableMoves.Add(move);

            return availableMoves;
        }

        /// <summary>
        /// Moves it can learn from other methods.
        /// </summary>
        public List<Move> GetOtherLearnMoves(MonsterDatabaseInstance database) =>
            CanLearnAnyMove ? database.GetMoves() : OtherLearnMoves;

        /// <summary>
        /// Can this monster learn the given move?
        /// </summary>
        /// <param name="move">Move to check.</param>
        /// <returns>True if it can learn it.</returns>
        public bool CanLearnMove(Move move)
        {
            if (CanLearnAnyMove) return true;

            bool inLevelMoves = false;

            foreach (MoveLevelPair _ in MovesByLevel.Where(pair => pair.Move == move)) inLevelMoves = true;

            return inLevelMoves
                || OnEvolutionMoves.Contains(move)
                || EggMoves.Contains(move)
                || OtherLearnMoves.Contains(move);
        }

        /// <summary>
        /// Get the moves that this monster learns at the given level.
        /// </summary>
        /// <param name="level">Level to check.</param>
        /// <returns>A list of the moves learnt.</returns>
        public List<Move> GetMovesLearntInLevel(byte level) =>
            (from pair in MovesByLevel where pair.Level == level select pair.Move).ToList();

        /// <summary>
        /// Does this form have the given egg group?
        /// </summary>
        /// <param name="group">Group to check.</param>
        /// <returns>True if it has it.</returns>
        public bool HasEggGroup(EggGroup group) => EggGroups.Contains(group);

        /// <summary>
        /// Get the relationships this mega evolution data can have to be displayed on the dex.
        /// </summary>
        /// <param name="entry">Monster entry being displayed.</param>
        /// <param name="formEntry">Form entry being displayed.</param>
        /// <param name="gender">Gender being displayed.</param>
        /// <returns>A list of the relationships generated.</returns>
        public List<DexMonsterRelationshipData> GetMegaEvolutionDexRelationshipData(MonsterDexEntry entry,
            FormDexEntry formEntry,
            MonsterGender gender)
        {
            List<DexMonsterRelationshipData> evolutionRelationships = new();

            foreach (KeyValuePair<Item, Form> megaEvolution in MegaEvolutions)
                evolutionRelationships.Add(new DexMonsterRelationshipData
                                           {
                                               Species = entry.Species,
                                               Form = formEntry.Form.IsShiny && megaEvolution.Value.HasShinyVersion
                                                          ? megaEvolution.Value.ShinyVersion
                                                          : megaEvolution.Value,
                                               Gender = gender,
                                               Mode = DexMonsterRelationshipData
                                                     .RelationShipDisplayType.Icon,
                                               Icon = megaEvolution.Key.Icon,
                                               LocalizableDescriptionKey =
                                                   "Dex/MegaEvolutions/Description"
                                           });

            return evolutionRelationships;
        }

        /// <summary>
        /// Get the monster icon.
        /// </summary>
        /// <returns>A sprite with the icon.</returns>
        public Sprite GetIcon(bool isEgg, bool isShiny, MonsterGender gender)
        {
            if (isEgg) return EggIcon;

            if (isShiny)
            {
                if (HasMaleMaterialOverride && gender == MonsterGender.Male) return IconShinyMale;

                return IconShiny;
            }

            if (HasMaleMaterialOverride && gender == MonsterGender.Male) return IconMale;

            return Icon;
        }

        /// <summary>
        /// Clone this entry to a new object.
        /// </summary>
        /// <returns>A deep clone copy.</returns>
        public DataByFormEntry Clone() =>
            new()
            {
                Form = Form,
                DeveloperComments = DeveloperComments,
                HasMaleMaterialOverride = HasMaleMaterialOverride,
                Front = Front,
                Back = Back,
                FrontMale = FrontMale,
                BackMale = BackMale,
                FrontShiny = FrontShiny,
                BackShiny = BackShiny,
                FrontShinyMale = FrontShinyMale,
                BackShinyMale = BackShinyMale,
                Icon = Icon,
                IconShiny = IconShiny,
                IconMale = IconMale,
                IconShinyMale = IconShinyMale,
                EggMaterial = EggMaterial,
                EggIcon = EggIcon,
                HasWorldSprites = HasWorldSprites,
                WorldIdleSpriteDown = WorldIdleSpriteDown,
                WorldIdleSpriteUp = WorldIdleSpriteUp,
                WorldIdleSpriteLeft = WorldIdleSpriteLeft,
                WorldIdleSpriteRight = WorldIdleSpriteRight,
                WorldIdleSpriteDownMale = WorldIdleSpriteDownMale,
                WorldIdleSpriteUpMale = WorldIdleSpriteUpMale,
                WorldIdleSpriteLeftMale = WorldIdleSpriteLeftMale,
                WorldIdleSpriteRightMale = WorldIdleSpriteRightMale,
                WorldWalkingSpriteDown = WorldWalkingSpriteDown.ShallowClone(),
                WorldWalkingSpriteUp = WorldWalkingSpriteUp.ShallowClone(),
                WorldWalkingSpriteLeft = WorldWalkingSpriteLeft.ShallowClone(),
                WorldWalkingSpriteRight = WorldWalkingSpriteRight.ShallowClone(),
                WorldWalkingSpriteDownMale = WorldWalkingSpriteDownMale.ShallowClone(),
                WorldWalkingSpriteUpMale = WorldWalkingSpriteUpMale.ShallowClone(),
                WorldWalkingSpriteLeftMale = WorldWalkingSpriteLeftMale.ShallowClone(),
                WorldWalkingSpriteRightMale = WorldWalkingSpriteRight.ShallowClone(),
                WorldIdleShinySpriteDown = WorldIdleShinySpriteDown,
                WorldIdleShinySpriteUp = WorldIdleShinySpriteUp,
                WorldIdleShinySpriteLeft = WorldIdleShinySpriteLeft,
                WorldIdleShinySpriteRight = WorldIdleShinySpriteRight,
                WorldIdleShinySpriteDownMale = WorldIdleShinySpriteDownMale,
                WorldIdleShinySpriteUpMale = WorldIdleShinySpriteUpMale,
                WorldIdleShinySpriteLeftMale = WorldIdleShinySpriteLeftMale,
                WorldIdleShinySpriteRightMale = WorldIdleShinySpriteRightMale,
                WorldWalkingShinySpriteDown = WorldWalkingShinySpriteDown.ShallowClone(),
                WorldWalkingShinySpriteUp = WorldWalkingShinySpriteUp.ShallowClone(),
                WorldWalkingShinySpriteLeft = WorldWalkingShinySpriteLeft.ShallowClone(),
                WorldWalkingShinySpriteRight =
                    WorldWalkingShinySpriteRight.ShallowClone(),
                WorldWalkingShinySpriteDownMale =
                    WorldWalkingShinySpriteDownMale.ShallowClone(),
                WorldWalkingShinySpriteUpMale =
                    WorldWalkingShinySpriteUpMale.ShallowClone(),
                WorldWalkingShinySpriteLeftMale =
                    WorldWalkingShinySpriteLeftMale.ShallowClone(),
                WorldWalkingShinySpriteRightMale =
                    WorldWalkingShinySpriteRightMale.ShallowClone(),
                FirstType = FirstType,
                SecondType = SecondType,
                DexDescriptionKey = DexDescriptionKey,
                Species = Species,
                Height = Height,
                Weight = Weight,
                Cry = new AudioReference {Audio = Cry.Audio},
                Abilities = Abilities.ShallowClone(),
                HiddenAbilities = HiddenAbilities.ShallowClone(),
                EVYield = EVYield.ShallowClone(),
                CatchRate = CatchRate,
                BaseFriendship = BaseFriendship,
                BaseExperience = BaseExperience,
                GrowthRate = GrowthRate,
                EggGroups = EggGroups.ShallowClone(),
                EggCycles = EggCycles,
                BreedingData = BreedingData.ShallowClone(),
                HasBinaryGender = HasBinaryGender,
                FemaleRatio = FemaleRatio,
                WildRunChance = WildRunChance,
                WildHeldItems = WildHeldItems.ShallowClone(),
                MovesByLevel = MovesByLevel.ShallowClone(),
                OnEvolutionMoves = OnEvolutionMoves.ShallowClone(),
                EggMoves = EggMoves.ShallowClone(),
                OtherLearnMoves = OtherLearnMoves.ShallowClone(),
                BaseStats = BaseStats.ShallowClone(),
                StatLimits = StatLimits.ShallowClone(),
                Evolutions = Evolutions.DeepClone(),
                IsUltraBeast = IsUltraBeast,
                IsLegendary = IsLegendary,
                IsMythical = IsMythical,
                CanLearnAnyMove = CanLearnAnyMove,
                UsesSpindaShader = UsesSpindaShader
            };

        #region InspectorUtilities

        #if UNITY_EDITOR

        /// <summary>
        /// Does it have a second type?
        /// </summary>
        private bool HasSecondType => SecondType != null;

        /// <summary>
        /// Remove the second type of the monster.
        /// </summary>
        [FoldoutGroup("$GroupTitle" + "")]
        [TitleGroup("$GroupTitle" + "/Types")]
        [HorizontalGroup("$GroupTitle" + "/Types/Second", Width = 30)]
        [Button("X", Stretch = false)]
        [ShowIf(nameof(HasSecondType))]
        private void RemoveSecondType() => SecondType = null;

        /// <summary>
        /// Initialize the base stats.
        /// Validate moves.
        /// </summary>
        public override void InspectorInit()
        {
            base.InspectorInit();

            if (BaseStats is {Count: > 0}) return;

            BaseStats = new SerializableDictionary<Stat, byte>();

            foreach (Stat stat in Utils.GetAllItems<Stat>()) BaseStats[stat] = 0;

            ValidateMoves();
        }

        /// <summary>
        /// Validate that there are no duplicated moves.
        /// </summary>
        // ReSharper disable once CyclomaticComplexity
        private void ValidateMoves()
        {
            List<Move> checkedMoves = new();

            otherWithEggOrLevelMoveDupes.Clear();
            otherMoveDupes.Clear();

            if (OtherLearnMoves != null)
                foreach (Move move in OtherLearnMoves)
                {
                    if (checkedMoves.Contains(move)) otherMoveDupes.Add(move);

                    foreach (MoveLevelPair _ in MovesByLevel.Where(pair => pair.Move == move))
                        otherWithEggOrLevelMoveDupes.Add(move);

                    if (EggMoves.Contains(move)) otherWithEggOrLevelMoveDupes.Add(move);
                    if (OnEvolutionMoves.Contains(move)) otherWithEggOrLevelMoveDupes.Add(move);

                    checkedMoves.Add(move);
                }

            checkedMoves.Clear();
            levelMoveDupes.Clear();

            if (MovesByLevel != null)
                foreach (MoveLevelPair pair in MovesByLevel)
                {
                    if (checkedMoves.Contains(pair.Move)) levelMoveDupes.Add(pair.Move);
                    checkedMoves.Add(pair.Move);
                }

            checkedMoves.Clear();
            evolutionMoveDupes.Clear();

            if (OnEvolutionMoves != null)
                foreach (Move move in OnEvolutionMoves)
                {
                    if (checkedMoves.Contains(move)) evolutionMoveDupes.Add(move);
                    checkedMoves.Add(move);
                }

            checkedMoves.Clear();
            eggMoveDupes.Clear();

            // ReSharper disable once InvertIf
            if (EggMoves != null)
                foreach (Move move in EggMoves)
                {
                    if (checkedMoves.Contains(move)) eggMoveDupes.Add(move);
                    checkedMoves.Add(move);
                }
        }

        #endif

        #endregion
    }
}