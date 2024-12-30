using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Animations
{
    /// <summary>
    /// Command to make a character change its size in an amount of time.
    /// </summary>
    [Serializable]
    public class ChangeActorSize : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Target scale to achieve.
        /// </summary>
        [SerializeField]
        private Vector3 TargetScale;

        /// <summary>
        /// Time to do it.
        /// </summary>
        [SerializeField]
        private float Time;

        /// <summary>
        /// Change the size.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            yield return parameterData.Owner.transform.DOScale(TargetScale, Time).WaitForCompletion();
        }
    }
}