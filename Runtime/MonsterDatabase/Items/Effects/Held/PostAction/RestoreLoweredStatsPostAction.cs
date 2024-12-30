using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.PostAction
{
    /// <summary>
    /// Data class for a held item effect that will restore lowered stats after the holder does an action.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/PostAction/RestoreLoweredStatsPostAction",
                     fileName = "RestoreLoweredStatsPostAction")]
    public class RestoreLoweredStatsPostAction : PostActionCallbackItemEffect
    {
        /// <summary>
        /// Called each time an action has been performed in battle.
        /// </summary>
        /// <param name="item">Reference to the held item.</param>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="action">Action that was performed.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public override IEnumerator AfterAction(Item item,
                                                Battler battler,
                                                BattleAction action,
                                                BattleManager battleManager,
                                                ILocalizer localizer,
                                                Action<bool> finished)
        {
            if (!battler.ResetLoweredStatStages()) yield break;
            
            item.ShowItemNotification(battler, battleManager.Localizer);

            yield return DialogManager.ShowDialogAndWait("Battle/RestoredLoweredStats",
                                                         localizableModifiers: false,
                                                         modifiers: battler.GetNameOrNickName(localizer),
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            finished.Invoke(true);
        }
    }
}