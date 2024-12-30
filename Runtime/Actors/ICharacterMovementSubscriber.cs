using System.Collections;
using Varguiniano.YAPU.Runtime.Characters;

namespace Varguiniano.YAPU.Runtime.Actors
{
    /// <summary>
    /// Interface defining classes that subscribe to the movement of a character.
    /// </summary>
    public interface ICharacterMovementSubscriber
    {
        /// <summary>
        /// Called when the character moves.
        /// </summary>
        /// <param name="characterController">Reference to the character.</param>
        IEnumerator OnCharacterMoved(CharacterController characterController);
    }
}