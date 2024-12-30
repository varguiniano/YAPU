using System;

// ReSharper disable InconsistentNaming

namespace Varguiniano.YAPU.Runtime.PokeAPI
{
    /// <summary>
    /// PokeAPI class for a mon description.
    /// </summary>
    [Serializable]
    public class Description
    {
        /// <summary>
        /// Description text.
        /// </summary>
        public string description;

        /// <summary>
        /// Language the description is in.
        /// </summary>
        public NamedAPIResource<Language> language;
    }
}