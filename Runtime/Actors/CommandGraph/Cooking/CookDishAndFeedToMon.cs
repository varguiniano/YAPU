using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Berries;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Cooking
{
    /// <summary>
    /// Command to allow the player to cook four berries and feed the resulting dish to a mon.
    /// </summary>
    [Serializable]
    public class CookDishAndFeedToMon : CommandNode
    {
        /// <summary>
        /// Name of the variable that states if the player canceled.
        /// </summary>
        [SerializeField]
        private string PlayerCanceledVariable;

        /// <summary>
        /// Name of the variable that stores the chosen berry.
        /// </summary>
        [SerializeField]
        private string BerryVariableName;

        /// <summary>
        /// Command to be executed after the dish is cooked.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        public CommandNode NextCommand;

        /// <summary>
        /// Commands to run if the player canceled.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        private CommandNode PlayerCanceled;

        /// <summary>
        /// Dialog that will be used to choose berries.
        /// </summary>
        [SerializeReference]
        [HideInInspector]
        private CommandNode ChooseBerryDialog;

        /// <summary>
        /// Ask the player for four berries and then cook the dish and feed it to the mon.
        /// </summary>
        /// <param name="parameterData">Parameters from the previous command, will have the chosen mon.</param>
        /// <param name="callback">Callback with the parameters resulting from this command, if changed.</param>
        protected override IEnumerator Run(CommandParameterData parameterData, Action<CommandParameterData> callback)
        {
            if (parameterData.ExtraParams[0] is not MonsterInstance monster)
            {
                Logger.Error("We need a monster to feed!");
                yield break;
            }

            List<Berry> berries = new();
            bool playerCanceled = false;

            for (int i = 0; i < 4; ++i)
            {
                yield return ChooseBerryDialog.RunCommandAndContinue(parameterData);

                playerCanceled = parameterData.Actor.TemporalVariables.GetVariable<bool>(PlayerCanceledVariable);

                if (playerCanceled) break;

                berries.Add(parameterData.MonsterDatabase.GetItemByHash(parameterData.Actor.TemporalVariables
                                                                           .GetVariable<int>(BerryVariableName)) as
                                Berry);
            }

            if (playerCanceled)
            {
                // Return the already taken berries.
                foreach (Berry berry in berries) parameterData.PlayerCharacter.PlayerBag.ChangeItemAmount(berry, 1);

                yield return PlayerCanceled?.RunCommandAndContinue(parameterData);
            }
            else
            {
                StringBuilder debugText = new("Cook the dish for ");
                debugText.Append(monster.GetLocalizedName(parameterData.Localizer));
                debugText.Append(" with ");

                foreach (Berry berry in berries)
                {
                    debugText.Append(berry.GetLocalizedName(parameterData.Localizer));
                    debugText.Append(", ");
                }

                Logger.Info(debugText);

                (SerializableDictionary<Flavour, int> flavours, uint smoothness) =
                    BerriesUtils.CookDish(berries, Logger);

                yield return monster.EatDish(flavours,
                                             smoothness,
                                             parameterData.PlayerCharacter,
                                             parameterData.Localizer,
                                             _ =>
                                             {
                                             });

                yield return NextCommand?.RunCommandAndContinue(parameterData);
            }
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
                "DishCooked",
                "PlayerCanceled",
                "ChooseBerryDialog"
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
                    PlayerCanceled = child;
                    break;
                case 2:
                    ChooseBerryDialog = child;
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
                case 1 when PlayerCanceled == child:
                    PlayerCanceled = null;
                    break;
                case 2 when ChooseBerryDialog == child:
                    ChooseBerryDialog = null;
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

            if (PlayerCanceled != null) children.Add(PlayerCanceled);

            if (ChooseBerryDialog != null) children.Add(ChooseBerryDialog);

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
                PlayerCanceled,
                ChooseBerryDialog
            };
    }
}