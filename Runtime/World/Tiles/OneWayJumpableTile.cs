using Varguiniano.YAPU.Runtime.Characters;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.World.Tiles
{
    /// <summary>
    /// Behaviour to attach to tiles that are jumpable.
    /// </summary>
    public class OneWayJumpableTile : WhateverBehaviour<OneWayJumpableTile>
    {
        /// <summary>
        /// Direction in which to jump.
        /// </summary>
        public CharacterController.Direction JumpDirection;
    }
}