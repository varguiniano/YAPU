using System;
using System.Collections;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Animations
{
    /// <summary>
    /// Look at the player.
    /// </summary>
    [Serializable]
    public class LookAtPlayer : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Look at the player.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            ActorCharacter actorCharacter = parameterData.Actor as ActorCharacter;

            if (actorCharacter != null && parameterData.PlayerCharacter != null)
                actorCharacter.CharacterController.LookAt(parameterData.PlayerDirection,
                                                          useBikingSprite: actorCharacter.CharacterController.IsBiking);

            yield break;
        }
    }
}