using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using CharacterController = Varguiniano.YAPU.Runtime.Characters.CharacterController;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Animations
{
    /// <summary>
    /// Force the player to look at this actor.
    /// </summary>
    [Serializable]
    public class ForcePlayerToLook : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Mode for the command to work on.
        /// </summary>
        [SerializeField]
        private Mode LookMode;

        /// <summary>
        /// Direction to look at in the fixed direction mode.
        /// </summary>
        [ShowIf("@" + nameof(LookMode) + " == Mode.FixedDirection")]
        [SerializeField]
        private CharacterController.Direction Direction;

        /// <summary>
        /// Force the player to look at this actor.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            if (parameterData.PlayerCharacter == null) yield break;

            switch (LookMode)
            {
                case Mode.AtThisActor:
                    Vector3Int position = parameterData.Actor.Position;
                    Vector3Int playerPosition = parameterData.PlayerCharacter.Position;

                    int xDistanceToPlayer = position.x - playerPosition.x;
                    int yDistanceToPlayer = position.y - playerPosition.y;

                    CharacterController.Direction xDirection =
                        xDistanceToPlayer > 0
                            ? CharacterController.Direction.Right
                            : CharacterController.Direction.Left;

                    CharacterController.Direction yDirection =
                        yDistanceToPlayer > 0 ? CharacterController.Direction.Up : CharacterController.Direction.Down;

                    if (xDistanceToPlayer != 0)
                        parameterData.PlayerCharacter.CharacterController.LookAt(xDirection,
                            useBikingSprite: parameterData.PlayerCharacter.CharacterController
                                                          .IsBiking);

                    if (yDistanceToPlayer != 0)
                        parameterData.PlayerCharacter.CharacterController.LookAt(yDirection,
                            useBikingSprite: parameterData.PlayerCharacter.CharacterController
                                                          .IsBiking);

                    break;
                case Mode.FixedDirection:
                    parameterData.PlayerCharacter.CharacterController.LookAt(Direction,
                                                                             useBikingSprite: parameterData
                                                                                .PlayerCharacter.CharacterController
                                                                                .IsBiking);

                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Modes this command can work on.
        /// </summary>
        private enum Mode
        {
            AtThisActor,
            FixedDirection
        }
    }
}