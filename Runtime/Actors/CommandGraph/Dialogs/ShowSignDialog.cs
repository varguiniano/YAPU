using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Dialogs
{
    /// <summary>
    /// Show a sign dialog when the command is run.
    /// </summary>
    [Serializable]
    public class ShowSignDialog : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Localization key for the dialog to show.
        /// </summary>
        [SerializeField]
        protected string DialogLocalizationKey;

        /// <summary>
        /// Run the command.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            yield return DialogManager.ShowDialogAndWait(DialogLocalizationKey,
                                                         background: DialogManager.BasicDialogBackground.Sign,
                                                         horizontalAlignment: HorizontalAlignmentOptions.Center);
        }
    }
}