using System;
using System.Collections;
using Varguiniano.YAPU.Runtime.Characters;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Dex
{
    /// <summary>
    /// Register the monster this actor represents as seen in the dex.
    /// </summary>
    [Serializable]
    public class RegisterMonsterActorAsSeenInDex : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Register in the dex.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            ActorCharacter actorCharacter = parameterData.Actor as ActorCharacter;

            if (actorCharacter == null
             || actorCharacter.CharacterController.Mode != CharacterController.CharacterMode.Monster)
                yield break;

            parameterData.PlayerCharacter.PlayerDex.RegisterAsSeen(actorCharacter.CharacterController.GetMonsterData(),
                                                                   true,
                                                                   false);
        }
    }
}