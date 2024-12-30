using System;

// ReSharper disable InconsistentNaming

namespace Varguiniano.YAPU.Runtime.PokeAPI
{
    /// <summary>
    /// Class representing a gamge version.
    /// </summary>
    [Serializable]
    public class Version
    {
        /// <summary>
        /// Name of the game.
        /// </summary>
        public string name;
        
        /// <summary>
        /// Version group of this version.
        /// </summary>
        public NamedAPIResource<VersionGroup> version_group;
    }
}