using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Monsters
{
    /// <summary>
    /// Command that allows the player to change the nickname of a monster.
    /// </summary>
    [Serializable]
    public class ChangeMonsterNickname : CommandNode
    {
        /// <summary>
        /// Commands to run when the player sets the nickname.
        /// </summary>
        [HideInInspector]
        [SerializeReference]
        private CommandNode OnNicknameSet;

        /// <summary>
        /// Commands to run when the player cancels the nicknaming.
        /// </summary>
        [HideInInspector]
        [SerializeReference]
        private CommandNode OnCanceled;

        /// <summary>
        /// Change the nickname.
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

            bool newNickname = false;
            
            yield return DialogManager.RequestTextInput(parameterData.YAPUSettings.MaxNicknameSize,
                                                        "Dialogs/TextInput/Nickname",
                                                        new[] {monster.GetNameOrNickName(parameterData.Localizer)},
                                                        monster.GetIcon(),
                                                        (entered, text) =>
                                                        {
                                                            newNickname = entered;
                                                            if (!entered) return;
                                                            monster.Nickname = text;
                                                            monster.HasNickname = true;
                                                        });

            yield return TransitionManager.BlackScreenFadeOutRoutine();

            parameterData.InputManager.BlockInput(false);

            if (newNickname)
            {
                parameterData.ExtraParams[0] = monster;
                yield return OnNicknameSet.RunCommandAndContinue(parameterData);
            }
            else
                yield return OnCanceled.RunCommandAndContinue(parameterData);
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
                "On Nickname Set",
                "On Canceled"
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
                    OnNicknameSet = child;
                    break;
                case 1:
                    OnCanceled = child;
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
                case 0 when OnNicknameSet == child:
                    OnNicknameSet = null;
                    break;
                case 1 when OnCanceled == child:
                    OnCanceled = null;
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

            if (OnNicknameSet != null) children.Add(OnNicknameSet);
            if (OnCanceled != null) children.Add(OnCanceled);

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
                OnNicknameSet,
                OnCanceled
            };
    }
}