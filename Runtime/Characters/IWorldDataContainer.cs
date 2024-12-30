using System.Collections.Generic;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Characters
{
    /// <summary>
    /// Interface that defines a class that has world sprites and data.
    /// </summary>
    public interface IWorldDataContainer
    {
        /// <summary>
        /// Get the sprite to look in a direction.
        /// </summary>
        /// <param name="direction">Direction to look into.</param>
        /// <param name="swimming">Is the character swimming?</param>
        /// <param name="biking">Is the character biking?</param>
        /// <param name="running">Is the character running?</param>
        /// <param name="fishing">Is the character fishing?</param>
        /// <returns>A sprite.</returns>
        Sprite GetLooking(CharacterController.Direction direction, bool swimming, bool biking, bool running, bool fishing);
        
        /// <summary>
        /// Get the sprites to walk in a direction.
        /// </summary>
        /// <param name="direction">Direction to look into.</param>
        /// <param name="swimming">Is the character swimming?</param>
        /// <param name="biking">Is the character biking?</param>
        /// <param name="running">Is the character running.</param>
        /// <returns>A list of sprites.</returns>
        List<Sprite> GetWalking(CharacterController.Direction direction, bool swimming, bool biking, bool running);
    }
}