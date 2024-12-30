using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Badges;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Badges
{
    /// <summary>
    /// Command to give a badge to the player.
    /// </summary>
    [Serializable]
    public class GiveBadge : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Badge to give.
        /// </summary>
        [SerializeField]
        private Badge Badge;

        /// <summary>
        /// Give the badge to the player.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            parameterData.PlayerCharacter.GlobalGameData.AddBadge(Badge, parameterData.PlayerCharacter.Region);

            yield return DialogManager.ShowDialogAndWait("Dialogs/ReceivedBadge", modifiers: Badge.LocalizableName);
        }
    }
}