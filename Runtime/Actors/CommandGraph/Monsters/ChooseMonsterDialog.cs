using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph.CommandUtils;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Monsters
{
    /// <summary>
    /// Command to ask the player to choose a monster and run commands accordingly.
    /// </summary>
    [Serializable]
    public class ChooseMonsterDialog : CommandNode
    {
        /// <summary>
        /// Allow the player to use the storage.
        /// </summary>
        [SerializeField]
        private bool AllowStorage;

        /// <summary>
        /// Checker to see if the monster is compatible with the current dialog.
        /// </summary>
        [SerializeField]
        private MonsterCompatibilityChecker CompatibilityChecker;

        /// <summary>
        /// Commands to run when the player chooses a monster.
        /// </summary>
        [HideInInspector]
        [SerializeReference]
        private CommandNode OnMonsterChosen;

        /// <summary>
        /// Commands to run when the player cancels the choosing.
        /// </summary>
        [HideInInspector]
        [SerializeReference]
        private CommandNode OnChoiceCanceled;

        /// <summary>
        /// Request the player to choose a monster, then run commands if chosen or if not.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            bool choiceMade = false;
            bool chosen = false;
            MonsterInstance chosenMonster = null;
            bool isFromRoster = false;

            DialogManager.ShowPlayerRosterMenu(parameterData.PlayerCharacter,
                                               AllowStorage,
                                               openStorageDirectly: AllowStorage,
                                               isChoosingDialog: true,
                                               compatibilityChecker: CompatibilityChecker,
                                               onBackCallback:
                                               (didChoose, monsterReference, belongsToRoster) =>
                                               {
                                                   choiceMade = true;
                                                   chosen = didChoose;
                                                   chosenMonster = monsterReference;
                                                   isFromRoster = belongsToRoster;
                                               });

            yield return new WaitUntil(() => choiceMade);

            if (chosen)
            {
                parameterData.ExtraParams = new ICommandParameter[]
                                            {
                                                chosenMonster, (CommandParameter<bool>) isFromRoster
                                            };

                yield return OnMonsterChosen.RunCommandAndContinue(parameterData);
            }
            else
                yield return OnChoiceCanceled.RunCommandAndContinue(parameterData);
        }

        /// <summary>
        /// Get the input ports for this node.
        /// By default, it has one for execution in.
        /// </summary>
        public override List<string> GetInputPorts() => new() {""};

        /// <summary>
        /// Get the output ports for this node.
        /// </summary>
        public override List<string> GetOutputPorts() =>
            new()
            {
                "On Monster Chosen",
                "On Choice Canceled"
            };

        /// <summary>
        /// Add a child to this node.
        /// Normal command nodes only have a single output port.
        /// </summary>
        /// <param name="child">Child to add.</param>
        /// <param name="index">Index of the port to use.</param>
        public override void AddChild(CommandNode child, int index)
        {
            switch (index)
            {
                case 0:
                    OnMonsterChosen = child;
                    break;
                case 1:
                    OnChoiceCanceled = child;
                    break;
            }
        }

        /// <summary>
        /// Remove a child from this node.
        /// Normal command nodes only have a single output port.
        /// </summary>
        /// <param name="child">Child to remove.</param>
        /// <param name="index">Index of the port to use.</param>
        public override void RemoveChild(CommandNode child, int index)
        {
            switch (index)
            {
                case 0 when OnMonsterChosen == child:
                    OnMonsterChosen = null;
                    break;
                case 1 when OnChoiceCanceled == child:
                    OnChoiceCanceled = null;
                    break;
            }
        }

        /// <summary>
        /// Get all the children of this node.
        /// Normal command nodes only have a single output port.
        /// </summary>
        /// <returns>A list of all the children.</returns>
        public override List<CommandNode> GetChildren()
        {
            List<CommandNode> children = new();

            if (OnMonsterChosen != null) children.Add(OnMonsterChosen);
            if (OnChoiceCanceled != null) children.Add(OnChoiceCanceled);

            return children;
        }

        /// <summary>
        /// Get all the children of this node, including nulls, so they are indexed.
        /// </summary>
        /// <returns>A list of all the children.</returns>
        public override List<CommandNode> GetIndexedChildren() =>
            new()
            {
                OnMonsterChosen,
                OnChoiceCanceled
            };
    }
}