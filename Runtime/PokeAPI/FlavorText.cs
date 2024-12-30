using System;

// ReSharper disable InconsistentNaming

namespace Varguiniano.YAPU.Runtime.PokeAPI
{
    /// <summary>
    /// PokeAPI class for a flavor text in the dex.
    /// </summary>
    [Serializable]
    public class FlavorText
    {
        /// <summary>
        /// Flavor text displayed.
        /// </summary>
        public string flavor_text;
        
        /// <summary>
        /// Clean version of the flavor text, with no line breaks.
        /// </summary>
        public string FlavorTextClean => flavor_text.Replace("\n", " ");

        /// <summary>
        /// Language the text is in.
        /// </summary>
        public NamedAPIResource<Language> language;

        /// <summary>
        /// Game version of this text.
        /// </summary>
        public NamedAPIResource<Version> version;
    }
}