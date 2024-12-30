using System;
using System.Collections;
using System.Collections.Generic;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph
{
    /// <summary>
    /// Root node for all command graphs.
    /// </summary>
    [Serializable]
    public class RootNode : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Doesn't run anything.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            yield break;
        }

        /// <summary>
        /// No inputs.
        /// </summary>
        public override List<string> GetInputPorts() => new();

        /// <summary>
        /// One output.
        /// </summary>
        public override List<string> GetOutputPorts() => new() {""};
    }
}