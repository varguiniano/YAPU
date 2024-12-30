using System;
using System.Collections;
using UnityEngine;
using WhateverDevs.Core.Behaviours;
using CharacterController = Varguiniano.YAPU.Runtime.Characters.CharacterController;

namespace Varguiniano.YAPU.Runtime.World.Tiles
{
    /// <summary>
    /// Behaviour for a ladder tile that goes up.
    /// </summary>
    public class LadderUp : WhateverBehaviour<LadderUp>, ICharacterInteractingTile
    {
        /// <summary>
        /// Sort order when the character is not on the tile.
        /// </summary>
        [SerializeField]
        private int CharacterOffTileSortOrder = 46;

        /// <summary>
        /// Sort order when the character is on the tile.
        /// </summary>
        [SerializeField]
        private int CharacterOnTileSortOrder = 16;

        /// <summary>
        /// Reference to the upper part sprite renderer.
        /// </summary>
        [SerializeField]
        private SpriteRenderer SpriteRenderer;

        /// <summary>
        /// Set the normal sort order on enable.
        /// </summary>
        private void OnEnable() => SpriteRenderer.sortingOrder = CharacterOffTileSortOrder;

        /// <summary>
        /// Doesn't have a blocking interaction.
        /// </summary>
        public bool HasBlockingInteraction() => false;

        /// <summary>
        /// Set the on sort order when character enters.
        /// </summary>
        public IEnumerator CharacterAboutToEnterTileAsync(CharacterController characterController)
        {
            SpriteRenderer.sortingOrder = CharacterOnTileSortOrder;
            yield break;
        }

        /// <summary>
        /// Set the normal sort order when character leaves.
        /// </summary>
        public IEnumerator CharacterLeftTileAsync(CharacterController characterController)
        {
            SpriteRenderer.sortingOrder = CharacterOffTileSortOrder;
            yield break;
        }

        #region Unused

        public IEnumerator CharacterEnterTileAsync(CharacterController characterController)
        {
            yield break;
        }

        public IEnumerator CharacterAboutToLeaveTileAsync(CharacterController characterController)
        {
            yield break;
        }

        public IEnumerator CharacterAboutToEnterTile(CharacterController characterController,
                                                     Action<bool, bool> finished)
        {
            yield break;
        }

        public IEnumerator CharacterEnterTile(CharacterController characterController, Action<bool> finished)
        {
            yield break;
        }

        public IEnumerator CharacterAboutToLeaveTile(CharacterController characterController, Action<bool> finished)
        {
            yield break;
        }

        public IEnumerator CharacterLeftTile(CharacterController characterController, Action<bool> finished)
        {
            yield break;
        }

        #endregion
    }
}