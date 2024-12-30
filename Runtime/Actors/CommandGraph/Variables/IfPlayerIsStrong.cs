using System;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Variables
{
    /// <summary>
    /// If command that checks if the player is strong.
    /// </summary>
    [Serializable]
    public class IfPlayerIsStrong : IfCondition
    {
        /// <summary>
        /// Check if the player is strong.
        /// </summary>
        protected override bool CheckCondition(CommandParameterData parameterData) =>
            parameterData.PlayerCharacter.Strong;
    }
}