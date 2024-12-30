using System;
using System.Collections;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Dialogs
{
    /// <summary>
    /// Command to skip to the next dialog.
    /// </summary>
    [Serializable]
    public class NextDialog : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Skip to next.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            yield return DialogManager.WaitForTypewriter;
            DialogManager.NextDialog();
        }
    }
}