using System;
using System.Collections;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph.Items
{
    /// <summary>
    /// Command to give the player an item.
    /// </summary>
    [Serializable]
    public class GivePlayerItem : SingleExecutionPathCommandNode
    {
        /// <summary>
        /// Mode to give the item.
        /// </summary>
        [SerializeField]
        private GiveMode Mode;

        /// <summary>
        /// Item to give in override mode.
        /// </summary>
        [SerializeField]
        [ShowIf("@" + nameof(Mode) + " == GiveMode.Override")]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllItems))]
        #endif
        private Item Item;

        /// <summary>
        /// Amount to give to the player.
        /// </summary>
        [SerializeField]
        [InfoBox("Item is retrieved from the actor.", VisibleIf = "@" + nameof(Mode) + " == GiveMode.FromActor")]
        [OnValueChanged(nameof(OnAmountChanged))]
        private uint Amount = 1;

        /// <summary>
        /// Type of message to display.
        /// </summary>
        [SerializeField]
        private Message MessageType;

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
            Item itemToGive = Mode == GiveMode.Override ? Item : ((ActorItem) parameterData.Actor).Item;

            string messageKey = MessageType switch
            {
                Message.Found => "Dialogs/Items/Found",
                Message.Given => "Dialogs/Items/Given",
                _ => throw new ArgumentOutOfRangeException()
            };

            AudioManager.Instance.PlayAudio(GiveItemAudio);

            parameterData.PlayerCharacter.PlayerBag.ChangeItemAmount(itemToGive, (int) Amount);

            yield return DialogManager.ShowDialogAndWait(messageKey,
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        Amount.ToString(),
                                                                        itemToGive.GetName(parameterData.Localizer)
                                                                    });
        }

        /// <summary>
        /// Make sure the amount is more than 0.
        /// Only to be used in editor.
        /// </summary>
        private void OnAmountChanged()
        {
            if (Amount < 1) Amount = 1;
        }

        /// <summary>
        /// Modes to give the item.
        /// </summary>
        private enum GiveMode
        {
            [UsedImplicitly]
            FromActor,
            Override
        }

        /// <summary>
        /// Types of message to display.
        /// </summary>
        public enum Message
        {
            Found,
            Given
        }
    }
}