using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using CharacterController = Varguiniano.YAPU.Runtime.Characters.CharacterController;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Dialogs
{
    /// <summary>
    /// Show a common dialog when the command is run.
    /// </summary>
    [Serializable]
    public class ShowDialog : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Localization key for the dialog to show.
        /// </summary>
        [SerializeField]
        protected string DialogLocalizationKey;

        /// <summary>
        /// Show the character name if available.
        /// </summary>
        [SerializeField]
        private bool ShowCharacterNameIfAvailable = true;

        /// <summary>
        /// Show the name of this own character?
        /// </summary>
        [SerializeField]
        [ShowIf(nameof(ShowCharacterNameIfAvailable))]
        private bool OwnCharacter = true;

        /// <summary>
        /// Show the name of the player character?
        /// </summary>
        [SerializeField]
        [ShowIf("@" + nameof(ShowCharacterNameIfAvailable) + " && !" + nameof(OwnCharacter))]
        private bool PlayerCharacter;

        /// <summary>
        /// Reference to the other character to show.
        /// </summary>
        [SerializeField]
        [HideIf("@!"
              + nameof(ShowCharacterNameIfAvailable)
              + " || "
              + nameof(OwnCharacter)
              + " || "
              + nameof(PlayerCharacter))]
        private CharacterData OtherCharacter;

        /// <summary>
        /// Run the command.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            List<string> modifiers = new();

            foreach (ICommandParameter param in parameterData.ExtraParams)
                modifiers.Add(param.GetLocalizedName(parameterData.Localizer));

            ActorCharacter actorCharacter = parameterData.Actor as ActorCharacter;

            yield return DialogManager.ShowDialogAndWait(DialogLocalizationKey,
                                                         ExtractCharacterData(actorCharacter,
                                                                              parameterData.PlayerCharacter),
                                                         ExtractMonsterData(actorCharacter),
                                                         localizableModifiers: false,
                                                         modifiers: modifiers.ToArray());
        }

        /// <summary>
        /// Extract the character data to use on the dialog.
        /// </summary>
        /// <param name="actorCharacter">Reference to its own actor character.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        protected virtual CharacterData ExtractCharacterData(ActorCharacter actorCharacter,
                                                             PlayerCharacter playerCharacter)
        {
            if (!ShowCharacterNameIfAvailable) return null;

            if (OwnCharacter)
                return actorCharacter != null ? actorCharacter.CharacterController.GetCharacterData() : null;

            return PlayerCharacter ? playerCharacter.CharacterController.GetCharacterData() : OtherCharacter;
        }

        /// <summary>
        /// Extract the monster data to use on the dialog.
        /// </summary>
        /// <param name="actorCharacter">Reference to its own actor character.</param>
        protected virtual MonsterInstance ExtractMonsterData(ActorCharacter actorCharacter)
        {
            if (!ShowCharacterNameIfAvailable) return null;

            if (!OwnCharacter) return null;

            return actorCharacter.CharacterController.Mode == CharacterController.CharacterMode.Monster
                       ? actorCharacter.CharacterController.GetMonsterData()
                       : null;
        }
    }
}