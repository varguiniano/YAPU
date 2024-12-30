using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster.Breeding;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Breeding
{
    /// <summary>
    /// Command to update the steps since the last visit to the nursery.
    /// </summary>
    [Serializable]
    public class UpdateNurserySteps : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Nursery to use.
        /// </summary>
        [SerializeField]
        private Nursery Nursery;

        /// <summary>
        /// Update the steps.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            Nursery.UpdateSteps(parameterData.PlayerCharacter.GlobalGameData);
            yield break;
        }
    }
}