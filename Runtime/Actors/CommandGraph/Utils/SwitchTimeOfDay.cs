using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.GameFlow;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Utils
{
    /// <summary>
    /// Command that switches the time of day to the given one.
    /// </summary>
    [Serializable]
    public class SwitchTimeOfDay : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Time to switch to.
        /// </summary>
        [SerializeField]
        private DayMoment TimeToSwitchTo;

        /// <summary>
        /// Switch to that time of day.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData, Action<CommandParameterData> callback)
        {
            yield return TransitionManager.BlackScreenFadeInRoutine();

            parameterData.TimeManager.SetNewDayMoment(TimeToSwitchTo);
            yield return new WaitForSeconds(.5f);

            yield return TransitionManager.BlackScreenFadeOutRoutine();
        }
    }
}