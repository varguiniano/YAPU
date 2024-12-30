using System;
using System.Collections;
using CharacterController = Varguiniano.YAPU.Runtime.Characters.CharacterController;

namespace Varguiniano.YAPU.Runtime.World.Tiles
{
    /// <summary>
    /// Interface that defines a tile that can interact with the player.
    /// </summary>
    public interface ICharacterInteractingTile
    {
        /// <summary>
        /// Does this tile have blocking interaction?
        /// </summary>
        /// <returns></returns>
        bool HasBlockingInteraction();

        /// <summary>
        /// Called by a character controller when it is about to enter a tile.
        /// </summary>
        /// <param name="characterController">Character that entered the tile.</param>
        IEnumerator CharacterAboutToEnterTileAsync(CharacterController characterController);

        /// <summary>
        /// Called by a character controller when it enters a tile.
        /// </summary>
        /// <param name="characterController">Character that entered the tile.</param>
        IEnumerator CharacterEnterTileAsync(CharacterController characterController);

        /// <summary>
        /// Called by a character controller when it is about to leave a tile.
        /// </summary>
        /// <param name="characterController">Character that left the tile.</param>
        IEnumerator CharacterAboutToLeaveTileAsync(CharacterController characterController);

        /// <summary>
        /// Called by a character controller when it leaves a tile.
        /// </summary>
        /// <param name="characterController">Character that left the tile.</param>
        IEnumerator CharacterLeftTileAsync(CharacterController characterController);

        /// <summary>
        /// Called by a character controller when it is about to enter a tile.
        /// </summary>
        /// <param name="characterController">Character that entered the tile.</param>
        /// <param name="finished">Callback used to determine if the character can still move to that position and if wild encounters will still be triggered.</param>
        IEnumerator CharacterAboutToEnterTile(CharacterController characterController, Action<bool, bool> finished);

        /// <summary>
        /// Called by a character controller when it enters a tile.
        /// </summary>
        /// <param name="characterController">Character that entered the tile.</param>
        /// <param name="finished">Callback used to determine if wild encounters will still be triggered.</param>
        IEnumerator CharacterEnterTile(CharacterController characterController, Action<bool> finished);

        /// <summary>
        /// Called by a character controller when it is about to leave a tile.
        /// </summary>
        /// <param name="characterController">Character that left the tile.</param>
        /// <param name="finished">Callback used to determine if wild encounters will still be triggered.</param>
        IEnumerator CharacterAboutToLeaveTile(CharacterController characterController, Action<bool> finished);

        /// <summary>
        /// Called by a character controller when it leaves a tile.
        /// </summary>
        /// <param name="characterController">Character that left the tile.</param>
        /// <param name="finished">Callback used to determine if wild encounters will still be triggered.</param>
        IEnumerator CharacterLeftTile(CharacterController characterController, Action<bool> finished);
    }
}