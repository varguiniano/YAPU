using System;
using System.Collections;
using Varguiniano.YAPU.Runtime.GameFlow;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Saves
{
    /// <summary>
    /// Auto save if it is enabled in the config.
    /// </summary>
    [Serializable]
    public class AutoSaveIfEnabled : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Auto save if it is enabled in the config.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            if (!parameterData.ConfigurationManager.GetConfiguration(out GameplayConfiguration configuration))
            {
                Logger.Error("Couldn't retrieve gameplay configuration.");
                yield break;
            }

            if (configuration.AutoSaveOnStory)
                yield return parameterData.SavegameManager.Autosave(parameterData.PlayerCharacter);
        }
    }
}