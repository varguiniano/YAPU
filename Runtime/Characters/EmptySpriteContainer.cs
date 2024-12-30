using System;
using System.Collections.Generic;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Characters
{
    /// <summary>
    /// Class that contains an empty sprite that can be retrieved as an implementation of IWorldDataContainer.
    /// </summary>
    [Serializable]
    public class EmptySpriteContainer : IWorldDataContainer
    {
        /// <summary>
        /// Reference to the empty sprite.
        /// </summary>
        /// <returns></returns>
        [SerializeField]
        private Sprite EmptySprite;

        /// <summary>
        /// Return the empty sprite.
        /// </summary>
        public Sprite GetLooking(CharacterController.Direction direction,
                                 bool swimming,
                                 bool biking,
                                 bool running,
                                 bool fishing) =>
            EmptySprite;

        /// <summary>
        /// Return a flipbook of the empty sprite.
        /// </summary>
        public List<Sprite> GetWalking(CharacterController.Direction direction,
                                       bool swimming,
                                       bool biking,
                                       bool running) =>
            new()
            {
                EmptySprite,
                EmptySprite
            };
    }
}