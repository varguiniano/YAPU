using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.Common;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnVolatileStatusAdded
{
    /// <summary>
    /// Data class for a held item effect that heals the just added volatile statuses if they match the list.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/OnVolatileStatusAdded/HealJustAddedVolatileStatuses",
                     fileName = "HealJustAddedVolatileStatuses")]
    public class HealJustAddedVolatileStatuses : OnVolatileStatusAddedItemEffect
    {
        /// <summary>
        /// Volatile statuses to heal.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllVolatileStatuses))]
        #endif
        [SerializeField]
        private List<VolatileStatus> StatusesToHeal;

        /// <summary>
        /// Custom message to show when healing.
        /// </summary>
        [SerializeField]
        private string CustomMessage;

        /// <summary>
        /// Called when a volatile status is added to the holder.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="item">Item that has this effect.</param>
        /// <param name="holder">Holder of the item.</param>
        /// <param name="userType">Type of the one that added the status.</param>
        /// <param name="userIndex">Index of the one that added the status.</param>
        /// <param name="status">Status added.</param>
        /// <param name="countdown">Countdown established.</param>
        /// <param name="finished">Callback establishing if the item should be consumed.</param>
        public override IEnumerator OnVolatileStatusAdded(BattleManager battleManager,
                                                          Item item,
                                                          Battler holder,
                                                          BattlerType userType,
                                                          int userIndex,
                                                          VolatileStatus status,
                                                          int countdown,
                                                          Action<bool> finished)
        {
            if (!StatusesToHeal.Contains(status)) yield break;

            item.ShowItemNotification(holder, battleManager.Localizer);

            if (!CustomMessage.IsNullEmptyOrWhiteSpace())
                yield return DialogManager.ShowDialogAndWait(CustomMessage,
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);

            yield return battleManager.Statuses.RemoveStatus(status, holder);
            finished.Invoke(true);
        }
    }
}