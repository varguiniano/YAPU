using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.AfterHitByMove
{
    /// <summary>
    /// Data class for a held item effect that breaks the item if hit by a type of move.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/AfterHit/BreakItemIfHit", fileName = "BreakItemIfHit")]
    public class BreakItemIfHit : AfterHitByMoveItemEffect
    {
        /// <summary>
        /// Break if hit by this category.
        /// </summary>
        [SerializeField]
        private Move.Category BreakIfHitBy;

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
            item.ShowItemNotification(battler, battleManager.Localizer);
            finished.Invoke(move.GetMoveCategory(user, battler, ignoresAbilities, battleManager) == BreakIfHitBy);
            yield break;
        }
    }
}