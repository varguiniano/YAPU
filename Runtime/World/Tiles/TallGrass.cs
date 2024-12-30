using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.DataStructures;
using CharacterController = Varguiniano.YAPU.Runtime.Characters.CharacterController;

namespace Varguiniano.YAPU.Runtime.World.Tiles
{
    /// <summary>
    /// Tall grass controller.
    /// </summary>
    public class TallGrass : WhateverBehaviour<TallGrass>, ICharacterInteractingTile
    {
        /// <summary>
        /// Sprites for this grass and order they should have.
        /// </summary>
        [SerializeField]
        private SerializableDictionary<SpriteRenderer, Vector2Int> Sprites;

        /// <summary>
        /// Disable the character shadow when in this tile?
        /// </summary>
        [SerializeField]
        private bool DisableCharacterShadow;

        /// <summary>
        /// Play a hit animation when a character passes?
        /// </summary>
        [SerializeField]
        private bool PlayHitAnimation;

        /// <summary>
        /// Grass FX when the grass is hit.
        /// </summary>
        [SerializeField]
        [ShowIf(nameof(PlayHitAnimation))]
        private VisualEffect GrassHit;

        /// <summary>
        /// Set the normal order.
        /// </summary>
        private void OnEnable() => SetOrder(true);

        /// <summary>
        /// Does this tile have blocking interaction?
        /// </summary>
        /// <returns></returns>
        public bool HasBlockingInteraction() => false;

        /// <summary>
        /// Called by a character controller when it is about to enter a tile.
        /// </summary>
        /// <param name="characterController">Character that entered the tile.</param>
        public IEnumerator CharacterAboutToEnterTileAsync(CharacterController characterController)
        {
            SetOrder(false);

            if (DisableCharacterShadow) characterController.EnableShadow(false);

            if (!PlayHitAnimation) yield break;

            GrassHit.enabled = true;
            GrassHit.Play();
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
            SetOrder(true);
            if (DisableCharacterShadow) characterController.EnableShadow(true);

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

        /// <summary>
        /// Called by a character controller when it enters a tile.
        /// </summary>
        /// <param name="characterController">Character that entered the tile.</param>
        /// <param name="finished">Callback used to determine if wild encounters will still be triggered.</param>
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

        /// <summary>
        /// Set the sorting order of the sprites.
        /// </summary>
        /// <param name="normal">Normal or character over.</param>
        private void SetOrder(bool normal)
        {
            foreach (KeyValuePair<SpriteRenderer, Vector2Int> sprite in Sprites)
                sprite.Key.sortingOrder = normal ? sprite.Value.x : sprite.Value.y;
        }
    }
}