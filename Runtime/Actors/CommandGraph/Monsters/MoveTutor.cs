using System;
using System.Collections;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Monsters
{
    /// <summary>
    /// Command that teaches a move to a monster.
    /// The monster must be the first param and the move must be the third.
    /// </summary>
    [Serializable]
    public class MoveTutor : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Teach the move.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            if (parameterData.ExtraParams[0] is not MonsterInstance monster || monster.IsNullEntry)
            {
                Logger.Info("No monster selected.");
                yield break;
            }

            MonsterDatabase.Moves.Move moveToLearn = parameterData.ExtraParams[2] as MonsterDatabase.Moves.Move;

            if (moveToLearn == null)
            {
                Logger.Info("No move selected.");
                yield break;
            }

            yield return DialogManager.ShowMoveLearnPanel(monster,
                                                          moveToLearn,
                                                          parameterData.Localizer,
                                                          _ =>
                                                          {
                                                          });
        }
    }
}