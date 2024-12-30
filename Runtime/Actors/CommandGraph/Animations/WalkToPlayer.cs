using System;
using System.Collections;
using UnityEngine;
using CharacterController = Varguiniano.YAPU.Runtime.Characters.CharacterController;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Animations
{
    /// <summary>
    /// Actor command to walk to the player.
    /// </summary>
    [Serializable]
    public class WalkToPlayer : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Walk to the player.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            ActorCharacter actorCharacter = parameterData.Actor as ActorCharacter;

            if (actorCharacter == null) yield break;

            Vector3Int position = actorCharacter.Position;
            Vector3Int playerPosition = parameterData.PlayerCharacter.Position;

            int xDistanceToPlayer = position.x - playerPosition.x;
            int yDistanceToPlayer = position.y - playerPosition.y;

            CharacterController.Direction xDirection =
                xDistanceToPlayer > 0 ? CharacterController.Direction.Left : CharacterController.Direction.Right;

            CharacterController.Direction yDirection =
                yDistanceToPlayer > 0 ? CharacterController.Direction.Down : CharacterController.Direction.Up;

            if (xDistanceToPlayer != 0)
                yield return actorCharacter.CharacterController.Move(xDirection, Mathf.Abs(xDistanceToPlayer));

            if (yDistanceToPlayer != 0)
                yield return actorCharacter.CharacterController.Move(yDirection, Mathf.Abs(yDistanceToPlayer));
        }
    }
}