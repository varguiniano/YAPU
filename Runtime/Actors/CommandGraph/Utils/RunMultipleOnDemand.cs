using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using WhateverDevs.Core.Runtime.Common;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Utils
{
    /// <summary>
    /// Command to run multiple actors on demand commands.
    /// </summary>
    [Serializable]
    public class RunMultipleOnDemand : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Target of the command.
        /// </summary>
        [SerializeField]
        [InfoBox("Will only wait for the last one.")]
        private List<Actor> Targets;

        /// <summary>
        /// Index of the on demand command list to use.
        /// </summary>
        [SerializeField]
        [PropertyRange(0, 3)]
        private int OnDemandCommandListIndex;

        /// <summary>
        /// Run the on demand commands of the actors.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData, Action<CommandParameterData> callback)
        {
            for (int i = 0; i < Targets.Count; i++)
            {
                Actor target = Targets[i];

                if (i < Targets.Count - 1)
                    CoroutineRunner.RunRoutine(target.RunOnDemandCommandGraphs(OnDemandCommandListIndex,
                                                                               parameterData.PlayerCharacter,
                                                                               parameterData.PlayerDirection,
                                                                               parameterData.Callback));
                else
                    yield return target.RunOnDemandCommandGraphs(OnDemandCommandListIndex,
                                                                 parameterData.PlayerCharacter,
                                                                 parameterData.PlayerDirection,
                                                                 parameterData.Callback);

                parameterData.Actor.RunningCommands = true;
            }
        }
    }
}