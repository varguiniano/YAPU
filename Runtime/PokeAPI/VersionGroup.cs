using System;

// ReSharper disable InconsistentNaming

namespace Varguiniano.YAPU.Runtime.PokeAPI
{
    /// <summary>
    /// Group that represents a generation of games.
    /// </summary>
    [Serializable]
    public class VersionGroup
    {
        /// <summary>
        /// Name of the version.
        /// </summary>
        public string name;

        /// <summary>
        /// Order of the game releases.
        /// </summary>
        public int order;
    }
}