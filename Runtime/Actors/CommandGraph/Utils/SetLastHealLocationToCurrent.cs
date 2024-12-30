using System;
using System.Collections;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Utils
{
    /// <summary>
    /// Actor command that sets the player's last heal location to the current one.
    /// </summary>
    [Serializable]
    public class SetLastHealLocationToCurrent : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Set the location.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData, Action<CommandParameterData> callback)
        {
            parameterData.GlobalGameData.LastHealLocation = parameterData.PlayerCharacter.CurrentLocation;
            yield break;
        }
    }
}