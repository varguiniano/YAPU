using System;

// ReSharper disable InconsistentNaming

namespace Varguiniano.YAPU.Runtime.PokeAPI
{
    /// <summary>
    /// Class representing a language in the PokeAPI.
    /// </summary>
    [Serializable]
    public class Language
    {
        /// <summary>
        /// Two-letter code of the country.
        /// </summary>
        public string iso639;

        /// <summary>
        /// Two-letter code of the language.
        /// </summary>
        public string iso3166;
        
        /// <summary>
        /// ISO code of the language.
        /// </summary>
        public string ISOCode => iso639 + "-" + iso3166;
    }
}