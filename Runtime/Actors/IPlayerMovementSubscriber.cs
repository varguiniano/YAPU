using System;
using System.Collections;
using Varguiniano.YAPU.Runtime.Characters;

namespace Varguiniano.YAPU.Runtime.Actors
{
    /// <summary>
    /// Interface defining objects that subscribe to when the player has moved.
    /// </summary>
    public interface IPlayerMovementSubscriber
    {
        /// <summary>
        /// Called each time the player moves.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player.</param>
        /// <param name="finished">Called when the routine finishes.</param>
        IEnumerator PlayerMoved(PlayerCharacter playerCharacter, Action finished);
    }
}