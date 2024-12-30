using System;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph.Variables;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Battles
{
    /// <summary>
    /// If command to check if the player can have a double battle.
    /// </summary>
    [Serializable]
    public class IfCanHaveDoubleBattle : IfCondition
    {
        /// <summary>
        /// Check the condition.
        /// TODO: Check companions?
        /// </summary>
        protected override bool CheckCondition(CommandParameterData parameterData) =>
            parameterData.PlayerCharacter.PlayerRoster.NumberThatCanBattle >= 2;
    }
}