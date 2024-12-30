using System;
using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using WhateverDevs.Core.Runtime.Common;
using CharacterController = Varguiniano.YAPU.Runtime.Characters.CharacterController;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Animations
{
    /// <summary>
    /// Move the character a number of tiles in a direction.
    /// </summary>
    [Serializable]
    public class Move : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Move this actor?
        /// </summary>
        [SerializeField]
        private bool OwnActor = true;

        /// <summary>
        /// Target to move if not own actor.
        /// </summary>
        [SerializeField]
        [HideIf(nameof(OwnActor))]
        private ActorCharacter Target;

        /// <summary>
        /// Direction to move towards.
        /// </summary>
        [SerializeField]
        [HideIf("@" + nameof(RandomDirection) + " || " + nameof(OppositeToPlayer))]
        [InfoBox("Direction shouldn't be None.",
                 InfoMessageType.Error,
                 VisibleIf = "@ Direction == Varguiniano.YAPU.Runtime.Characters.CharacterController.Direction.None")]
        private CharacterController.Direction Direction;

        /// <summary>
        /// Use a random direction to move into?
        /// </summary>
        [SerializeField]
        [HideIf(nameof(OppositeToPlayer))]
        private bool RandomDirection;

        /// <summary>
        /// Move opposite to player?
        /// </summary>
        [SerializeField]
        [HideIf(nameof(RandomDirection))]
        private bool OppositeToPlayer;

        /// <summary>
        /// Number of tiles to move in that direction.
        /// </summary>
        [SerializeField]
        private uint NumberOfTiles = 1;

        /// <summary>
        /// Do the move while running?
        /// </summary>
        [SerializeField]
        private bool Running;

        /// <summary>
        /// Wait for the next tile to be free?
        /// </summary>
        [SerializeField]
        private bool WaitForNextTileToBeFree;

        /// <summary>
        /// Wait and only move when the actor is looping?
        /// </summary>
        [SerializeField]
        private bool WaitIfLoopingStops;

        /// <summary>
        /// Perform the move.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            ActorCharacter actorCharacter = OwnActor ? parameterData.Actor as ActorCharacter : Target;

            if (actorCharacter == null) yield break;

            CharacterController.Direction direction = Direction;

            if (RandomDirection)
                do
                    direction = WhateverDevs.Core.Runtime.Common.Utils.GetAllItems<CharacterController.Direction>()
                                            .ToList()
                                            .Random();
                while (direction == CharacterController.Direction.None);

            if (OppositeToPlayer) direction = parameterData.PlayerDirection.Invert();

            actorCharacter.CharacterController.IsRunning = Running;

            for (int i = 0; i < NumberOfTiles; ++i)
            {
                if (WaitIfLoopingStops && !parameterData.Actor.IsLooping)
                {
                    parameterData.Actor.LoopRunning = false;

                    yield return new WaitUntil(() => parameterData.Actor.IsLooping);

                    parameterData.Actor.LoopRunning = true;
                }

                yield return actorCharacter.CharacterController.Move(direction,
                                                                     waitForNextTileToBeFree: WaitForNextTileToBeFree);
            }

            actorCharacter.CharacterController.IsRunning = false;
        }
    }
}