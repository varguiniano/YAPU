using System;

// ReSharper disable InconsistentNaming

namespace Varguiniano.YAPU.Runtime.PokeAPI
{
    /// <summary>
    /// Class that stores the data for a move a Pokemon can learn in a specific game.
    /// </summary>
    [Serializable]
    public class PokemonMoveVersion
    {
        /// <summary>
        /// Level the move is learnt at.
        /// </summary>
        public int level_learned_at;

        /// <summary>
        /// Method the move is learnt with.
        /// </summary>
        public NamedAPIResource<MoveLearnMethod> move_learn_method;

        /// <summary>
        /// Version this move is learnt in.
        /// </summary>
        public NamedAPIResource<VersionGroup> version_group;
    }
}