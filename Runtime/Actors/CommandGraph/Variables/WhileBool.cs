using System;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Variables
{
    /// <summary>
    /// Command to run a loop while a variable is true.
    /// </summary>
    [Serializable]
    public class WhileBool : WhileVariable<bool>
    {
        /// <summary>
        /// Run while the variable is true.
        /// </summary>
        protected override bool CheckCondition(CommandParameterData parameterData) => VariableValue;
    }
}