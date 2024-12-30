using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Quests;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Quests
{
    /// <summary>
    /// Command to start a quest for the player.
    /// </summary>
    [Serializable]
    public class StartQuest : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Quest to start.
        /// </summary>
        [SerializeField]
        private Quest Quest;

        /// <summary>
        /// Index of the initial objective.
        /// </summary>
        [Tooltip("Can be -1 for no objective.")]
        [SerializeField]
        [PropertyRange(-1, "@Quest == null ? -1 : Quest.Objectives.Count - 1")]
        private int InitialObjective = -1;

        /// <summary>
        /// Start the quest.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            parameterData.QuestManager.StartQuest(Quest, InitialObjective);

            yield break;
        }
    }
}