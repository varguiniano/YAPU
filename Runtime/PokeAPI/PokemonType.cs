using System;

// ReSharper disable InconsistentNaming

namespace Varguiniano.YAPU.Runtime.PokeAPI
{
    /// <summary>
    /// PokeAPI class for the Pokémon type.
    /// </summary>
    [Serializable]
    public class PokemonType
    {
        /// <summary>
        /// Slot of the type.
        /// </summary>
        public int slot;

        /// <summary>
        /// Type resource.
        /// </summary>
        public NamedAPIResource<Type> type;
    }
}