using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.UI.Bags;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Items
{
    /// <summary>
    /// Command to give the player money.
    /// </summary>
    [Serializable]
    public class GivePlayerMoney : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Amount to give to the player.
        /// </summary>
        [SerializeField]
        private uint Amount = 1;

        /// <summary>
        /// Type of message to display.
        /// </summary>
        [SerializeField]
        private GivePlayerItem.Message MessageType;

        /// <summary>
        /// Audio to play when given an item.
        /// </summary>
        [SerializeField]
        private AudioReference GiveItemAudio = new() {Audio = "Tutututi"};

        /// <summary>
        /// Give the player an item.
        /// </summary>
        protected override IEnumerator Run(CommandParameterData parameterData,
                                           Action<CommandParameterData> callback)
        {
            string messageKey = MessageType switch
            {
                GivePlayerItem.Message.Found => "Dialogs/Money/Found",
                GivePlayerItem.Message.Given => "Dialogs/Money/Given",
                _ => throw new ArgumentOutOfRangeException()
            };

            AudioManager.Instance.PlayAudio(GiveItemAudio);

            parameterData.PlayerCharacter.PlayerBag.Money += Amount;

            yield return DialogManager.ShowDialogAndWait(messageKey,
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        MoneyHelper.BuildMoneyString(Amount,
                                                                            parameterData.YAPUSettings,
                                                                            parameterData.Localizer)
                                                                    });
        }
    }
}