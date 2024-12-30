using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Utils
{
    /// <summary>
    /// Actor command that caches the temporal variable storage of the actor
    /// so that there isn't a such a noticeable performance hit when used for the first time.
    /// </summary>
    [Serializable]
    public class CacheTemporalVariables : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Bool variables to cache with default values.
        /// </summary>
        [SerializeField]
        private List<string> BoolVariablesToCache;

        /// <summary>
        /// Int variables to cache with default values.
        /// </summary>
        [SerializeField]
        private List<string> IntVariablesToCache;

        /// <summary>
        /// Cache the storage and variables.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            TemporalVariables variables = parameterData.Actor.TemporalVariables;

            foreach (string variable in BoolVariablesToCache) variables.SetVariable(variable, default(bool));

            foreach (string variable in IntVariablesToCache) variables.SetVariable(variable, default(bool));

            yield break;
        }
    }
}