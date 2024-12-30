using System;
using System.Collections;
using Varguiniano.YAPU.Runtime.Monster;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Monsters
{
    /// <summary>
    /// Command to remove the nickname of a monster.
    /// </summary>
    [Serializable]
    public class RemoveMonsterNickname : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Remove the nickname.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            MonsterInstance monsterToRemoveNickname = (MonsterInstance) parameterData.ExtraParams[0];

            if (monsterToRemoveNickname == null)
            {
                Logger.Error("Monster wasn't passed as param!");
                yield break;
            }

            monsterToRemoveNickname.Nickname = string.Empty;
            monsterToRemoveNickname.HasNickname = false;
        }
    }
}