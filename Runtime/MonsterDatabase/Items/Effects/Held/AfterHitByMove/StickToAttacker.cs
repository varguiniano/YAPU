using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.AfterHitByMove
{
    /// <summary>
    /// Data class for a held item effect that sticks the item to the attacker when hit.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/AfterHit/StickToAttacker", fileName = "StickToAttacker")]
    public class StickToAttacker : AfterHitByMoveItemEffect
    {
        /// <summary>
        /// Called after the holder is hit by a move.
        /// </summary>
        /// <param name="item">Item held.</param>
        /// <param name="move">The hitting move.</param>
        /// <param name="effectiveness">Its effectiveness.</param>
        /// <param name="battler">Battler holding the item.</param>
        /// <param name="user">Move user.</param>
        /// <param name="substituteTookHit"></param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Callback stating if it should be consumed.</param>
        public override IEnumerator AfterHitByMove(Item item,
                                                   DamageMove move,
                                                   float effectiveness,
                                                   Battler battler,
                                                   Battler user,
                                                   bool substituteTookHit,
                                                   bool ignoresAbilities,
                                                   BattleManager battleManager,
                                                   ILocalizer localizer,
                                                   Action<bool> finished)
        {
            // Won't work if user fainted.
            if (!user.CanBattle) yield break;

            // Can't steal if the user has an item.
            if (user.HeldItem != null) yield break;
            
            item.ShowItemNotification(battler, localizer);

            yield return battler.HeldItem.OnItemStolen(battler, battleManager);

            user.HeldItem = item;

            battler.ConsumedItemData.ConsumedItem = item;
            battler.ConsumedItemData.CanBeRecycled = false;
            battler.ConsumedItemData.CanBeRecoveredAfterBattle = true;

            battler.HeldItem = null;

            yield return DialogManager.ShowDialogAndWait("Dialogs/Items/StickyBarb/StuckToAttacker",
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed,
                                                         localizableModifiers: false,
                                                         modifiers: new[] {user.GetNameOrNickName(localizer)});
        }
    }
}