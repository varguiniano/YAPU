using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable InconsistentNaming

namespace Varguiniano.YAPU.Runtime.PokeAPI
{
    /// <summary>
    /// Class that stores the Pokémon data received from PokeAPI.
    /// </summary>
    [Serializable]
    public class PokemonSpecies
    {
        /// <summary>
        /// Name of the mon.
        /// </summary>
        public string name;

        /// <summary>
        /// Catch rate.
        /// </summary>
        public int capture_rate;

        /// <summary>
        /// Base friendship.
        /// </summary>
        public int base_happiness;

        /// <summary>
        /// Egg cycles.
        /// </summary>
        public int hatch_counter;

        /// <summary>
        /// Rate of females in the species.
        /// -1 for genderless.
        /// </summary>
        public int gender_rate;

        /// <summary>
        /// Is the mon legendary?
        /// </summary>
        public bool is_legendary;

        /// <summary>
        /// Is the mon mythical?
        /// </summary>
        public bool is_mythical;

        /// <summary>
        /// Varieties (forms) this mon can have.
        /// </summary>
        public List<PokemonSpeciesVariety> varieties;

        /// <summary>
        /// All the genus of this mon in different languages.
        /// </summary>
        public List<Genus> genera;

        /// <summary>
        /// Descriptions this species has for each form.
        /// </summary>
        public List<Description> form_descriptions;

        /// <summary>
        /// Descriptions this species has in the dex.
        /// </summary>
        public List<FlavorText> flavor_text_entries;

        /// <summary>
        /// Growth rate of this species.
        /// </summary>
        public NamedAPIResource<GrowthRate> growth_rate;

        /// <summary>
        /// Egg groups this species belongs to.
        /// </summary>
        public List<NamedAPIResource<EggGroup>> egg_groups;

        /// <summary>
        /// Name of the Pokémon's English genus.
        /// </summary>
        public string EnglishGenus => genera.Find(genus => genus.language.Resource.ISOCode == "en-us").genus;
        
        /// <summary>
        /// Get a list of the names of the varieties this mon can take.
        /// Exclude the varieties with no data.
        /// </summary>
        public List<string> GetAllPossibleVarieties() =>
            varieties.Select(variety => variety.pokemon.Resource.name).Where(variety => PokeAPIConnector.GetPokemon(variety) != null).ToList();

        /// <summary>
        /// Get a list of the names of the forms this mon can take.
        /// Exclude the forms with no data.
        /// </summary>
        public List<string> GetAllPossibleForms() =>
            varieties.SelectMany(variety => variety.pokemon.Resource.GetPossibleForms()
                                                   .Where(form => PokeAPIConnector.GetPokemon(form) != null))
                     .ToList();
    }
}