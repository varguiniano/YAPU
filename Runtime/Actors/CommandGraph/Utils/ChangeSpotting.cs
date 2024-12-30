using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Utils
{
    /// <summary>
    /// Command to change the player spotting.
    /// </summary>
    [Serializable]
    public class ChangeSpotting : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Can it spot the player?
        /// </summary>
        [SerializeField]
        private bool Spot;

        /// <summary>
        /// Should this change the own actor's looping?
        /// </summary>
        [SerializeField]
        private bool TargetItself = true;

        /// <summary>
        /// Target actor.
        /// </summary>
        [HideIf(nameof(TargetItself))]
        [SerializeField]
        private ActorCharacter Target;

        /// <summary>
        /// Change spotting the player.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData, Action<CommandParameterData> callback)
        {
            ActorCharacter actorCharacter = TargetItself ? parameterData.Actor as ActorCharacter : Target;

            if (actorCharacter == null) yield break;

            actorCharacter.CanSpotPlayer = Spot;
        }
    }
}