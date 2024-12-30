using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using WhateverDevs.Core.Editor.Utils;

namespace Varguiniano.YAPU.Editor.MonsterDatabase
{
    /// <summary>
    /// Wizard to help create monsters.
    /// It helps handling materials and textures so that the user doesn't have to do it manually.
    /// </summary>
    public class MonsterCreationHelper : OdinEditorWindow
    {
        /// <summary>
        /// Open the window.
        /// </summary>
        [MenuItem("YAPU/Monster Creation Helper")]
        private static void OpenWindow() => GetWindow<MonsterCreationHelper>().Show();

        /// <summary>
        /// Repaint the UI continuously to allow previews to move.
        /// </summary>
        private void Update() => Repaint();

        /// <summary>
        /// Reference to this window's configuration.
        /// </summary>
        public static MonsterCreationHelperConfiguration Configuration
        {
            get
            {
                if (configuration != null) return configuration;

                EditorUtility.DisplayProgressBar("Loading config...", "Loading config...", .5f);

                try
                {
                    configuration = AssetManagementUtils.FindAssetsByType<MonsterCreationHelperConfiguration>()
                                                        .First();
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }

                return configuration;
            }
        }

        /// <summary>
        /// Backfield for Configuration.
        /// </summary>
        private static MonsterCreationHelperConfiguration configuration;

        /// <summary>
        /// Try to autoselect the entry.
        /// </summary>
        [OnInspectorInit]
        private void OnInspectorInit()
        {
            if (Selection.activeObject is not MonsterEntry entry) return;

            MonsterEntry = entry;
            OnEntrySet();
            SetDexIdName();
            SetMonsterName();
        }

        /// <summary>
        /// Reference to the monster entry to create.
        /// </summary>
        [InfoBox("Reference the monster entry you have just created here.")]
        [HideIf(nameof(IsEntrySet))]
        [OnValueChanged(nameof(OnEntrySet))]
        [PropertyOrder(-100)]
        public MonsterEntry MonsterEntry;

        /// <summary>
        /// Is the monster entry set?
        /// </summary>
        private bool IsEntrySet => MonsterEntry != null;

        /// <summary>
        /// Try to get the graphics file name from the monster entry name.
        /// </summary>
        private void OnEntrySet()
        {
            string[] split = MonsterEntry.name.Split('-');

            DexNumber = split[0];
            MonsterName = split[1];
        }

        /// <summary>
        /// ID that will be used for all textures, materials...
        /// </summary>
        [ShowIf("@" + nameof(IsEntrySet) + " && !" + nameof(dexIdSet))]
        [HorizontalGroup("Dex index")]
        [PropertyOrder(-100)]
        public string DexNumber;

        /// <summary>
        /// Dex number without the 0s in front.
        /// </summary>
        public string DexNumberWithoutZeros => int.Parse(DexNumber).ToString();

        /// <summary>
        /// Flag to know if the graphics file name has been set.
        /// </summary>
        private bool dexIdSet;

        /// <summary>
        /// Set the name for the graphic files.
        /// </summary>
        [Button("Set")]
        [HorizontalGroup("Dex index")]
        [ShowIf("@" + nameof(IsEntrySet) + " && !" + nameof(dexIdSet))]
        [PropertyOrder(-100)]
        public void SetDexIdName()
        {
            if (DexNumber != null) dexIdSet = true;
        }

        /// <summary>
        /// Name that will be used for all textures, materials...
        /// </summary>
        [ShowIf("@" + nameof(IsEntrySet) + " && !" + nameof(monsterNameSet))]
        [HorizontalGroup("Name")]
        [PropertyOrder(-100)]
        public string MonsterName;

        /// <summary>
        /// Flag to know if the monster file name has been set.
        /// </summary>
        private bool monsterNameSet;

        /// <summary>
        /// Set the name for the monster files.
        /// </summary>
        [Button("Set")]
        [HorizontalGroup("Name")]
        [ShowIf("@" + nameof(IsEntrySet) + " && !" + nameof(monsterNameSet))]
        [PropertyOrder(-100)]
        public void SetMonsterName()
        {
            if (MonsterName != null) monsterNameSet = true;
        }

        /// <summary>
        /// Title of the window.
        /// </summary>
        private string Title => "Dex: " + DexNumber + ", Name: " + MonsterName + ".";

        /// <summary>
        /// Flag to know if the initial setup is done.
        /// </summary>
        private bool SetupDone => IsEntrySet && dexIdSet && monsterNameSet;

        /// <summary>
        /// Form to create the materials for.
        /// </summary>
        [Title("$" + nameof(Title))]
        [ShowIf(nameof(SetupDone))]
        [ValueDropdown(nameof(GetEntryForms))]
        [InfoBox("Forms need to be setup on the entry asset first.",
                 InfoMessageType.Warning,
                 "@" + nameof(GetEntryForms) + "().Count == 0")]
        [OnValueChanged(nameof(LoadAllSections))]
        [PropertyOrder(-100)]
        public Form Form;

        /// <summary>
        /// Get the forms available for this entry.
        /// </summary>
        /// <returns></returns>
        private List<Form> GetEntryForms() => MonsterEntry == null ? new List<Form>() : MonsterEntry.AvailableForms;

        /// <summary>
        /// Flag to know if a form has been selected.
        /// </summary>
        public bool FormSelected => SetupDone && Form != null;

        /// <summary>
        /// Button to autofill the description key.
        /// </summary>
        [FoldoutGroup("General")]
        [ShowIf(nameof(FormSelected))]
        [Button]
        [PropertyOrder(-1)]
        private void AutofillDescriptionKey()
        {
            MonsterEntry[Form].DexDescriptionKey = MonsterEntry.LocalizableName + "/" + Form.name;
            EditorUtility.SetDirty(MonsterEntry);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Section for PokeAPI data retrieval.
        /// </summary>
        [ShowIf(nameof(FormSelected))]
        [SerializeField]
        [HideLabel]
        [FoldoutGroup(nameof(PokeAPI))]
        private PokeAPISection PokeAPI;

        /// <summary>
        /// Does this form have gender variations in its graphics?
        /// </summary>
        [ShowIf(nameof(FormSelected))]
        [FoldoutGroup("Graphics")]
        [PropertySpace]
        [Tooltip("Does this form have gender variations in its graphics?")]
        [ShowInInspector]
        [OnValueChanged(nameof(LoadGraphicsSections))]
        public bool HasGenderVariations
        {
            get
            {
                if (MonsterEntry == null || Form == null) return false;

                return MonsterEntry[Form].HasBinaryGender && MonsterEntry[Form].HasMaleMaterialOverride;
            }

            set
            {
                if (value) MonsterEntry[Form].HasBinaryGender = true;

                MonsterEntry[Form].HasMaleMaterialOverride = value;
            }
        }

        /// <summary>
        /// Load all the sections.
        /// </summary>
        private void LoadAllSections()
        {
            PokeAPI = new PokeAPISection(this);

            LoadGraphicsSections();
        }

        /// <summary>
        /// Load all the graphics sections.
        /// </summary>
        private void LoadGraphicsSections()
        {
            LoadMaterialSections();
            LoadIconsSections();
            Egg = new MonsterEggSection(this);
            LoadWorldSections();
        }

        /// <summary>
        /// Load the sections for the materials.
        /// </summary>
        private void LoadMaterialSections()
        {
            Materials = new List<MonsterMaterialSection> {new(this, MaterialType.Front)};

            if (Form.HasShinyVersion) Materials.Add(new MonsterMaterialSection(this, MaterialType.FrontShiny));

            if (HasGenderVariations)
            {
                Materials.Add(new MonsterMaterialSection(this, MaterialType.FrontMale));
                if (Form.HasShinyVersion) Materials.Add(new MonsterMaterialSection(this, MaterialType.FrontShinyMale));
            }

            Materials.Add(new MonsterMaterialSection(this, MaterialType.Back));

            if (Form.HasShinyVersion) Materials.Add(new MonsterMaterialSection(this, MaterialType.BackShiny));

            if (!HasGenderVariations) return;

            Materials.Add(new MonsterMaterialSection(this, MaterialType.BackMale));
            if (Form.HasShinyVersion) Materials.Add(new MonsterMaterialSection(this, MaterialType.BackShinyMale));
        }

        /// <summary>
        /// List of sections for the materials.
        /// </summary>
        [ShowIf(nameof(FormSelected))]
        [FoldoutGroup("Graphics")]
        [ListDrawerSettings(ShowFoldout = false, IsReadOnly = true)]
        [PropertyOrder(1)]
        public List<MonsterMaterialSection> Materials;

        /// <summary>
        /// Load the sections for the icons.
        /// </summary>
        private void LoadIconsSections()
        {
            Icons = new List<MonsterIconSection> {new(this, IconType.Normal)};

            if (Form.HasShinyVersion) Icons.Add(new MonsterIconSection(this, IconType.Shiny));

            if (!HasGenderVariations) return;

            Icons.Add(new MonsterIconSection(this, IconType.NormalMale));
            if (Form.HasShinyVersion) Icons.Add(new MonsterIconSection(this, IconType.ShinyMale));
        }

        /// <summary>
        /// List of sections for the icons.
        /// </summary>
        [ShowIf(nameof(FormSelected))]
        [FoldoutGroup("Graphics")]
        [ListDrawerSettings(ShowFoldout = false, IsReadOnly = true)]
        [PropertyOrder(1)]
        public List<MonsterIconSection> Icons;

        /// <summary>
        /// Section for the egg graphics.
        /// </summary>
        [ShowIf(nameof(FormSelected))]
        [FoldoutGroup("Graphics")]
        [PropertyOrder(1)]
        [UsedImplicitly]
        public MonsterEggSection Egg;

        /// <summary>
        /// Does this form have world sprites?
        /// </summary>
        [ShowIf(nameof(FormSelected))]
        [FoldoutGroup("Graphics")]
        [PropertyOrder(1)]
        [ShowInInspector]
        public bool HasWorldSprites
        {
            get => FormSelected && MonsterEntry[Form].HasWorldSprites;
            set
            {
                MonsterEntry[Form].HasWorldSprites = value;

                EditorUtility.SetDirty(MonsterEntry);
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// Load the sections for the world sprites.
        /// </summary>
        private void LoadWorldSections()
        {
            World = new List<MonsterWorldSpritesSection> {new(this, IconType.Normal)};
            if (Form.HasShinyVersion) World.Add(new MonsterWorldSpritesSection(this, IconType.Shiny));

            if (!HasGenderVariations) return;

            World.Add(new MonsterWorldSpritesSection(this, IconType.NormalMale));
            if (Form.HasShinyVersion) World.Add(new MonsterWorldSpritesSection(this, IconType.ShinyMale));
        }

        /// <summary>
        /// List of sections for the world sprites.
        /// </summary>
        [ShowIf("@" + nameof(FormSelected) + " && " + nameof(HasWorldSprites))]
        [FoldoutGroup("Graphics")]
        [ListDrawerSettings(ShowFoldout = false, IsReadOnly = true)]
        [PropertyOrder(2)]
        public List<MonsterWorldSpritesSection> World;

        /// <summary>
        /// Clear the list of potential textures.
        /// </summary>
        [PropertyOrder(3)]
        [ShowIf("@" + nameof(FormSelected))]
        [Button]
        [FoldoutGroup("Graphics/Debug")]
        private void ClearSearchCache()
        {
            foreach (MonsterMaterialSection material in Materials) material.ClearSearchCache();

            foreach (MonsterIconSection icon in Icons) icon.ClearSearchCache();

            foreach (MonsterWorldSpritesSection section in World) section.ClearSearchCache();
        }

        /// <summary>
        /// Clear all the sprites.
        /// </summary>
        [PropertyOrder(3)]
        [ShowIf("@" + nameof(FormSelected))]
        [Button]
        [FoldoutGroup("Graphics/Debug")]
        private void ClearAllSprites()
        {
            foreach (MonsterMaterialSection material in Materials) material.ClearAllSprites();

            foreach (MonsterIconSection icon in Icons) icon.ClearAllSprites();

            Egg.ClearSprites();

            foreach (MonsterWorldSpritesSection section in World) section.ClearAllSprites();
        }
    }
}