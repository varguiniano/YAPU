using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using WhateverDevs.Core.Behaviours;
using CharacterController = Varguiniano.YAPU.Runtime.Characters.CharacterController;

namespace Varguiniano.YAPU.Runtime.World.Tiles
{
    /// <summary>
    /// Tile FX for tiles that have a circle wave when the player steps or leaves them.
    /// </summary>
    public class CircleWaveFX : WhateverBehaviour<CircleWaveFX>, ICharacterInteractingTile
    {
        /// <summary>
        /// Duration of the animation.
        /// </summary>
        [SerializeField]
        private float AnimationDuration;

        /// <summary>
        /// Reference to the wave transform.
        /// </summary>
        [SerializeField]
        private Transform WaveTransform;

        /// <summary>
        /// Reference to the wave sprite.
        /// </summary>
        [SerializeField]
        private SpriteRenderer WaveSprite;

        /// <summary>
        /// flag to know when we are animating.
        /// </summary>
        private bool animating;

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
            yield break;
        }

        /// <summary>
        /// Called by a character controller when it enters a tile.
        /// </summary>
        /// <param name="characterController">Character that entered the tile.</param>
        public IEnumerator CharacterEnterTileAsync(CharacterController characterController)
        {
            yield return PlayAnimation();
        }

        /// <summary>
        /// Called by a character controller when it is about to leave a tile.
        /// </summary>
        /// <param name="characterController">Character that left the tile.</param>
        public IEnumerator CharacterAboutToLeaveTileAsync(CharacterController characterController)
        {
            yield return PlayAnimation();
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
        /// Play the wave animation.
        /// </summary>
        private IEnumerator PlayAnimation()
        {
            if (animating) yield break;

            animating = true;

            WaveTransform.localScale = new Vector3(0, 0, 1);

            Color color = WaveSprite.color;
            color.a = 255;
            WaveSprite.color = color;

            bool finished = false;

            WaveTransform.DOScale(Vector3.one, AnimationDuration).OnComplete(() => finished = true);

            WaveSprite.DOFade(0, AnimationDuration).SetEase(Ease.OutExpo);

            yield return new WaitUntil(() => finished);

            animating = false;
        }
    }
}