using System;
// ReSharper disable InconsistentNaming

namespace Varguiniano.YAPU.Runtime.PokeAPI
{
    /// <summary>
    /// Class representing an ability slot in the PokeAPI.
    /// </summary>
    [Serializable]
    public class PokemonAbility
    {
        /// <summary>
        /// Is it a hidden ability?
        /// </summary>
        public bool is_hidden;

        /// <summary>
        /// Slot of this ability.
        /// </summary>
        public int slot;
        
        /// <summary>
        /// Ability resource.
        /// </summary>
        public NamedAPIResource<Ability> ability;
    }
}