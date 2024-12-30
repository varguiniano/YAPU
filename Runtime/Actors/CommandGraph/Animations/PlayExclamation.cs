using System;
using System.Collections;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Animations
{
    /// <summary>
    /// Command to play an exclamation popup on top of the player.
    /// </summary>
    [Serializable]
    public class PlayExclamation : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Play the popup.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            ActorCharacter actorCharacter = parameterData.Actor as ActorCharacter;

            if (actorCharacter != null) yield return actorCharacter.Popup.PlayExclamation();
        }
    }
}