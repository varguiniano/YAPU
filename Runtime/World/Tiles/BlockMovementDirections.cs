using System.Collections.Generic;
using Varguiniano.YAPU.Runtime.Characters;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.World.Tiles
{
    /// <summary>
    /// Behaviour for tiles that block the movement in certain directions.
    /// </summary>
    public class BlockMovementDirections : WhateverBehaviour<BlockMovementDirections>
    {
        /// <summary>
        /// Directions the movement is blocked in.
        /// </summary>
        public List<CharacterController.Direction> BlockedDirections;
    }
}