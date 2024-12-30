using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph.CommandUtils;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Items
{
    /// <summary>
    /// Command to ask the player to choose an item and run commands accordingly.
    /// </summary>
    [Serializable]
    public class ChooseItemDialog : CommandNode
    {
        /// <summary>
        /// Checker to filter certain items.
        /// </summary>
        [SerializeField]
        private ItemCompatibilityChecker CompatibilityChecker;

        /// <summary>
        /// Commands to run when the player chooses an item.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        private CommandNode OnItemChosen;

        /// <summary>
        /// Commands to run when the player cancels the choosing.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        private CommandNode OnChoiceCanceled;

        /// <summary>
        /// Request the player to choose an item, then run commands if chosen or if not.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            bool choiceMade = false;
            bool chosen = false;
            Item itemChosen = null;

            DialogManager.ShowBag((didChoose, item) =>
                                  {
                                      choiceMade = true;
                                      chosen = didChoose;
                                      itemChosen = item;
                                  },
                                  parameterData.PlayerCharacter,
                                  selection: true,
                                  itemCompatibilityChecker: CompatibilityChecker);

            yield return new WaitUntil(() => choiceMade);

            if (chosen)
            {
                parameterData.ExtraParams = new ICommandParameter[] {itemChosen};
                yield return OnItemChosen.RunCommandAndContinue(parameterData);
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
        /// By default, it has one for execution out.
        /// </summary>
        public override List<string> GetOutputPorts() =>
            new()
            {
                "On Item Chosen",
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
                    OnItemChosen = child;
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
                case 0 when OnItemChosen == child:
                    OnItemChosen = null;
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

            if (OnItemChosen != null) children.Add(OnItemChosen);
            if (OnChoiceCanceled != null) children.Add(OnChoiceCanceled);

            return children;
        }

        /// <summary>
        /// Get all the children of this node, including nulls, so they are indexed.
        /// Normal command nodes only have a single output port.
        /// </summary>
        /// <returns>A list of all the children.</returns>
        public override List<CommandNode> GetIndexedChildren() =>
            new()
            {
                OnItemChosen,
                OnChoiceCanceled
            };
    }
}