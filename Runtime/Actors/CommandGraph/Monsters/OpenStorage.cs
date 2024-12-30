using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Monsters
{
    /// <summary>
    /// Actor command to open the storage.
    /// </summary>
    [Serializable]
    public class OpenStorage : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Open the storage.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            bool closed = false;

            DialogManager.ShowPlayerRosterMenu(parameterData.PlayerCharacter,
                                               openStorageDirectly: true,
                                               onBackCallback: (_, _, _) => closed = true);

            yield return new WaitUntil(() => closed);
        }
    }
}