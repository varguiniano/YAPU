using System;
using System.Collections;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Utils
{
    /// <summary>
    /// Log a message when the command is run.
    /// </summary>
    [Serializable]
    public class LogMessage : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Message to log.
        /// </summary>
        [SerializeField]
        private string Message;

        /// <summary>
        /// Log type.
        /// </summary>
        [SerializeField]
        private LogType LogType = LogType.Log;

        /// <summary>
        /// Log the message.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData, Action<CommandParameterData> callback)
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (LogType)
            {
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    Logger.Error(Message);
                    break;
                case LogType.Warning:
                    Logger.Warn(Message);
                    break;
                default:
                    Logger.Info(Message);
                    break;
            }

            yield break;
        }
    }
}