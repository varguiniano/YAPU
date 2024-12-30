using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph.CommandUtils;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph.Dialogs;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Monsters
{
    /// <summary>
    /// Command to ask the player to choose two monsters and run commands accordingly.
    /// </summary>
    [Serializable]
    public class ChooseTwoMonstersDialog : CommandNode
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
        /// Dialog to use when player doesn't have enough.
        /// </summary>
        [HideInInspector]
        [SerializeReference]
        private ShowDialog NotEnoughMonsters;

        /// <summary>
        /// Dialog to use to ask for a second monster.
        /// </summary>
        [HideInInspector]
        [SerializeReference]
        private ShowDialog ChooseSecondMonsterDialog;

        /// <summary>
        /// Commands to run when the player chooses two monsters.
        /// </summary>
        [HideInInspector]
        [SerializeReference]
        private CommandNode OnMonstersChosen;

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
            if (parameterData.PlayerCharacter.PlayerRoster.RosterSize < 3)
            {
                yield return NotEnoughMonsters.RunCommandAndContinue(parameterData);

                yield break;
            }

            bool choiceMade = false;
            bool chosen = false;
            MonsterInstance firstChosenMonster = null;
            bool isFirstFromRoster = false;
            MonsterInstance secondChosenMonster = null;
            bool isSecondFromRoster = false;

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
                                                   firstChosenMonster = monsterReference;
                                                   isFirstFromRoster = belongsToRoster;
                                               });

            yield return new WaitUntil(() => choiceMade);

            yield return new WaitForEndOfFrame();

            if (chosen)
            {
                if (ChooseSecondMonsterDialog != null)
                    yield return ChooseSecondMonsterDialog.RunCommandAndContinue(parameterData);

                yield return new WaitForEndOfFrame();

                choiceMade = false;
                chosen = false;

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
                                                       secondChosenMonster = monsterReference;
                                                       isSecondFromRoster = belongsToRoster;
                                                   });

                yield return new WaitUntil(() => choiceMade);
            }

            if (chosen)
            {
                parameterData.ExtraParams = new ICommandParameter[]
                                            {
                                                firstChosenMonster,
                                                (CommandParameter<bool>) isFirstFromRoster,
                                                secondChosenMonster,
                                                (CommandParameter<bool>) isSecondFromRoster
                                            };

                yield return OnMonstersChosen.RunCommandAndContinue(parameterData);
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
                "Not Enough Monsters",
                "Choose Second Monster",
                "On Monsters Chosen",
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
                    NotEnoughMonsters = child as ShowDialog;
                    break;
                case 1:
                    ChooseSecondMonsterDialog = child as ShowDialog;
                    break;
                case 2:
                    OnMonstersChosen = child;
                    break;
                case 3:
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
                case 0 when NotEnoughMonsters == child:
                    NotEnoughMonsters = null;
                    break;
                case 1 when ChooseSecondMonsterDialog == child:
                    ChooseSecondMonsterDialog = null;
                    break;
                case 2 when OnMonstersChosen == child:
                    OnMonstersChosen = null;
                    break;
                case 3 when OnChoiceCanceled == child:
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

            if (NotEnoughMonsters != null) children.Add(NotEnoughMonsters);
            if (ChooseSecondMonsterDialog != null) children.Add(ChooseSecondMonsterDialog);
            if (OnMonstersChosen != null) children.Add(OnMonstersChosen);
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
                NotEnoughMonsters,
                ChooseSecondMonsterDialog,
                OnMonstersChosen,
                OnChoiceCanceled
            };
    }
}