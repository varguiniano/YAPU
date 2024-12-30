using System;
using System.Collections;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Utils
{
    /// <summary>
    /// Command to the destroy the actor that was pushed inside the trigger.
    /// </summary>
    [Serializable]
    public class DestroyPushedActor : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Destroy the pushed actor.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData, Action<CommandParameterData> callback)
        {
            parameterData.Callback.Invoke(new CommandCallbackParams {DestroyPushedActor = true});
            yield break;
        }
    }
}