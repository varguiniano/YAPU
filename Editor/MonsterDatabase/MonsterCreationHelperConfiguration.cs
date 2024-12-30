using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using Varguiniano.YAPU.Runtime.MonsterDatabase.EggGroups;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Experience;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Species;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Editor.Utils;
using WhateverDevs.Core.Runtime.DataStructures;
using Utils = WhateverDevs.Core.Runtime.Common.Utils;

namespace Varguiniano.YAPU.Editor.MonsterDatabase
{
    /// <summary>
    /// Data class for the configuration used by the monster creation helper.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/MonsterCreationHelperConfig",
                     fileName = "MonsterCreationHelperConfig")]
    public class MonsterCreationHelperConfiguration : WhateverScriptable<MonsterCreationHelperConfiguration>
    {
        /// <summary>
        /// Path to the materials folder.
        /// </summary>
        [FoldoutGroup("Graphics")]
        [FolderPath(RequireExistingPath = true)]
        public string FrontMaterialsPath;

        /// <summary>
        /// Path to the materials folder.
        /// </summary>
        [FoldoutGroup("Graphics")]
        [FolderPath(RequireExistingPath = true)]
        public string BackMaterialsPath;

        /// <summary>
        /// Path to the materials folder.
        /// </summary>
        [FoldoutGroup("Graphics")]
        [FolderPath(RequireExistingPath = true)]
        public string FrontShinyMaterialsPath;

        /// <summary>
        /// Path to the materials folder.
        /// </summary>
        [FolderPath(RequireExistingPath = true)]
        [FoldoutGroup("Graphics")]
        public string BackShinyMaterialsPath;

        /// <summary>
        /// Path to the materials folder.
        /// </summary>
        [FolderPath(RequireExistingPath = true)]
        [FoldoutGroup("Graphics")]
        public string FrontTexturesPath;

        /// <summary>
        /// Path to the materials folder.
        /// </summary>
        [FolderPath(RequireExistingPath = true)]
        [FoldoutGroup("Graphics")]
        public string BackTexturesPath;

        /// <summary>
        /// Path to the materials folder.
        /// </summary>
        [FolderPath(RequireExistingPath = true)]
        [FoldoutGroup("Graphics")]
        public string FrontShinyTexturesPath;

        /// <summary>
        /// Path to the materials folder.
        /// </summary>
        [FolderPath(RequireExistingPath = true)]
        [FoldoutGroup("Graphics")]
        public string BackShinyTexturesPath;

        /// <summary>
        /// Path to the icons folder.
        /// </summary>
        [FolderPath(RequireExistingPath = true)]
        [FoldoutGroup("Graphics")]
        public string IconsPath;

        /// <summary>
        /// Path to the icons folder.
        /// </summary>
        [FolderPath(RequireExistingPath = true)]
        [FoldoutGroup("Graphics")]
        public string MaleIconsPath;

        /// <summary>
        /// Path to the icons folder.
        /// </summary>
        [FolderPath(RequireExistingPath = true)]
        [FoldoutGroup("Graphics")]
        public string ShinyIconsPath;

        /// <summary>
        /// Path to the icons folder.
        /// </summary>
        [FolderPath(RequireExistingPath = true)]
        [FoldoutGroup("Graphics")]
        public string ShinyMaleIconsPath;

        /// <summary>
        /// Path to the icons folder.
        /// </summary>
        [FolderPath(RequireExistingPath = true)]
        [FoldoutGroup("Graphics")]
        public string WorldPath;

        /// <summary>
        /// Path to the icons folder.
        /// </summary>
        [FolderPath(RequireExistingPath = true)]
        [FoldoutGroup("Graphics")]
        public string MaleWorldPath;

        /// <summary>
        /// Path to the icons folder.
        /// </summary>
        [FolderPath(RequireExistingPath = true)]
        [FoldoutGroup("Graphics")]
        public string ShinyWorldPath;

        /// <summary>
        /// Path to the icons folder.
        /// </summary>
        [FolderPath(RequireExistingPath = true)]
        [FoldoutGroup("Graphics")]
        public string ShinyMaleWorldPath;

        /// <summary>
        /// Shader to use with all monster materials.
        /// </summary>
        [FoldoutGroup("Graphics")]
        public Shader MaterialShader;

        /// <summary>
        /// Preset to use for monster textures.
        /// </summary>
        [FoldoutGroup("Graphics")]
        public Preset MonsterTexturePreset;

        /// <summary>
        /// Preset to use for the icons.
        /// </summary>
        [FoldoutGroup("Graphics")]
        public Preset MonsterIconPreset;

        /// <summary>
        /// Preset to use for the shiny icons.
        /// </summary>
        [FoldoutGroup("Graphics")]
        public Preset ShinyMonsterIconPreset;

        /// <summary>
        /// Preset to use for world sprites.
        /// </summary>
        [FoldoutGroup("Graphics")]
        public Preset MonsterWorldSpritesPreset;

        /// <summary>
        /// Default material to use for eggs.
        /// </summary>
        [FoldoutGroup("Graphics")]
        public Material DefaultEggMaterial;

        /// <summary>
        /// Default icon to use for eggs.
        /// </summary>
        [FoldoutGroup("Graphics")]
        public Sprite DefaultEggIcon;

        /// <summary>
        /// Suffix to use when creating the materials for each form.
        /// </summary>
        [FoldoutGroup("Graphics")]
        public SerializableDictionary<Form, string> FormSuffixes;

        /// <summary>
        /// Suffix to use when creating the materials for each type.
        /// </summary>
        [FoldoutGroup("Graphics")]
        public SerializableDictionary<MaterialType, string> TypeSuffixes;

        /// <summary>
        /// Suffix to use when creating the icons for each type.
        /// </summary>
        [FoldoutGroup("Graphics")]
        public SerializableDictionary<IconType, string> IconSuffixes;

        /// <summary>
        /// Equivalence between the names of the languages in the PokeAPI and the languages in the database.
        /// </summary>
        [FoldoutGroup("PokeAPI")]
        public SerializableDictionary<string, string> PokeAPILanguageToYAPULanguage;

        /// <summary>
        /// Equivalences between the names of the genus in the PokeAPI and the species in the database.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("PokeAPI")]
        private SerializableDictionary<string, MonsterSpecies> PokeAPIGenusToYAPUSpeciesEquivalences;

        /// <summary>
        /// Convert a genus name from the PokeAPI to a species in the database.
        /// </summary>
        public MonsterSpecies PokeAPIGenusToYAPUSpecies(string pokeAPIName)
        {
            if (PokeAPIGenusToYAPUSpeciesEquivalences.TryGetValue(pokeAPIName, out MonsterSpecies yapuSpecies))
                return yapuSpecies;

            string pokeAPICleanName = CleanName(pokeAPIName);

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (MonsterSpecies species in AssetManagementUtils.FindAssetsByType<MonsterSpecies>())
            {
                string speciesCleanName = CleanName(species.name);

                if (speciesCleanName != pokeAPICleanName) continue;

                if (!EditorUtility.DisplayDialog("PokeAPI",
                                                 "Does species " + pokeAPIName + " correspond to " + species.name + "?",
                                                 "Yes",
                                                 "No"))
                    continue;

                PokeAPIGenusToYAPUSpeciesEquivalences[pokeAPIName] = species;

                EditorUtility.SetDirty(this);

                return species;
            }

            EditorUtility.DisplayDialog("PokeAPI",
                                        "Species " + pokeAPIName + " was not found in YAPU.",
                                        "Ok");

            return null;
        }

        /// <summary>
        /// Equivalences between the names of the types in the PokeAPI and the types in the database.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("PokeAPI")]
        private SerializableDictionary<string, MonsterType> PokeAPINameToTypeEquivalences;

        /// <summary>
        /// Convert a type name from the PokeAPI to a type in the database.
        /// </summary>
        public MonsterType PokeAPITypeToYAPUType(string pokeAPIName)
        {
            if (PokeAPINameToTypeEquivalences.TryGetValue(pokeAPIName, out MonsterType yapuType)) return yapuType;

            string pokeAPICleanName = CleanName(pokeAPIName);

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (MonsterType type in AssetManagementUtils.FindAssetsByType<MonsterType>())
            {
                string typeCleanName = CleanName(type.name);

                if (typeCleanName != pokeAPICleanName) continue;

                if (!EditorUtility.DisplayDialog("PokeAPI",
                                                 "Does type " + pokeAPIName + " correspond to " + type.name + "?",
                                                 "Yes",
                                                 "No"))
                    continue;

                PokeAPINameToTypeEquivalences[pokeAPIName] = type;

                EditorUtility.SetDirty(this);

                return type;
            }

            return null;
        }

        /// <summary>
        /// Move categories from the PokeAPI that should be considered as level up moves.
        /// </summary>
        [FoldoutGroup("PokeAPI")]
        public List<string> PokeAPILevelUpMoveCategories;

        /// <summary>
        /// Move categories from the PokeAPI that should be considered as on evolution moves.
        /// </summary>
        [FoldoutGroup("PokeAPI")]
        public List<string> PokeAPIOnEvolutionMoveCategories;

        /// <summary>
        /// Move categories from the PokeAPI that should be considered as egg moves.
        /// </summary>
        [FoldoutGroup("PokeAPI")]
        public List<string> PokeAPIEggMoveCategories;

        /// <summary>
        /// Move categories from the PokeAPI that should be considered as other moves.
        /// </summary>
        [FoldoutGroup("PokeAPI")]
        public List<string> PokeAPIOtherMoveCategories;

        /// <summary>
        /// Equivalences between the names of the moves in the PokeAPI and the moves in the database.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("PokeAPI")]
        private SerializableDictionary<string, Move> PokeAPINameToMoveEquivalences;

        /// <summary>
        /// Moves retrieved from PokeAPI that should be ignored.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("PokeAPI")]
        private List<string> IgnoredPokeAPIMoves;

        /// <summary>
        /// Translate a move name from the PokeAPI to a move in the database.
        /// </summary>
        /// <param name="pokeAPIName">Name of the move in PokeAPI.</param>
        /// <param name="movesInYAPU">List of all moves in YPAU. Useful as a cached if this method is being called multiple times.</param>
        /// <returns>The move in the database.</returns>
        public Move PokeAPIMoveToYAPUMove(string pokeAPIName, List<Move> movesInYAPU = null)
        {
            if (PokeAPINameToMoveEquivalences.TryGetValue(pokeAPIName, out Move yapuMove)) return yapuMove;
            if (IgnoredPokeAPIMoves.Contains(pokeAPIName)) return null;

            movesInYAPU ??= AssetManagementUtils.FindAssetsByType<Move>();

            string pokeAPICleanName = CleanName(pokeAPIName);

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (Move move in movesInYAPU)
            {
                string moveCleanName = CleanName(move.name);

                if (moveCleanName != pokeAPICleanName) continue;

                if (!EditorUtility.DisplayDialog("PokeAPI",
                                                 "Does move " + pokeAPIName + " correspond to " + move.name + "?",
                                                 "Yes",
                                                 "No"))
                    continue;

                PokeAPINameToMoveEquivalences[pokeAPIName] = move;

                EditorUtility.SetDirty(this);

                return move;
            }

            if (!EditorUtility.DisplayDialog("PokeAPI",
                                             "Move " + pokeAPIName + " was not found in YAPU.",
                                             "Ignore move forever",
                                             "Ok"))
                return null;

            IgnoredPokeAPIMoves.Add(pokeAPIName);
            EditorUtility.SetDirty(this);

            return null;
        }

        /// <summary>
        /// Equivalences between the names of the abilities in the PokeAPI and the abilities in the database.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("PokeAPI")]
        private SerializableDictionary<string, Ability> PokeAPINameToAbilityEquivalences;

        /// <summary>
        /// Translate an abiity name from the PokeAPI to an ability in the database.
        /// </summary>
        /// <param name="pokeAPIName">Name of the ability in PokeAPI.</param>
        /// <returns>The ability in the database.</returns>
        public Ability PokeAPIAbilityToYAPUAbility(string pokeAPIName)
        {
            if (PokeAPINameToAbilityEquivalences.TryGetValue(pokeAPIName, out Ability yapuAbility)) return yapuAbility;

            string pokeAPICleanName = CleanName(pokeAPIName);

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (Ability ability in AssetManagementUtils.FindAssetsByType<Ability>())
            {
                string abilityCleanName = CleanName(ability.name);

                if (abilityCleanName != pokeAPICleanName) continue;

                if (!EditorUtility.DisplayDialog("PokeAPI",
                                                 "Does ability " + pokeAPIName + " correspond to " + ability.name + "?",
                                                 "Yes",
                                                 "No"))
                    continue;

                PokeAPINameToAbilityEquivalences[pokeAPIName] = ability;

                EditorUtility.SetDirty(this);

                return ability;
            }

            EditorUtility.DisplayDialog("PokeAPI",
                                        "Ability " + pokeAPIName + " was not found in YAPU.",
                                        "Ok");

            return null;
        }

        /// <summary>
        /// Equivalences between the names of the growth rates in the PokeAPI and the growth rates in the database.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("PokeAPI")]
        private SerializableDictionary<string, GrowthRate> PokeAPINameToGrowthRateEquivalences;

        /// <summary>
        /// Translate an GrowthRate name from the PokeAPI to an GrowthRate in the database.
        /// </summary>
        /// <param name="pokeAPIName">Name of the GrowthRate in PokeAPI.</param>
        /// <returns>The GrowthRate in the database.</returns>
        public GrowthRate PokeAPIGrowthRateToYAPUGrowthRate(string pokeAPIName)
        {
            if (PokeAPINameToGrowthRateEquivalences.TryGetValue(pokeAPIName, out GrowthRate yapuGrowthRate))
                return yapuGrowthRate;

            string pokeAPICleanName = CleanName(pokeAPIName);

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (GrowthRate growthRate in Utils.GetAllItems<GrowthRate>())
            {
                string rateCleanName = CleanName(growthRate.ToString());

                if (rateCleanName != pokeAPICleanName) continue;

                if (!EditorUtility.DisplayDialog("PokeAPI",
                                                 "Does Growth rate "
                                               + pokeAPIName
                                               + " correspond to "
                                               + growthRate
                                               + "?",
                                                 "Yes",
                                                 "No"))
                    continue;

                PokeAPINameToGrowthRateEquivalences[pokeAPIName] = growthRate;

                EditorUtility.SetDirty(this);

                return growthRate;
            }

            EditorUtility.DisplayDialog("PokeAPI",
                                        "Growth rate " + pokeAPIName + " was not found in YAPU.",
                                        "Ok");

            return default;
        }

        /// <summary>
        /// Equivalences between the names of the EggGroup in the PokeAPI and the EggGroup in the database.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("PokeAPI")]
        private SerializableDictionary<string, EggGroup> PokeAPINameToEggGroupEquivalences;

        /// <summary>
        /// Translate an EggGroup name from the PokeAPI to an EggGroup in the database.
        /// </summary>
        /// <param name="pokeAPIName">Name of the EggGroup in PokeAPI.</param>
        /// <returns>The EggGroup in the database.</returns>
        public EggGroup PokeAPIEggGroupToYAPUEggGroup(string pokeAPIName)
        {
            if (PokeAPINameToEggGroupEquivalences.TryGetValue(pokeAPIName, out EggGroup yapuEggGroup))
                return yapuEggGroup;

            string pokeAPICleanName = CleanName(pokeAPIName);

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (EggGroup eggGroup in AssetManagementUtils.FindAssetsByType<EggGroup>())
            {
                string groupCleanName = CleanName(eggGroup.name);

                if (groupCleanName != pokeAPICleanName) continue;

                if (!EditorUtility.DisplayDialog("PokeAPI",
                                                 "Does egg group "
                                               + pokeAPIName
                                               + " correspond to "
                                               + eggGroup.name
                                               + "?",
                                                 "Yes",
                                                 "No"))
                    continue;

                PokeAPINameToEggGroupEquivalences[pokeAPIName] = eggGroup;

                EditorUtility.SetDirty(this);

                return eggGroup;
            }

            EditorUtility.DisplayDialog("PokeAPI",
                                        "Egg group " + pokeAPIName + " was not found in YAPU.",
                                        "Ok");

            return null;
        }

        /// <summary>
        /// Equivalences between the names of the Stat in the PokeAPI and the Stat in the database.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("PokeAPI")]
        private SerializableDictionary<string, Stat> PokeAPINameToStatEquivalences;

        /// <summary>
        /// Translate a Stat name from the PokeAPI to a Stat in the database.
        /// </summary>
        /// <param name="pokeAPIName">Name of the Stat in PokeAPI.</param>
        /// <returns>The Stat in the database.</returns>
        public Stat PokeAPIStatToYAPUStat(string pokeAPIName)
        {
            if (PokeAPINameToStatEquivalences.TryGetValue(pokeAPIName, out Stat yapuStat)) return yapuStat;

            string pokeAPICleanName = CleanName(pokeAPIName);

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (Stat stat in Utils.GetAllItems<Stat>())
            {
                string rateCleanName = CleanName(stat.ToString());

                if (rateCleanName != pokeAPICleanName) continue;

                if (!EditorUtility.DisplayDialog("PokeAPI",
                                                 "Does stat "
                                               + pokeAPIName
                                               + " correspond to "
                                               + stat
                                               + "?",
                                                 "Yes",
                                                 "No"))
                    continue;

                PokeAPINameToStatEquivalences[pokeAPIName] = stat;

                EditorUtility.SetDirty(this);

                return stat;
            }

            EditorUtility.DisplayDialog("PokeAPI",
                                        "Stat " + pokeAPIName + " was not found in YAPU.",
                                        "Ok");

            return default;
        }

        /// <summary>
        /// Populate the form suffixes with the default values.
        /// </summary>
        [OnInspectorInit]
        private void OnInspectorInit()
        {
            foreach (Form form in AssetManagementUtils.FindAssetsByType<Form>()
                                                      .Where(form => !form.IsShiny && !FormSuffixes.ContainsKey(form)))
                FormSuffixes[form] = form.name[..1].ToLower();

            foreach (MaterialType type in Utils.GetAllItems<MaterialType>()
                                               .Where(type => !TypeSuffixes.ContainsKey(type)))
                TypeSuffixes[type] = type.ToString().ToLower();

            foreach (IconType type in Utils.GetAllItems<IconType>()
                                           .Where(type => !IconSuffixes.ContainsKey(type)))
                IconSuffixes[type] = type.ToString().ToLower();
        }

        /// <summary>
        /// Take a dirty name and clean it.
        /// </summary>
        /// <param name="dirtyName">Name with spaces, slashes, accents...</param>
        /// <returns>Clean name.</returns>
        private static string CleanName(string dirtyName) =>
            RemoveDiacritics(dirtyName.Replace("-", "").Replace(" ", "").ToLower());

        /// <summary>
        /// Remove the diacritics from a string.
        /// https://stackoverflow.com/questions/249087/how-do-i-remove-diacritics-accents-from-a-string-in-net
        /// </summary>
        private static string RemoveDiacritics(string text)
        {
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new(capacity: normalizedString.Length);

            for (int i = 0; i < normalizedString.Length; i++)
            {
                char c = normalizedString[i];
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark) stringBuilder.Append(c);
            }

            return stringBuilder
                  .ToString()
                  .Normalize(NormalizationForm.FormC);
        }
    }
}