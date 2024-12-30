using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Animations
{
    /// <summary>
    /// Command to enable or disable an actor item's sprite.
    /// </summary>
    [Serializable]
    public class DisableActorItemSprite : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Disable the sprite of the own actor?
        /// </summary>
        [SerializeField]
        private bool OwnActor = true;

        /// <summary>
        /// Target actor otherwise.
        /// </summary>
        [SerializeField]
        [HideIf(nameof(OwnActor))]
        private ActorItem Target;

        /// <summary>
        /// Disable the sprite.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            ActorItem target = OwnActor ? parameterData.Actor as ActorItem : Target;

            if (target == null) yield break;

            target.EnableSprite(false);
        }
    }
}