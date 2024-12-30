using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Quests;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Quests
{
    /// <summary>
    /// Command to complete a quest.
    /// </summary>
    [Serializable]
    public class CompleteQuest : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Quest to complete.
        /// </summary>
        [SerializeField]
        private Quest Quest;

        /// <summary>
        /// Complete the quest.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            parameterData.QuestManager.CompleteQuest(Quest);

            yield break;
        }
    }
}