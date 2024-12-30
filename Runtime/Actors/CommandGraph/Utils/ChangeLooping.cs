using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Utils
{
    /// <summary>
    /// Command to change whether a character is looping.
    /// </summary>
    [Serializable]
    public class ChangeLooping : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Loop?
        /// </summary>
        [SerializeField]
        private bool Loop;

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
        private Actor Target;

        /// <summary>
        /// Resume or stop the looping.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            (TargetItself ? parameterData.Actor : Target).ShouldLoop = Loop;

            parameterData.Callback.Invoke(new CommandCallbackParams
                                          {
                                              UpdateLooping = true,
                                              NewLooping = Loop
                                          });

            yield break;
        }
    }
}