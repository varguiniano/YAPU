using System;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph.Variables;
using Varguiniano.YAPU.Runtime.Quests;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Quests
{
    /// <summary>
    /// Conditional command that checks if the player has a quest ongoing or completed.
    /// </summary>
    [Serializable]
    public class IfPlayerHasQuest : IfCondition
    {
        /// <summary>
        /// Quest to check.
        /// </summary>
        [SerializeField]
        private Quest Quest;

        /// <summary>
        /// Checks if the player has the quest ongoing or completed.
        /// </summary>
        protected override bool CheckCondition(CommandParameterData parameterData) =>
            parameterData.QuestManager.HasQuest(Quest);
    }
}