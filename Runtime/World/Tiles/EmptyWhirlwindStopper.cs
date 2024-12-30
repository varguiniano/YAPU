using System;
using System.Collections;
using Varguiniano.YAPU.Runtime.Characters;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.World.Tiles
{
    /// <summary>
    /// Empty behaviour that stops the whirlwind.
    /// </summary>
    public class EmptyWhirlwindStopper : WhateverBehaviour<EmptyWhirlwindStopper>, IWhirlwindStopper
    {
        #region Not Used

        /// <summary>
        /// No blocking interaction.
        /// </summary>
        public bool HasBlockingInteraction() => false;

        /// <summary>
        /// Called by a character controller when it is about to enter a tile.
        /// </summary>
        /// <param name="characterController">Character that entered the tile.</param>
        public IEnumerator CharacterAboutToEnterTileAsync(CharacterController characterController)
        {
            yield break;
        }

        /// <summary>
        /// Called by a character controller when it enters a tile.
        /// </summary>
        /// <param name="characterController">Character that entered the tile.</param>
        public IEnumerator CharacterEnterTileAsync(CharacterController characterController)
        {
            yield break;
        }

        /// <summary>
        /// Called by a character controller when it is about to leave a tile.
        /// </summary>
        /// <param name="characterController">Character that left the tile.</param>
        public IEnumerator CharacterAboutToLeaveTileAsync(CharacterController characterController)
        {
            yield break;
        }

        /// <summary>
        /// Called by a character controller when it leaves a tile.
        /// </summary>
        /// <param name="characterController">Character that left the tile.</param>
        public IEnumerator CharacterLeftTileAsync(CharacterController characterController)
        {
            yield break;
        }

        /// <summary>
        /// Called by a character controller when it is about to enter a tile.
        /// </summary>
        /// <param name="characterController">Character that entered the tile.</param>
        /// <param name="finished">Callback used to determine if wild encounters will still be triggered.</param>
        public IEnumerator CharacterAboutToEnterTile(CharacterController characterController,
                                                     Action<bool, bool> finished)
        {
            yield break;
        }

        public IEnumerator CharacterEnterTile(CharacterController characterController, Action<bool> finished)
        {
            yield break;
        }

        /// <summary>
        /// Called by a character controller when it is about to leave a tile.
        /// </summary>
        /// <param name="characterController">Character that left the tile.</param>
        /// <param name="finished">Callback used to determine if wild encounters will still be triggered.</param>
        public IEnumerator CharacterAboutToLeaveTile(CharacterController characterController, Action<bool> finished)
        {
            yield break;
        }

        /// <summary>
        /// Called by a character controller when it leaves a tile.
        /// </summary>
        /// <param name="characterController">Character that entered the tile.</param>
        /// <param name="finished">Callback used to determine if wild encounters will still be triggered.</param>
        public IEnumerator CharacterLeftTile(CharacterController characterController, Action<bool> finished)
        {
            yield break;
        }

        #endregion
    }
}