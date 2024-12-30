using System;

namespace Varguiniano.YAPU.Runtime.Quests
{
    /// <summary>
    /// Data representing the status a quest is in.
    /// </summary>
    [Serializable]
    public class QuestStatus
    {
        /// <summary>
        /// Has the quest been completed?
        /// </summary>
        public bool IsCompleted;
        
        /// <summary>
        /// Index of the current objective.
        /// Can be -1 for no current objective.
        /// </summary>
        public int CurrentObjective = -1;
    }
}