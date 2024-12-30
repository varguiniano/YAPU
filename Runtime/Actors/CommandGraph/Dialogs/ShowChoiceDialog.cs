using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using CharacterController = Varguiniano.YAPU.Runtime.Characters.CharacterController;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Dialogs
{
    /// <summary>
    /// Show a dialog in which the player makes a choice.
    /// </summary>
    [Serializable]
    public class ShowChoiceDialog : CommandNode
    {
        /// <summary>
        /// Localization key for the dialog to show.
        /// </summary>
        [SerializeField]
        private string DialogLocalizationKey;

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
        [HideIf("@"
              + nameof(ShowCharacterNameIfAvailable)
              + " || "
              + nameof(OwnCharacter)
              + " || "
              + nameof(PlayerCharacter))]
        private CharacterData OtherCharacter;

        /// <summary>
        /// Choice to show the player and commands to run on each.
        /// </summary>
        [SerializeField]
        [InfoBox("Only a max of 8 choices is supported.",
                 InfoMessageType.Error,
                 VisibleIf = "@Choices != null && Choices.Count > 8")]
        private List<string> Choices = new();

        /// <summary>
        /// Commands to execute for each choice.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        private List<CommandNode> ChoiceCommands = new();

        /// <summary>
        /// Flag to mark if pressing back selects the last option.
        /// </summary>
        [SerializeField]
        private bool PressingBackWorksAsLastOption;

        /// <summary>
        /// Show the dialog.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            ActorCharacter actorCharacter = parameterData.Actor as ActorCharacter;

            int choice = -2;

            DialogManager.ShowChoiceMenu(Choices,
                                         option => choice = option,
                                         onBackCallback: PressingBackWorksAsLastOption
                                                             ? () => choice = -1
                                                             : null,
                                         character: ExtractCharacterData(actorCharacter, parameterData.PlayerCharacter),
                                         monster: ExtractMonsterData(actorCharacter),
                                         showDialog: true,
                                         localizationKey: DialogLocalizationKey);

            yield return new WaitWhile(() => choice == -2);

            if (choice == -1) choice = Choices.Count - 1;

            if (ChoiceCommands[choice] != null)
                yield return ChoiceCommands[choice].RunCommandAndContinue(parameterData);
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

        /// <summary>
        /// Update the cases list when the cases are updated.
        /// </summary>
        [Button]
        private void UpdateCases()
        {
            if (Choices.Count < ChoiceCommands.Count)
                ChoiceCommands.RemoveRange(Choices.Count, ChoiceCommands.Count - Choices.Count);
            else if (Choices.Count > ChoiceCommands.Count)
                ChoiceCommands.AddRange(Enumerable.Repeat<CommandNode>(null, Choices.Count - ChoiceCommands.Count));

            OnPortsUpdated?.Invoke();
        }

        /// <summary>
        /// Get the input ports for this node.
        /// </summary>
        public override List<string> GetInputPorts() => new() {""};

        /// <summary>
        /// Get the output ports for this node.
        /// </summary>
        public override List<string> GetOutputPorts() => Choices.Select(caseInt => caseInt.ToString()).ToList();

        /// <summary>
        /// Add a child to this node.
        /// Normal command nodes only have a single output port.
        /// </summary>
        /// <param name="child">Child to add.</param>
        /// <param name="index">Index of the port to use.</param>
        public override void AddChild(CommandNode child, int index)
        {
            if (index < Choices.Count) ChoiceCommands[index] = child;
        }

        /// <summary>
        /// Remove a child from this node.
        /// Normal command nodes only have a single output port.
        /// </summary>
        /// <param name="child">Child to remove.</param>
        /// <param name="index">Index of the port to use.</param>
        public override void RemoveChild(CommandNode child, int index)
        {
            if (index < Choices.Count && ChoiceCommands[index] == child) ChoiceCommands[index] = null;
        }

        /// <summary>
        /// Get all the children of this node.
        /// Normal command nodes only have a single output port.
        /// </summary>
        /// <returns>A list of all the children.</returns>
        public override List<CommandNode> GetChildren() => ChoiceCommands.Where(node => node != null).ToList();

        /// <summary>
        /// Get all the children of this node, including nulls, so they are indexed.
        /// </summary>
        /// <returns>A list of all the children.</returns>
        public override List<CommandNode> GetIndexedChildren() => ChoiceCommands;
    }
}