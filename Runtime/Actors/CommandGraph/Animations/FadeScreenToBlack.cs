using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Animations
{
    /// <summary>
    /// Actor command that fades the screen to black.
    /// </summary>
    [Serializable]
    public class FadeScreenToBlack : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Mode to use.
        /// </summary>
        [SerializeField]
        private CommandMode Mode;

        /// <summary>
        /// Fade the screen.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData, Action<CommandParameterData> callback)
        {
            if (Mode == CommandMode.FadeIn)
                yield return TransitionManager.BlackScreenFadeInRoutine();
            else
                yield return TransitionManager.BlackScreenFadeOutRoutine();
        }

        /// <summary>
        /// Modes this command has.
        /// </summary>
        public enum CommandMode
        {
            FadeIn,
            FadeOut
        }
    }
}