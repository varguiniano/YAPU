using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph.CommandUtils;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph.Dialogs;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.Monster.Breeding;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Breeding
{
    /// <summary>
    /// Actor command to store two selected monsters in the nursery.
    /// </summary>
    [Serializable]
    public class StoreTwoSelectedMonstersInNursery : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Nursery to use.
        /// </summary>
        [SerializeField]
        private Nursery Nursery;

        /// <summary>
        /// Dialog to show when the nursery is not empty.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        private ShowDialog AlreadyOccupiedDialog;

        /// <summary>
        /// Dialog to show when they are the same monster.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        private ShowDialog SameMonsterDialog;

        /// <summary>
        /// Dialog to show when they can't be bred.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        private ShowDialog CantBreedDialog;

        /// <summary>
        /// Commands to run when successfully stored.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        private CommandNode Success;

        /// <summary>
        /// Take the two mons, breed an egg with the nursery and give it to the player's.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            MonsterInstance firstParent = (MonsterInstance) parameterData.ExtraParams[0];
            bool firstMonsterIsInRoster = (CommandParameter<bool>) parameterData.ExtraParams[1];
            MonsterInstance secondParent = (MonsterInstance) parameterData.ExtraParams[2];
            bool secondMonsterIsInRoster = (CommandParameter<bool>) parameterData.ExtraParams[3];

            if (Nursery.IsOccupied)
            {
                yield return AlreadyOccupiedDialog.RunCommandAndContinue(parameterData);

                yield break;
            }

            if (firstParent == secondParent)
            {
                yield return SameMonsterDialog.RunCommandAndContinue(parameterData);

                yield break;
            }

            if (!Nursery.CanBeBred(firstParent, secondParent))
            {
                yield return CantBreedDialog.RunCommandAndContinue(parameterData);

                yield break;
            }

            if (firstMonsterIsInRoster)
                parameterData.PlayerCharacter.PlayerRoster.RemoveMonster(firstParent);
            else
                parameterData.PlayerCharacter.PlayerStorage.RemoveMonster(firstParent);

            if (secondMonsterIsInRoster)
                parameterData.PlayerCharacter.PlayerRoster.RemoveMonster(secondParent);
            else
                parameterData.PlayerCharacter.PlayerStorage.RemoveMonster(secondParent);

            Nursery.StoreMonsters(firstParent, secondParent, parameterData.PlayerCharacter.GlobalGameData);

            if (Success != null) yield return Success.RunCommandAndContinue(parameterData);
        }

        /// <summary>
        /// Get the output ports for this node.
        /// By default, it has one for execution out.
        /// </summary>
        public override List<string> GetOutputPorts() =>
            new()
            {
                "",
                "Already occupied dialog",
                "Same monster dialog",
                "Can't breed dialog",
                "Success"
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
                    AlreadyOccupiedDialog = child as ShowDialog;
                    break;
                case 2:
                    SameMonsterDialog = child as ShowDialog;
                    break;
                case 3:
                    CantBreedDialog = child as ShowDialog;
                    break;
                case 4:
                    Success = child;
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
                case 1 when AlreadyOccupiedDialog == child:
                    AlreadyOccupiedDialog = null;
                    break;
                case 2 when SameMonsterDialog == child:
                    SameMonsterDialog = null;
                    break;
                case 3 when CantBreedDialog == child:
                    CantBreedDialog = null;
                    break;
                case 4 when Success == child:
                    Success = null;
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
            if (AlreadyOccupiedDialog != null) children.Add(AlreadyOccupiedDialog);
            if (SameMonsterDialog != null) children.Add(SameMonsterDialog);
            if (CantBreedDialog != null) children.Add(CantBreedDialog);
            if (Success != null) children.Add(Success);

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
                AlreadyOccupiedDialog,
                SameMonsterDialog,
                CantBreedDialog,
                Success
            };
    }
}