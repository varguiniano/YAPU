using System;
using System.Collections;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Dialogs
{
    /// <summary>
    /// Show a dialog that can be skipped.
    /// </summary>
    [Serializable]
    public class ShowNoSkippableDialog : ShowDialog
    {
        /// <summary>
        /// Run the command.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            ActorCharacter actorCharacter = parameterData.Actor as ActorCharacter;

            DialogManager.ShowDialog(DialogLocalizationKey,
                                     ExtractCharacterData(actorCharacter, parameterData.PlayerCharacter),
                                     ExtractMonsterData(actorCharacter),
                                     false);

            yield break;
        }
    }
}