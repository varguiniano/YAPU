using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors;
using Varguiniano.YAPU.Runtime.Characters;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using CharacterController = Varguiniano.YAPU.Runtime.Characters.CharacterController;

namespace Varguiniano.YAPU.Runtime.World.Tiles
{
    /// <summary>
    /// Tile that forces any character that enters it whirlwind in a direction.
    /// </summary>
    public class WhirlwindInDirection : WhateverBehaviour<WhirlwindInDirection>,
                                        IWhirlwindStopper
    {
        /// <summary>
        /// Direction to move to.
        /// </summary>
        [SerializeField]
        private CharacterController.Direction Direction;

        /// <summary>
        /// Does this tile have blocking interaction?
        /// </summary>
        /// <returns></returns>
        public bool HasBlockingInteraction() => true;

        /// <summary>
        /// Force the character to keep moving.
        /// </summary>
        /// <param name="characterController">Character that entered the tile.</param>
        /// <param name="finished">Callback used to determine if wild encounters will still be triggered.</param>
        public IEnumerator CharacterEnterTile(CharacterController characterController, Action<bool> finished)
        {
            finished.Invoke(false);

            bool keepMoving = true;

            PlayerCharacter playerCharacter = null;

            if (characterController.IsPlayer)
                playerCharacter = characterController.GetCachedComponent<PlayerCharacter>();

            while (keepMoving
                && characterController.CanMoveToNextTile(Direction,
                                                         false,
                                                         false,
                                                         out TileType _,
                                                         out TileType _,
                                                         out GameObject targetObject))
            {
                bool dontMove = false;
                List<TriggerActor> interactables = null;

                if (playerCharacter != null)
                {
                    playerCharacter.LockInput();

                    Vector3Int targetPosition =
                        CharacterController.MoveOneInDirection(characterController.Transform.position.ToInts(),
                                                               Direction);

                    if (!characterController.FindTargetGrid(targetPosition, out GridController targetGrid)) yield break;

                    yield return playerCharacter.TriggerPlayerAboutToEnterTile(targetGrid,
                                                                               targetPosition,
                                                                               (shouldNotMove, foundInteractables) =>
                                                                               {
                                                                                   dontMove = shouldNotMove;
                                                                                   interactables = foundInteractables;
                                                                               });
                }

                if (dontMove) yield break;

                yield return characterController.Move(Direction,
                                                      forceKeepMoving: true,
                                                      animateWhirlwind: true,
                                                      speedMultiplier: .33f,
                                                      runningAndOthersAffectSpeed: false);

                if (playerCharacter != null)
                {
                    yield return playerCharacter.TriggerPlayerEnteredTile(interactables);

                    playerCharacter.LockInput(false);
                }

                keepMoving = targetObject == null || targetObject.GetComponent<IWhirlwindStopper>() == null;
            }
        }

        #region Not Used

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

        #endregion
    }
}