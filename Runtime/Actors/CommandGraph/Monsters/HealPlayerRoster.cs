using System;
using System.Collections;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Monsters
{
    /// <summary>
    /// Command to heal the entire player roster.
    /// </summary>
    [Serializable]
    public class HealPlayerRoster : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Heal the entire player roster.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            parameterData.PlayerCharacter.PlayerRoster.CompletelyHeal();
            yield break;
        }
    }
}