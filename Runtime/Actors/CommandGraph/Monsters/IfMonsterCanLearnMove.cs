using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Monsters
{
    /// <summary>
    /// Actor command to check if a monster can learn a move.
    /// </summary>
    [Serializable]
    public class IfMonsterCanLearnMove : CommandNode
    {
        /// <summary>
        /// Move to learn.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMoves))]
        #endif
        [SerializeField]
        private MonsterDatabase.Moves.Move Move;

        /// <summary>
        /// Commands run when the move can be learnt.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        private CommandNode CanLearn;

        /// <summary>
        /// Commands run when the move cant be learnt.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        private CommandNode CantLearn;

        /// <summary>
        /// Commands run when the move is already known.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        private CommandNode AlreadyKnows;

        /// <summary>
        /// Check the move.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            if (parameterData.ExtraParams[0] is not MonsterInstance monster || monster.IsNullEntry)
            {
                Logger.Error("No monster selected.");
                yield break;
            }

            // Rebuild the params array.
            List<ICommandParameter> newParams = new(parameterData.ExtraParams) {Move};
            parameterData.ExtraParams = newParams.ToArray();

            if (monster.KnowsMove(Move))
                yield return AlreadyKnows.RunCommandAndContinue(parameterData);
            else if (monster.CanLearnMove(Move))
                yield return CanLearn.RunCommandAndContinue(parameterData);
            else
                yield return CantLearn.RunCommandAndContinue(parameterData);
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
                "Already Knows",
                "CanLearn",
                "CantLearn"
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
                    AlreadyKnows = child;
                    break;
                case 1:
                    CanLearn = child;
                    break;
                case 2:
                    CantLearn = child;
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
                case 0 when AlreadyKnows == child:
                    AlreadyKnows = null;
                    break;
                case 1 when CanLearn == child:
                    CanLearn = null;
                    break;
                case 2 when CantLearn == child:
                    CantLearn = null;
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

            if (AlreadyKnows != null) children.Add(AlreadyKnows);
            if (CanLearn != null) children.Add(CanLearn);
            if (CantLearn != null) children.Add(CantLearn);

            return children;
        }

        /// <summary>
        /// Get all the children of this node, including nulls, so they are indexed.
        /// </summary>
        /// <returns>A list of all the children.</returns>
        public override List<CommandNode> GetIndexedChildren() =>
            new()
            {
                AlreadyKnows,
                CanLearn,
                CantLearn
            };
    }
}