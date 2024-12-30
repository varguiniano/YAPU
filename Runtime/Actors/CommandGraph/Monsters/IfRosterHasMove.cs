using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph.Variables;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Monsters
{
    /// <summary>
    /// If command that checks is the player roster has a move.
    /// </summary>
    [Serializable]
    public class IfRosterHasMove : IfCondition
    {
        /// <summary>
        /// Move to check.
        /// </summary>
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMoves))]
        #endif
        private Move Move;

        /// <summary>
        /// Check if the roster has that move.
        /// </summary>
        protected override bool CheckCondition(CommandParameterData parameterData) =>
            parameterData.PlayerCharacter.PlayerRoster.AnyHasMove(Move);
    }
}