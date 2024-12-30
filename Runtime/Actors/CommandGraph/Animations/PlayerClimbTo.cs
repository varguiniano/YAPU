using System;
using System.Collections;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Animations
{
    /// <summary>
    /// Command to have the player climb to a position.
    /// </summary>
    [Serializable]
    public class PlayerClimbTo : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Target position.
        /// </summary>
        [SerializeField]
        private Vector3Int Target;

        /// <summary>
        /// Climb to that position.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            yield return parameterData.PlayerCharacter.ClimbTo(Target);
        }
    }
}