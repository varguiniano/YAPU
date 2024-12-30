using System;
using System.Collections;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Quests
{
    /// <summary>
    /// Command that unlocks the game completion percentage.
    /// </summary>
    [Serializable]
    public class UnlockGameCompletionPercentage : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Unlock the game completion percentage.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            parameterData.QuestManager.VisibleGamePercentage = true;
            yield break;
        }
    }
}