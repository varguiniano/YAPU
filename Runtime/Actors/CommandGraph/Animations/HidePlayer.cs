using System;
using System.Collections;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Animations
{
    /// <summary>
    /// Command to hide the player.
    /// </summary>
    [Serializable]
    public class HidePlayer : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Hide the player.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            parameterData.PlayerCharacter.CharacterController.HideGraphics();
            yield break;
        }
    }
}