using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.PostTurn
{
    /// <summary>
    /// Data class for a held item effect that adds a status to the holder after each turn.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/PostTurn/AddStatusToHolderPostTurn",
                     fileName = "AddStatusToHolderPostTurn")]
    public class AddStatusToHolderPostTurn : PostTurnCallbackItemEffect
    {
        /// <summary>
        /// Status to set to the holder.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllStatuses))]
        #endif
        [SerializeField]
        private Status StatusToSet;

        /// <summary>
        /// Custom message to show.
        /// </summary>
        [SerializeField]
        private string CustomMessage;

        /// <summary>
        /// Called once after each turn before statuses have ticked.
        /// </summary>
        /// <param name="item">Reference to the held item.</param>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public override IEnumerator AfterTurnPreStatus(Item item,
                                                       Battler battler,
                                                       BattleManager battleManager,
                                                       ILocalizer localizer,
                                                       Action<bool> finished)
        {
            finished.Invoke(false);
            yield break;
        }

        /// <summary>
        /// Called once after each turn.
        /// </summary>
        /// <param name="item">Reference to the held item.</param>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public override IEnumerator AfterTurnPostStatus(Item item,
                                                        Battler battler,
                                                        BattleManager battleManager,
                                                        ILocalizer localizer,
                                                        Action<bool> finished)
        {
            finished.Invoke(false);

            if (!StatusToSet.CanAddStatus(battler, battleManager, battler, false)) yield break;

            item.ShowItemNotification(battler, battleManager.Localizer);

            if (!CustomMessage.IsNullEmptyOrWhiteSpace())
                yield return DialogManager.ShowDialogAndWait(CustomMessage,
                                                             localizableModifiers: false,
                                                             modifiers: battler.GetNameOrNickName(localizer),
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);

            yield return battleManager.Statuses.AddStatus(StatusToSet, battler, battler, false);
        }
    }
}