using System;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph.Variables;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Dex
{
    /// <summary>
    /// If command that checks if the player has the dex.
    /// </summary>
    [Serializable]
    public class IfPlayerHasDex : IfCondition
    {
        /// <summary>
        /// Check if they have the dex.
        /// </summary>
        protected override bool CheckCondition(CommandParameterData parameterData) =>
            parameterData.GlobalGameData.HasDex;
    }
}