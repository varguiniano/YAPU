using System;
using System.Collections.Generic;

// ReSharper disable InconsistentNaming

namespace Varguiniano.YAPU.Runtime.PokeAPI
{
    /// <summary>
    /// Class that holds the data to state a move a Pokémon can learn and the games it can learn it in.
    /// </summary>
    [Serializable]
    public class PokemonMove
    {
        /// <summary>
        /// Resource for that move.
        /// </summary>
        public NamedAPIResource<Move> move;

        /// <summary>
        /// Details for the version group this move is learnt in.
        /// </summary>
        public List<PokemonMoveVersion> version_group_details;

        /// <summary>
        /// Get the version group details for the given version group.
        /// </summary>
        public PokemonMoveVersion this[string versionGroupName] =>
            version_group_details.Find(version => version.version_group.Resource.name == versionGroupName);
    }
}