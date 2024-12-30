using System;
using System.Collections.Generic;
using UnityEngine;
using CharacterController = Varguiniano.YAPU.Runtime.Characters.CharacterController;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Variables
{
    /// <summary>
    /// If command that checks the player direction.
    /// </summary>
    [Serializable]
    public class IfPlayerDirectionIs : IfCondition
    {
        /// <summary>
        /// Directions to check.
        /// </summary>
        [SerializeField]
        private List<CharacterController.Direction> Directions;

        /// <summary>
        /// Check if the player is in that direction.
        /// </summary>
        protected override bool CheckCondition(CommandParameterData parameterData) =>
            Directions.Contains(parameterData.PlayerDirection);
    }
}