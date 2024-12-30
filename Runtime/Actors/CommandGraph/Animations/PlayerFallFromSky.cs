using System;
using System.Collections;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Animations
{
    /// <summary>
    /// Make the player fall from the sky.
    /// </summary>
    [Serializable]
    public class PlayerFallFromSky : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Make the player fall from the sky.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            yield return parameterData.PlayerCharacter.CharacterController.FallFromSky();
        }
    }
}