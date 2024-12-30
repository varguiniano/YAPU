using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph.Dialogs;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Monsters
{
    /// <summary>
    /// Command to remind the previously learnt moves of a chosen monster.
    /// </summary>
    [Serializable]
    public class RemindPreviouslyLearntMoves : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Dialog to request the player to choose.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        private ShowDialog ChooseDialog;

        /// <summary>
        /// Dialog to display when no moves are available.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        private ShowDialog NoMovesDialog;

        /// <summary>
        /// Teach previous moves.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            MonsterInstance monster = (MonsterInstance) parameterData.ExtraParams[0];

            if (monster == null)
            {
                Logger.Error("Monster wasn't passed as param!");
                yield break;
            }

            List<MonsterDatabase.Moves.Move> movesToRemind = monster.LearntMoves
                                                                    .Except(monster.CurrentMoves
                                                                               .Select(slot => slot.Move)
                                                                               .Where(moveInSlot => moveInSlot != null))
                                                                    .ToList();

            if (movesToRemind.Count == 0)
                yield return NoMovesDialog.RunCommandAndContinue(parameterData);
            else
            {
                yield return ChooseDialog.RunCommandAndContinue(parameterData);

                yield return DialogManager.ShowMoveTutorDialog(monster, movesToRemind);
            }
        }

        /// <summary>
        /// Get the output ports for this node.
        /// By default, it has one for execution out.
        /// </summary>
        public override List<string> GetOutputPorts() =>
            new()
            {
                "",
                "No Moves Dialog",
                "Choose Dialog"
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
                    NextCommand = child;
                    break;
                case 1:
                    NoMovesDialog = child as ShowDialog;
                    break;
                case 2:
                    ChooseDialog = child as ShowDialog;
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
                case 0 when NextCommand == child:
                    NextCommand = null;
                    break;
                case 1 when NoMovesDialog == child:
                    NoMovesDialog = null;
                    break;
                case 2 when ChooseDialog == child:
                    ChooseDialog = null;
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

            if (NextCommand != null) children.Add(NextCommand);
            if (NoMovesDialog != null) children.Add(NoMovesDialog);
            if (ChooseDialog != null) children.Add(ChooseDialog);

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
                NextCommand,
                NoMovesDialog,
                ChooseDialog
            };
    }
}