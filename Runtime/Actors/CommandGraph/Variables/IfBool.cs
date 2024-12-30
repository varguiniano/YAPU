using System;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Variables
{
    /// <summary>
    /// Command that executes two paths depending on a bool variable.
    /// </summary>
    [Serializable]
    public class IfBool : IfSingleVariable<bool>
    {
        /// <summary>
        /// Check if the bool is true.
        /// </summary>
        /// <param name="parameterData">Command parameters.</param>
        protected override bool CheckCondition(CommandParameterData parameterData) => VariableValue;
    }
}