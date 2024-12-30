using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Utils
{
    /// <summary>
    /// Actor command to autodestroy the actor.
    /// </summary>
    [Serializable]
    public class AutoDestroy : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Unlock the input in case the input was lock by an interaction.
        /// </summary>
        [SerializeField]
        private bool UnlockInputBeforeDestroying;

        /// <summary>
        /// Unlock the rest of the actors before destroying.
        /// </summary>
        [SerializeField]
        private bool UnlockOtherActorsBeforeDestroying;

        /// <summary>
        /// Auto destroy.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            if (UnlockInputBeforeDestroying && parameterData.PlayerCharacter != null)
                parameterData.PlayerCharacter.LockInput(false);

            if (UnlockOtherActorsBeforeDestroying) parameterData.Actor.GlobalGridManager.StopAllActors = false;

            parameterData.Actor.RunningCommands = false;
            parameterData.Actor.ShouldLoop = false;

            Object.Destroy(parameterData.Owner);

            yield break;
        }
    }
}