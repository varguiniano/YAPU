using System;

// ReSharper disable InconsistentNaming

namespace Varguiniano.YAPU.Runtime.PokeAPI
{
    /// <summary>
    /// PokeAPI class for a Pokémon Genus.
    /// This corresponds to the species in Pokémon official games.
    /// </summary>
    [Serializable]
    public class Genus
    {
        /// <summary>
        /// Localized genus name.
        /// </summary>
        public string genus;

        /// <summary>
        /// Language the genus is in.
        /// </summary>
        public NamedAPIResource<Language> language;
    }
}