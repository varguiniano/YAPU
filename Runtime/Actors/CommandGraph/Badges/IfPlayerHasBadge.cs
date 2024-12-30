using System;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph.Variables;
using Varguiniano.YAPU.Runtime.Badges;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Badges
{
    /// <summary>
    /// Command to check if a player has a badge.
    /// </summary>
    [Serializable]
    public class IfPlayerHasBadge : IfCondition
    {
        /// <summary>
        /// Badge to check.
        /// </summary>
        [SerializeField]
        private Badge Badge;

        /// <summary>
        /// Check if the player has the badge.
        /// </summary>
        protected override bool CheckCondition(CommandParameterData parameterData) =>
            parameterData.PlayerCharacter.GlobalGameData.HasBadge(Badge, parameterData.PlayerCharacter.Region);
    }
}