using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Quests;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Quests
{
    /// <summary>
    /// Command to update the objective of a quest.
    /// </summary>
    [Serializable]
    public class UpdateQuestObjective : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Quest to update.
        /// </summary>
        [SerializeField]
        private Quest Quest;

        /// <summary>
        /// Index of the new objective.
        /// </summary>
        [Tooltip("Can be -1 for no objective.")]
        [SerializeField]
        [PropertyRange(-1, "@Quest == null ? -1 : Quest.Objectives.Count - 1")]
        private int NewObjective = -1;

        /// <summary>
        /// Update the objective
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            parameterData.QuestManager.SetQuestObjective(Quest, NewObjective);

            yield break;
        }
    }
}