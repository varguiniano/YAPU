using System.Collections;
using Varguiniano.YAPU.Runtime.Characters;

namespace Varguiniano.YAPU.Runtime.Actors
{
    /// <summary>
    /// Interface that defines scene elements the player can interact with with the interaction button.
    /// </summary>
    public interface IPlayerInteractable
    {
        /// <summary>
        /// Flag to mark if the player interacts with this interactable when they are on top of it.
        /// </summary>
        /// <returns></returns>
        bool InteractsWhenOnTop();
        
        /// <summary>
        /// Called when the player interacts with this element.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="playerDirection">Direction the player is at.</param>
        IEnumerator Interact(PlayerCharacter playerCharacter, CharacterController.Direction playerDirection);
    }
}