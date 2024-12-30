using System;
using System.Collections;
using UnityEngine;
using WhateverDevs.Core.Behaviours;
using CharacterController = Varguiniano.YAPU.Runtime.Characters.CharacterController;

namespace Varguiniano.YAPU.Runtime.World.Tiles
{
    /// <summary>
    /// Controller for a tile that displays footprints when the character goes over it.
    /// </summary>
    public class CharacterFootprintsTile : WhateverBehaviour<CharacterFootprintsTile>, ICharacterInteractingTile
    {
        /// <summary>
        /// Prefab to use when walking.
        /// </summary>
        [SerializeField]
        private Footprint WalkingPrefab;

        /// <summary>
        /// Prefab to use when biking.
        /// </summary>
        [SerializeField]
        private Footprint BikingPrefab;

        /// <summary>
        /// Doesn't block walking.
        /// </summary>
        public bool HasBlockingInteraction() => false;

        /// <summary>
        /// Spawn a footprint.
        /// </summary>
        public IEnumerator CharacterAboutToLeaveTileAsync(CharacterController characterController)
        {
            Footprint footprint = Instantiate(characterController.IsBiking ? BikingPrefab : WalkingPrefab, transform);

            yield return WaitAFrame;

            footprint.ShowFootprint(characterController.CurrentDirection);
        }

        #region Unused

        public IEnumerator CharacterAboutToEnterTileAsync(CharacterController characterController)
        {
            yield break;
        }

        public IEnumerator CharacterEnterTileAsync(CharacterController characterController)
        {
            yield break;
        }

        public IEnumerator CharacterAboutToLeaveTile(CharacterController characterController, Action<bool> finished)
        {
            yield break;
        }

        public IEnumerator CharacterLeftTileAsync(CharacterController characterController)
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

        public IEnumerator CharacterLeftTile(CharacterController characterController, Action<bool> finished)
        {
            yield break;
        }

        #endregion
    }
}