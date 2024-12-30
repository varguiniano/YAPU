using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Items
{
    /// <summary>
    /// Actor command to set an item chosen via dialog as a temporal int variable using its hash.
    /// </summary>
    [Serializable]
    public class SetChosenItemAsTempVariable : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Name of the variable.
        /// </summary>
        [SerializeField]
        private string IntVariableName;

        /// <summary>
        /// Set the value.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData, Action<CommandParameterData> callback)
        {
            if (parameterData.ExtraParams[0] is not Item item)
            {
                Logger.Error("The first parameter is not an item!");
                yield break;
            }

            parameterData.Actor.TemporalVariables.SetVariable(IntVariableName, item.name.GetHashCode());
        }
    }
}