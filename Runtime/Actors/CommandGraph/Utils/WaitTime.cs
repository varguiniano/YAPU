using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Utils
{
    /// <summary>
    /// Wait a certain amount of time.
    /// </summary>
    [Serializable]
    public class WaitTime : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Type of wait.
        /// </summary>
        [SerializeField]
        private WaitType Time;

        /// <summary>
        /// Fixed seconds to wait.
        /// </summary>
        [ShowIf("@" + nameof(Time) + " == WaitType.Fixed")]
        [SerializeField]
        private float Seconds = 1;

        /// <summary>
        /// Minimum random time to wait.
        /// </summary>
        [ShowIf("@" + nameof(Time) + " == WaitType.Random")]
        [SerializeField]
        [OnValueChanged(nameof(ValidateRange))]
        private float MinWait = 1;

        /// <summary>
        /// Maximum random time to wait.
        /// </summary>
        [ShowIf("@" + nameof(Time) + " == WaitType.Random")]
        [SerializeField]
        [OnValueChanged(nameof(ValidateRange))]
        private float MaxWait = 2;

        /// <summary>
        /// Seconds at which the time wait can be interrupted to interact.
        /// </summary>
        private const float InterruptInterval = .025f;

        /// <summary>
        /// Wait a certain amount of time or a random range.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            float time = Time == WaitType.Fixed ? Seconds : Random.Range(MinWait, MaxWait);

            int cycles = (int) (time / InterruptInterval);

            for (int i = 0; i < cycles; ++i)
            {
                if (parameterData.IsRunningOnLoop)
                    if (!parameterData.Actor.IsLooping)
                    {
                        parameterData.Actor.LoopRunning = false;

                        yield return new WaitUntil(() => parameterData.Actor.IsLooping);

                        parameterData.Actor.LoopRunning = true;
                    }

                yield return new WaitForSeconds(InterruptInterval);
            }
        }

        /// <summary>
        /// Validate that the max is always at least the min.
        /// </summary>
        private void ValidateRange()
        {
            if (MaxWait < MinWait) MaxWait = MinWait;
        }

        /// <summary>
        /// Enumeration describing the different types of waits.
        /// </summary>
        public enum WaitType
        {
            Fixed,
            Random
        }
    }
}