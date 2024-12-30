using System;
using System.Collections;
using System.Collections.Generic;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Utils
{
    /// <summary>
    /// Command that runs the next commands with the character type of the current actor as a parameter.
    /// </summary>
    [Serializable]
    public class WithCharacterTypeAsParam : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Add the param and run the commands.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData, Action<CommandParameterData> callback)
        {
            ActorCharacter actorCharacter = parameterData.Actor as ActorCharacter;

            if (actorCharacter == null)
            {
                Logger.Error("Can't add character type as param since this is a non-character actor.");
                yield break;
            }

            parameterData.ExtraParams =
                new List<ICommandParameter>(parameterData.ExtraParams)
                    {
                        actorCharacter.CharacterController.GetCharacterData()
                    }
                   .ToArray();

            callback.Invoke(parameterData);
        }
    }
}