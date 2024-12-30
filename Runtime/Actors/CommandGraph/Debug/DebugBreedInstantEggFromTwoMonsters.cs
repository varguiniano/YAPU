using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph.Dialogs;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.Monster.Breeding;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Debug
{
    /// <summary>
    /// Actor command to instantly breed an egg from two previously chosen monsters.
    /// </summary>
    [Serializable]
    public class DebugBreedInstantEggFromTwoMonsters : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Nursery to use.
        /// </summary>
        [SerializeField]
        private Nursery Nursery;

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
        /// Commands to run when an egg has been bred.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        private CommandNode OnEggBred;

        /// <summary>
        /// Take the two mons, breed an egg with the nursery and give it to the player's.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            MonsterInstance firstParent = (MonsterInstance) parameterData.ExtraParams[0];
            MonsterInstance secondParent = (MonsterInstance) parameterData.ExtraParams[2];

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

            MonsterInstance egg =
                Nursery.Breed(firstParent,
                              secondParent,
                              parameterData.MonsterDatabase,
                              parameterData.YAPUSettings,
                              parameterData.PlayerCharacter,
                              parameterData.Localizer);

            yield return DialogManager.ShowNewMonsterDialog(parameterData.PlayerCharacter, egg, false, true);

            yield return TransitionManager.BlackScreenFadeOutRoutine();

            yield return OnEggBred.RunCommandAndContinue(parameterData);
        }

        /// <summary>
        /// Get the output ports for this node.
        /// By default, it has one for execution out.
        /// </summary>
        public override List<string> GetOutputPorts() =>
            new()
            {
                "",
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
                case 0: NextCommand = child; break;
                case 1: SameMonsterDialog = child as ShowDialog; break;
                case 2: CantBreedDialog = child as ShowDialog; break;
                case 3: OnEggBred = child; break;
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
                case 0 when NextCommand == child: NextCommand = null; break;
                case 1 when SameMonsterDialog == child: SameMonsterDialog = null; break;
                case 2 when CantBreedDialog == child: CantBreedDialog = null; break;
                case 3 when OnEggBred == child: OnEggBred = null; break;
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
            if (SameMonsterDialog != null) children.Add(SameMonsterDialog);
            if (CantBreedDialog != null) children.Add(CantBreedDialog);
            if (OnEggBred != null) children.Add(OnEggBred);

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
                SameMonsterDialog,
                CantBreedDialog,
                OnEggBred
            };
    }
}