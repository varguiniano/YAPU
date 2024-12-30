using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.PokeAPI;
using WhateverDevs.Core.Editor.Utils;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.DataStructures;
using Ability = Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities.Ability;
using EggGroup = Varguiniano.YAPU.Runtime.MonsterDatabase.EggGroups.EggGroup;
using Move = Varguiniano.YAPU.Runtime.MonsterDatabase.Moves.Move;
using Stat = Varguiniano.YAPU.Runtime.MonsterDatabase.Stats.Stat;

namespace Varguiniano.YAPU.Editor.MonsterDatabase
{
    /// <summary>
    /// Section of the creation helper related to retrieving data from the PokeAPI.
    /// </summary>
    [InfoBox("The cry, breeding data, wild run chance, wild held items, evolution data and is ultra beast still need to be added manually.",
             InfoMessageType.Warning)]
    [Serializable]
    public class PokeAPISection : Loggable<PokeAPISection>
    {
        /// <summary>
        /// Constructor for this section.
        /// </summary>
        /// <param name="helper">Reference to the creation helper.</param>
        public PokeAPISection(MonsterCreationHelper helper) => creationHelper = helper;

        /// <summary>
        /// Reference to the creation helper.
        /// </summary>
        private MonsterCreationHelper creationHelper;

        /// <summary>
        /// Reference to the mon's form data.
        /// </summary>
        private DataByFormEntry FormData => creationHelper.MonsterEntry[creationHelper.Form];

        /// <summary>
        /// Attempt to fill the data from PokeAPI.
        /// </summary>
        [Button(ButtonSizes.Large)]
        [HideIf("@pokeAPIForms != null && pokeAPIForms.Count > 1")]
        [PropertyOrder(-2)]
        [PropertySpace(SpaceBefore = 10, SpaceAfter = 10)]
        private void TryToFillFromPokeAPI()
        {
            EditorUtility.DisplayProgressBar("Poke API",
                                             "Retrieving data for: " + creationHelper.MonsterEntry.DexNumber + ".",
                                             .05f);

            try
            {
                PokemonSpecies pokemonSpecies =
                    PokeAPIConnector.GetPokemonSpecies(creationHelper.MonsterEntry.DexNumber);

                Logger.Info("Retrieved data for: " + pokemonSpecies.name + ".");

                Logger.Info("This mon can take the following forms:");

                pokeAPIForms = pokemonSpecies.GetAllPossibleForms();

                // Attempt it with varieties.
                if (pokeAPIForms.Count == 0) pokeAPIForms = pokemonSpecies.GetAllPossibleVarieties();

                foreach (string form in pokeAPIForms) Logger.Info(form);

                if (pokeAPIForms.Count != 1) return;

                pokeAPIForm = pokeAPIForms[0];
                RetrieveDataForForm();
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        /// <summary>
        /// Forms this monster can take according to PokeAPI.
        /// </summary>
        private List<string> pokeAPIForms;

        /// <summary>
        /// Form to select if there are multiple available forms.
        /// </summary>
        [ValueDropdown(nameof(pokeAPIForms))]
        [ShowIf("@pokeAPIForms != null && pokeAPIForms.Count > 1")]
        [ShowInInspector]
        [InfoBox("PokeAPI has multiple forms/varieties for this mon. Select which one you want to retrieve data from.")]
        [PropertyOrder(-2)]
        private string pokeAPIForm;

        /// <summary>
        /// Retrieve data from PokeAPI for the selected form.
        /// </summary>
        [Button(ButtonSizes.Large)]
        [ShowIf("@pokeAPIForms != null && pokeAPIForms.Count > 1 && pokeAPIForm != null")]
        [PropertyOrder(-1)]
        [PropertySpace(SpaceBefore = 10, SpaceAfter = 10)]
        private void RetrieveDataForForm()
        {
            if (!EditorUtility.DisplayDialog("Poke API",
                                             "This might override previously added data. Are you sure you want to continue?",
                                             "Yes",
                                             "No"))
                return;

            EditorUtility.DisplayProgressBar("Poke API",
                                             "Retrieving data for form: " + pokeAPIForm + ". This may take a while.",
                                             .1f);

            try
            {
                Pokemon pokemon = PokeAPIConnector.GetPokemon(pokeAPIForm);

                FormData.DeveloperComments = "";

                FillTypes(pokemon);
                FillGeneralDexInfo(pokemon);
                FillAbilities(pokemon);
                FillTrainingInfo(pokemon);
                FillBreedingInfo(pokemon);
                FillMoves(pokemon);
                FillStats(pokemon);
                FillOtherInfo(pokemon);

                EditorUtility.SetDirty(creationHelper.MonsterEntry);
                AssetDatabase.SaveAssets();
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        /// <summary>
        /// Get all the localized descriptions for the dex.
        /// </summary>
        /// <returns>Language and description.</returns>
        [FoldoutGroup("Utils")]
        [Button]
        [Tooltip("There may be multiple descriptions for the same language if the mon has different forms.")]
        private SerializableDictionary<string, SerializableDictionary<string, string>> GetAllLocalizedDexDescriptions()
        {
            EditorUtility.DisplayProgressBar("Poke API",
                                             "Retrieving descriptions for: "
                                           + creationHelper.MonsterEntry.DexNumber
                                           + ".",
                                             .5f);

            SerializableDictionary<string, SerializableDictionary<string, string>> descriptions = new();

            try
            {
                PokemonSpecies pokemonSpecies =
                    PokeAPIConnector.GetPokemonSpecies(creationHelper.MonsterEntry.DexNumber);

                foreach (FlavorText entry in pokemonSpecies.flavor_text_entries)
                {
                    if (!descriptions.ContainsKey(entry.version.name))
                        descriptions.Add(entry.version.name, new SerializableDictionary<string, string>());

                    descriptions[entry.version.name][entry.language.Resource.ISOCode] = entry.FlavorTextClean;
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            return descriptions;
        }

        /// <summary>
        /// Get all the localized descriptions for the dex for a single game..
        /// </summary>
        /// <returns>Language and description.</returns>
        [FoldoutGroup("Utils")]
        [Button]
        [Tooltip("There may be multiple descriptions for the same language if the mon has different forms.")]
        private SerializableDictionary<string, string> GetLocalizedDexDescriptionsForGame(string game)
        {
            EditorUtility.DisplayProgressBar("Poke API",
                                             "Retrieving descriptions for: "
                                           + creationHelper.MonsterEntry.DexNumber
                                           + ".",
                                             .5f);

            SerializableDictionary<string, string> descriptions = new();

            try
            {
                PokemonSpecies pokemonSpecies =
                    PokeAPIConnector.GetPokemonSpecies(creationHelper.MonsterEntry.DexNumber);

                foreach (FlavorText entry in
                         pokemonSpecies.flavor_text_entries.Where(entry => entry.version.name == game))
                    descriptions[entry.language.Resource.ISOCode] = entry.FlavorTextClean;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            return descriptions;
        }

        /// <summary>
        /// Get all the localized descriptions for the dex for a single game..
        /// </summary>
        /// <returns>Language and description.</returns>
        [FoldoutGroup("Utils")]
        [Button]
        private SerializableDictionary<string, string> GetLocalizedDexDescriptionsForFormAndGame(string form,
            string game)
        {
            EditorUtility.DisplayProgressBar("Poke API",
                                             "Retrieving descriptions for: "
                                           + creationHelper.MonsterEntry.DexNumber
                                           + ".",
                                             .5f);

            SerializableDictionary<string, string> descriptions = new();

            try
            {
                Pokemon pokemon = PokeAPIConnector.GetPokemon(form);

                foreach (FlavorText entry in
                         pokemon.species.Resource.flavor_text_entries.Where(entry => entry.version.name == game))
                    descriptions[entry.language.Resource.ISOCode] = entry.FlavorTextClean;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            return descriptions;
        }

        /// <summary>
        /// Get all the localized names for the species.
        /// </summary>
        /// <returns>A list of all the localized names.</returns>
        [FoldoutGroup("Utils")]
        [Button]
        private List<ObjectPair<string, string>> GetAllLocalizedNamesForSpecies()
        {
            EditorUtility.DisplayProgressBar("Poke API",
                                             "Retrieving species for: " + creationHelper.MonsterEntry.DexNumber + ".",
                                             .5f);

            List<ObjectPair<string, string>> names;

            try
            {
                PokemonSpecies pokemonSpecies =
                    PokeAPIConnector.GetPokemonSpecies(creationHelper.MonsterEntry.DexNumber);

                names = pokemonSpecies.genera.Select(genus => new ObjectPair<string, string>
                                                              {
                                                                  Key = genus.genus,
                                                                  Value = genus.language.Resource.ISOCode
                                                              })
                                      .ToList();
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            return names;
        }

        /// <summary>
        /// Fill the mon types based on the retrieved data.
        /// </summary>
        /// <param name="pokemon">Retrieved data for this mon.</param>
        private void FillTypes(Pokemon pokemon)
        {
            EditorUtility.DisplayProgressBar("Poke API",
                                             "Retrieving types for form: " + pokeAPIForm + ". This may take a while.",
                                             .15f);

            FormData.DeveloperComments += "-Types retrieved from PokeAPI for form " + pokeAPIForm + ".";

            FormData.FirstType =
                MonsterCreationHelper.Configuration.PokeAPITypeToYAPUType(pokemon.types.First(type => type.slot == 1)
                                                                             .type.Resource.name);

            FormData.SecondType =
                pokemon.types.Count > 1
             && pokemon.types.FirstOrDefault(type => type.slot == 2) != null
             && !pokemon.types.First(type => type.slot == 2).type.Resource.name.IsNullEmptyOrWhiteSpace()
                    ? MonsterCreationHelper.Configuration
                                           .PokeAPITypeToYAPUType(pokemon.types.First(type => type.slot == 2)
                                                                         .type.Resource.name)
                    : null;
        }

        /// <summary>
        /// Fill the mon species based on the retrieved data.
        /// </summary>
        /// <param name="pokemon">Retrieved data for this mon.</param>
        private void FillGeneralDexInfo(Pokemon pokemon)
        {
            EditorUtility.DisplayProgressBar("Poke API",
                                             "Retrieving general dex info for form: "
                                           + pokeAPIForm
                                           + ". This may take a while.",
                                             .2f);

            FormData.DeveloperComments += "\n-General dex info retrieved from PokeAPI for form " + pokeAPIForm + ".";

            FormData.Species =
                MonsterCreationHelper.Configuration.PokeAPIGenusToYAPUSpecies(pokemon.species.Resource.EnglishGenus);

            FormData.Height = pokemon.HeightInM;
            FormData.Weight = pokemon.WidthInKg;
        }

        /// <summary>
        /// Fill the mon abilities based on the retrieved data.
        /// </summary>
        /// <param name="pokemon">Retrieved data for this mon.</param>
        private void FillAbilities(Pokemon pokemon)
        {
            EditorUtility.DisplayProgressBar("Poke API",
                                             "Retrieving abilities for form: "
                                           + pokeAPIForm
                                           + ". This may take a while.",
                                             .25f);

            FormData.DeveloperComments += "\n-Abilities retrieved from PokeAPI for form " + pokeAPIForm + ".";

            FormData.Abilities.Clear();
            FormData.HiddenAbilities.Clear();

            foreach (PokemonAbility pokemonAbility in pokemon.abilities.OrderBy(slot => slot.slot))
            {
                Ability ability = MonsterCreationHelper.Configuration.PokeAPIAbilityToYAPUAbility(pokemonAbility
                   .ability.name);

                if (ability == null) continue;

                if (pokemonAbility.is_hidden)
                    FormData.HiddenAbilities.Add(ability);
                else
                    FormData.Abilities.Add(ability);
            }
        }

        /// <summary>
        /// Fill the mon training info based on the retrieved data.
        /// </summary>
        /// <param name="pokemon">Retrieved data for this mon.</param>
        private void FillTrainingInfo(Pokemon pokemon)
        {
            EditorUtility.DisplayProgressBar("Poke API",
                                             "Retrieving training info for form: "
                                           + pokeAPIForm
                                           + ". This may take a while.",
                                             .3f);

            FormData.DeveloperComments += "\n-Training info retrieved from PokeAPI for form " + pokeAPIForm + ".";

            FormData.CatchRate = (byte) pokemon.species.Resource.capture_rate;

            FormData.BaseFriendship = (byte) pokemon.species.Resource.base_happiness;

            FormData.BaseExperience = (uint) pokemon.base_experience;

            FormData.GrowthRate =
                MonsterCreationHelper.Configuration.PokeAPIGrowthRateToYAPUGrowthRate(pokemon.species.Resource
                   .growth_rate.name);
        }

        /// <summary>
        /// Fill the mon breeding info based on the retrieved data.
        /// </summary>
        /// <param name="pokemon">Retrieved data for this mon.</param>
        private void FillBreedingInfo(Pokemon pokemon)
        {
            EditorUtility.DisplayProgressBar("Poke API",
                                             "Retrieving breeding info for form: "
                                           + pokeAPIForm
                                           + ". This may take a while.",
                                             .35f);

            FormData.DeveloperComments += "\n-Breeding info retrieved from PokeAPI for form " + pokeAPIForm + ".";

            FormData.EggGroups.Clear();

            foreach (EggGroup eggGroup in pokemon.species.Resource.egg_groups
                                                 .Select(groupResource =>
                                                             MonsterCreationHelper.Configuration
                                                                .PokeAPIEggGroupToYAPUEggGroup(groupResource.Resource
                                                                    .name))
                                                 .Where(eggGroup => eggGroup != null))
                FormData.EggGroups.Add(eggGroup);

            FormData.EggCycles = (byte) pokemon.species.Resource.hatch_counter;

            int femaleRate = pokemon.species.Resource.gender_rate;

            if (femaleRate == -1)
            {
                FormData.HasBinaryGender = false;
                FormData.HasMaleMaterialOverride = false;
            }
            else
            {
                FormData.HasBinaryGender = true;
                FormData.FemaleRatio = femaleRate / 8f;
            }
        }

        /// <summary>
        /// Fill the mon moves based on the retrieved data.
        /// </summary>
        /// <param name="pokemon">Retrieved data for this mon.</param>
        private void FillMoves(Pokemon pokemon)
        {
            EditorUtility.DisplayProgressBar("Poke API",
                                             "Retrieving moves for form: " + pokeAPIForm + ". This may take a while.",
                                             .4f);

            string latestVersionGroup = pokemon.GetLatestMoveVersionGroup().name;

            /*Logger.Info(pokeAPIForm
                      + " latest version group moves are for "
                      + latestVersionGroup
                      + ". These moves are:");*/

            FormData.DeveloperComments += "\n-Moves retrieved from PokeAPI for form "
                                        + pokeAPIForm
                                        + " and game version "
                                        + latestVersionGroup
                                        + ".";

            List<Move> movesInYAPU = AssetManagementUtils.FindAssetsByType<Move>();

            FormData.MovesByLevel.Clear();
            FormData.OnEvolutionMoves.Clear();
            FormData.EggMoves.Clear();
            FormData.ClearOtherLearnMoves();

            foreach (PokemonMove move in pokemon.GetMovesForVersionGroup(latestVersionGroup)
                                                .OrderBy(move => move[latestVersionGroup].move_learn_method.Resource
                                                            .name))
            {
                string learnMethod = move[latestVersionGroup].move_learn_method.Resource.name;

                /*Logger.Info(move.move.Resource.name
                          + " learnt by "
                          + learnMethod
                          + (MonsterCreationHelper.Configuration.PokeAPILevelUpMoveCategories.Contains(learnMethod)
                                 ? ". Level: " + move[latestVersionGroup].level_learned_at
                                 : "")
                          + ".");*/

                Move yapuMove = MonsterCreationHelper.Configuration.PokeAPIMoveToYAPUMove(move.move.name, movesInYAPU);

                if (yapuMove == null) continue;

                if (MonsterCreationHelper.Configuration.PokeAPILevelUpMoveCategories.Contains(learnMethod))
                {
                    byte level = (byte) move[latestVersionGroup].level_learned_at;

                    // If the level is 0, it means it's learnt upon evolving.
                    if (level > 0)
                        FormData.MovesByLevel.Add(new MoveLevelPair
                                                  {
                                                      Move = yapuMove,
                                                      Level = level
                                                  });
                    else
                        FormData.OnEvolutionMoves.Add(yapuMove);
                }
                else if (MonsterCreationHelper.Configuration.PokeAPIOnEvolutionMoveCategories.Contains(learnMethod))
                    FormData.OnEvolutionMoves.Add(yapuMove);
                else if (MonsterCreationHelper.Configuration.PokeAPIEggMoveCategories.Contains(learnMethod))
                    FormData.EggMoves.Add(yapuMove);
                else if (MonsterCreationHelper.Configuration.PokeAPIOtherMoveCategories.Contains(learnMethod))
                    FormData.AddOtherLearnMove(yapuMove);
                else
                {
                    Logger.Error("Unknown move learn method: " + learnMethod + ".");

                    EditorUtility.DisplayDialog("Poke API",
                                                "Learn method "
                                              + learnMethod
                                              + " is unknown. Please add it to a category in the configuration. Move "
                                              + move.move.name
                                              + " has this category and will be ignored.",
                                                "Ok");

                    continue;
                }

                // Order by level.
                FormData.MovesByLevel = creationHelper.MonsterEntry[creationHelper.Form]
                                                      .MovesByLevel.OrderBy(slot => slot.Level)
                                                      .ToList();

                // Remove duplicates.
                List<Move> duplicates = new();

                // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
                foreach (Move otherMove in FormData.GetOtherLearnMovesInEditor())
                    if (FormData.MovesByLevel.Any(slot => slot.Move == otherMove)
                     || FormData.EggMoves.Contains(otherMove)
                     || FormData.OnEvolutionMoves.Contains(otherMove))
                        duplicates.Add(otherMove);

                FormData.RemoveOtherLearnMoves(duplicates);
            }
        }

        /// <summary>
        /// Fill the mon stat data based on the retrieved data.
        /// </summary>
        /// <param name="pokemon">Retrieved data for this mon.</param>
        private void FillStats(Pokemon pokemon)
        {
            EditorUtility.DisplayProgressBar("Poke API",
                                             "Retrieving stat data for form: "
                                           + pokeAPIForm
                                           + ". This may take a while.",
                                             .45f);

            FormData.DeveloperComments += "\n-Stat data retrieved from PokeAPI for form " + pokeAPIForm + ".";

            FormData.EVYield.Clear();

            foreach (PokemonStat stat in pokemon.stats)
            {
                Stat yapuStat = MonsterCreationHelper.Configuration.PokeAPIStatToYAPUStat(stat.stat.name);

                FormData.BaseStats[yapuStat] = (byte) stat.base_stat;

                if (stat.effort > 0)
                    FormData.EVYield.Add(new StatByteValuePair
                                         {
                                             Stat = yapuStat,
                                             Value = (byte) stat.effort
                                         });
            }
        }

        /// <summary>
        /// Fill the mon other info based on the retrieved data.
        /// </summary>
        /// <param name="pokemon">Retrieved data for this mon.</param>
        private void FillOtherInfo(Pokemon pokemon)
        {
            EditorUtility.DisplayProgressBar("Poke API",
                                             "Retrieving other info for form: "
                                           + pokeAPIForm
                                           + ". This may take a while.",
                                             .5f);

            FormData.DeveloperComments += "\n-Other info retrieved from PokeAPI for form " + pokeAPIForm + ".";

            FormData.IsLegendary = pokemon.species.Resource.is_legendary;
            FormData.IsMythical = pokemon.species.Resource.is_mythical;
        }
    }
}