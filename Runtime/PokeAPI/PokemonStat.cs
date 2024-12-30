using System;

// ReSharper disable InconsistentNaming

namespace Varguiniano.YAPU.Runtime.PokeAPI
{
    /// <summary>
    /// Class representing a Pokémon stat slot in the PokeAPI.
    /// </summary>
    [Serializable]
    public class PokemonStat
    {
        /// <summary>
        /// The stat for this slot.
        /// </summary>
        public NamedAPIResource<Stat> stat;

        /// <summary>
        /// EV value of this stat.
        /// </summary>
        public int effort;

        /// <summary>
        /// Base value of this stat.
        /// </summary>
        public int base_stat;
    }
}