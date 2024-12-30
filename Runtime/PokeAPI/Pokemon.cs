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
    public class Pokemon
    {
        /// <summary>
        /// Mon id.
        /// </summary>
        public int id;

        /// <summary>
        /// Mon name.
        /// </summary>
        public string name;

        /// <summary>
        /// The height in dm.
        /// </summary>
        public int height;

        /// <summary>
        /// Height in meters.
        /// </summary>
        public float HeightInM => height / 10f;

        /// <summary>
        /// Weight in Hg.
        /// </summary>
        public int weight;

        /// <summary>
        /// Weight in kg.
        /// </summary>
        public float WidthInKg => weight / 10f;
        
        /// <summary>
        /// Base XP yield.
        /// </summary>
        public int base_experience;

        /// <summary>
        /// Forms this Pokémon can take.
        /// </summary>
        public List<NamedAPIResource<PokemonForm>> forms;

        /// <summary>
        /// Types this Pokémon can have.
        /// </summary>
        public List<PokemonType> types;

        /// <summary>
        /// Abilities this Pokémon can have.
        /// </summary>
        public List<PokemonAbility> abilities;

        /// <summary>
        /// Moves it can learn.
        /// </summary>
        public List<PokemonMove> moves;

        /// <summary>
        /// Indexes of the Pokémon in each game.
        /// </summary>
        public List<VersionGameIndex> game_indices;

        /// <summary>
        /// The species of this mon.
        /// </summary>
        public NamedAPIResource<PokemonSpecies> species;

        /// <summary>
        /// Stats this Pokémon has.
        /// </summary>
        public List<PokemonStat> stats;

        /// <summary>
        /// Get a list of the names of the forms this mon can take.
        /// </summary>
        /// <returns></returns>
        public List<string> GetPossibleForms() => forms.Select(form => form.Resource.name).ToList();

        /// <summary>
        /// Retrieve the latests gen this mon has moves for.
        /// </summary>
        /// <returns>The order of that version group un PokeAPI.</returns>
        public VersionGroup GetLatestMoveVersionGroup() =>
            moves.SelectMany(move => move.version_group_details)
                 .Select(versionGroupResource => versionGroupResource.version_group.Resource)
                 .OrderByDescending(versionGroup => versionGroup.order)
                 .First();

        /// <summary>
        /// Get the list of available moves for the given version group.
        /// </summary>
        /// <param name="versionGroupName">Name of the version group.</param>
        /// <returns>List of strings with the move names.</returns>
        public List<PokemonMove> GetMovesForVersionGroup(string versionGroupName) =>
            moves.Where(move => move.version_group_details.Any(details => details.version_group.Resource.name
                                                                       == versionGroupName))
                 .ToList();
    }
}