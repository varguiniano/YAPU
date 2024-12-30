using System;
using System.Collections;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Animations
{
    /// <summary>
    /// Command that prepares the player to fall from the sky.
    /// </summary>
    [Serializable]
    public class PreparePlayerToFallFromSky : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Prepare to fall.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            parameterData.PlayerCharacter.CharacterController.PrepareToFallFromSky();
            yield break;
        }
    }
}