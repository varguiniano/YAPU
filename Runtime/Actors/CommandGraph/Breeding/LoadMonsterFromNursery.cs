using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph.Utils;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.Monster.Breeding;
using CharacterController = Varguiniano.YAPU.Runtime.Characters.CharacterController;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Breeding
{
    /// <summary>
    /// Command that loads a monster from a nursery and makes this actor look like it.
    /// </summary>
    [Serializable]
    public class LoadMonsterFromNursery : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Nursery to load the monster from.
        /// </summary>
        [SerializeField]
        private Nursery Nursery;

        /// <summary>
        /// Flag to load the first or the second monster.
        /// </summary>
        [SerializeField]
        private bool LoadSecondMonster;

        /// <summary>
        /// Command to destroy it if the nursery is empty.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        private AutoDestroy DestroyCommand;

        /// <summary>
        /// Load the monster and make this actor look like it.
        /// Destroy it if the nursery is empty.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            if (!Nursery.IsOccupied)
            {
                yield return DestroyCommand.RunCommandAndContinue(parameterData);

                yield break;
            }

            ActorCharacter actorCharacter = (ActorCharacter) parameterData.Actor;

            actorCharacter.CharacterController.Mode = CharacterController.CharacterMode.Monster;

            (MonsterInstance firstMonster, MonsterInstance secondMonster) = Nursery.GetMonsterReferences();

            // Monsters to load into characters must be in rosters.
            Roster monsterRoster = ScriptableObject.CreateInstance<Roster>();
            monsterRoster.AddMonster(LoadSecondMonster ? secondMonster : firstMonster);

            actorCharacter.CharacterController.MonsterRoster = monsterRoster;

            actorCharacter.CharacterController.OnMonsterChanged();
        }

        /// <summary>
        /// Get the output ports for this node.
        /// By default, it has one for execution out.
        /// </summary>
        public override List<string> GetOutputPorts() =>
            new()
            {
                "",
                "Auto Destroy"
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
                    DestroyCommand = child as AutoDestroy;
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
                case 1 when DestroyCommand == child:
                    DestroyCommand = null;
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
            if (DestroyCommand != null) children.Add(DestroyCommand);

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
                DestroyCommand
            };
    }
}