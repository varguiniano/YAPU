using System;

// ReSharper disable InconsistentNaming

namespace Varguiniano.YAPU.Runtime.PokeAPI
{
    /// <summary>
    /// Class that stores the Pokémon species variety data received from PokeAPI.
    /// </summary>
    [Serializable]
    public class PokemonSpeciesVariety
    {
        /// <summary>
        /// Pokémon that stores the data for this variety.
        /// </summary>
        public NamedAPIResource<Pokemon> pokemon;
    }
}