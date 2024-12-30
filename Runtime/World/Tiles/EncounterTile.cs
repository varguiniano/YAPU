using Varguiniano.YAPU.Runtime.World.Encounters;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.World.Tiles
{
    /// <summary>
    /// Class that defines a possible encounter in this tile.
    /// </summary>
    public class EncounterTile : WhateverBehaviour<EncounterTile>
    {
        /// <summary>
        /// Type of encounter this tile has.
        /// </summary>
        public EncounterType EncounterType;
    }
}