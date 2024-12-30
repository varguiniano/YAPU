using System;

// ReSharper disable InconsistentNaming

namespace Varguiniano.YAPU.Runtime.PokeAPI
{
    /// <summary>
    /// Class representing the index of a Pokémon in a game.
    /// </summary>
    [Serializable]
    public class VersionGameIndex
    {
        /// <summary>
        /// The index in that game.
        /// </summary>
        public int game_index;

        /// <summary>
        /// Game version.
        /// </summary>
        public NamedAPIResource<Version> version;
    }
}