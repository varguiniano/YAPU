using System;
using System.Collections.Generic;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Variables
{
    /// <summary>
    /// Command that checks if the actor pushed inside the trigger is one of the given actors.
    /// </summary>
    [Serializable]
    public class IfPushedActorEquals : IfCondition
    {
        /// <summary>
        /// Actors to match.
        /// </summary>
        [SerializeField]
        private List<PushableActor> PossibleActors = new();

        /// <summary>
        /// Check if the actor matches.
        /// </summary>
        protected override bool CheckCondition(CommandParameterData parameterData) =>
            PossibleActors.Contains(parameterData.ExtraParams[0] as PushableActor);
    }
}