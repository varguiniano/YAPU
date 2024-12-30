using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Battles
{
    /// <summary>
    /// Command to launch a fight from a template.
    /// </summary>
    [Serializable]
    public class FightFromTemplate : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Template to use.
        /// </summary>
        [SerializeField]
        private BattleTemplate Template;

        /// <summary>
        /// Commands run when the player defeats the opponents.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        private CommandNode OnDefeatedOpponents;

        /// <summary>
        /// Play the battle.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            BattleResultParameters result = null;

            parameterData.BattleLauncher.LaunchBattle(parameterData.PlayerCharacter,
                                                      Template,
                                                      battleResult => result = battleResult);

            yield return new WaitWhile(() => result == null);

            // ReSharper disable once PossibleNullReferenceException
            if (!result.PlayerWon || OnDefeatedOpponents == null) yield break;

            yield return OnDefeatedOpponents.RunCommandAndContinue(parameterData);
        }

        /// <summary>
        /// Get the output ports for this node.
        /// By default, it has one for execution out.
        /// </summary>
        public override List<string> GetOutputPorts() =>
            new()
            {
                "",
                "On Player Won"
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
                    OnDefeatedOpponents = child;
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
                case 1 when OnDefeatedOpponents == child:
                    OnDefeatedOpponents = null;
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
            if (OnDefeatedOpponents != null) children.Add(OnDefeatedOpponents);

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
                OnDefeatedOpponents
            };
    }
}