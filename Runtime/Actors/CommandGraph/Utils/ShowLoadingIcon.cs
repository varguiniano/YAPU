using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Utils
{
    /// <summary>
    /// Command to show the loading icon.
    /// </summary>
    [Serializable]
    public class ShowLoadingIcon : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Show or hide?
        /// </summary>
        [SerializeField]
        private bool Show;

        /// <summary>
        /// Show or hide the icon.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData, Action<CommandParameterData> callback)
        {
            DialogManager.ShowLoadingIcon(Show);
            yield break;
        }
    }
}