using System;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph.Variables;
using Varguiniano.YAPU.Runtime.Quests;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Quests
{
    /// <summary>
    /// Conditional command that checks if the player has a quest ongoing or completed
    /// and if the current objective is the one specified.
    /// </summary>
    [Serializable]
    public class IfCurrentQuestObjective : IfCondition
    {
        /// <summary>
        /// Quest to check.
        /// </summary>
        [SerializeField]
        private Quest Quest;

        /// <summary>
        /// Objective to check.
        /// </summary>
        [SerializeField]
        private int Objective;

        /// <summary>
        /// Checks if the player has the quest ongoing or completed.
        /// </summary>
        protected override bool CheckCondition(CommandParameterData parameterData) =>
            parameterData.QuestManager.HasQuest(Quest)
         && !parameterData.QuestManager.GetStatus(Quest).IsCompleted
         && parameterData.QuestManager.GetStatus(Quest).CurrentObjective == Objective;
    }
}