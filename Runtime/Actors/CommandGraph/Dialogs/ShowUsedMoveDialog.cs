using System;
using System.Collections;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Dialogs
{
    /// <summary>
    /// Show a dialog telling a monster used a move.
    /// </summary>
    [Serializable]
    public class ShowUsedMoveDialog : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Run the command.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            if (!parameterData.UsingMove || parameterData.MoveUser == null || parameterData.Move == null)
            {
                Logger.Warn("This command will only run when using a move!");
                yield break;
            }

            yield return DialogManager.ShowDialogAndWait("Battle/Move/Used",
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        parameterData.MoveUser
                                                                           .GetNameOrNickName(parameterData.Localizer),
                                                                        parameterData.Localizer[parameterData.Move
                                                                           .LocalizableName]
                                                                    });
        }
    }
}