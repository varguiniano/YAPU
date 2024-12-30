using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Monsters
{
    /// <summary>
    /// Make the roster use a move.
    /// </summary>
    [Serializable]
    public class MakeRosterUseMove : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Move to use.
        /// </summary>
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMoves))]
        #endif
        private MonsterDatabase.Moves.Move Move;

        /// <summary>
        /// Run the command.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            ActorMoveTarget actorMoveTarget = parameterData.Actor as ActorMoveTarget;

            if (actorMoveTarget == null)
            {
                Logger.Warn("This command only works on ActorMoveTargets!");
                yield break;
            }

            if (!parameterData.PlayerCharacter.PlayerRoster.AnyHasMove(Move))
            {
                Logger.Warn("No monster in the roster has that move!");
                yield break;
            }

            foreach (MonsterInstance candidate in parameterData.PlayerCharacter.PlayerRoster)
                if (candidate.KnowsMove(Move))
                {
                    yield return Move.UseOutOfBattle(parameterData.PlayerCharacter,
                                                     candidate,
                                                     parameterData.Localizer,
                                                     parameterData.MapSceneLauncher);

                    break; // We only need to use it once.
                }
        }
    }
}