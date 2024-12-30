using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Utils
{
    /// <summary>
    /// Run the on demand commands of an actor character.
    /// </summary>
    [Serializable]
    public class RunOnDemand : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Should the target be themselves?
        /// </summary>
        [SerializeField]
        private bool TargetThemselves;

        /// <summary>
        /// Target of the command.
        /// </summary>
        [HideIf(nameof(TargetThemselves))]
        [SerializeField]
        private Actor Target;

        /// <summary>
        /// Index of the on demand command list to use.
        /// </summary>
        [SerializeField]
        [PropertyRange(0, 3)]
        private int OnDemandCommandListIndex;

        /// <summary>
        /// Run the on demand commands of an actor character.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData, Action<CommandParameterData> callback)
        {
            Actor target = TargetThemselves ? parameterData.Actor : Target;

            yield return target.RunOnDemandCommandGraphs(OnDemandCommandListIndex,
                                                         parameterData.PlayerCharacter,
                                                         parameterData.PlayerDirection,
                                                         parameterData.Callback);

            parameterData.Actor.RunningCommands = true;
        }
    }
}