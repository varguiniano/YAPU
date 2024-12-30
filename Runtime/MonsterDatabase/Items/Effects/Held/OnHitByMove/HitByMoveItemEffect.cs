using System;
using System.Collections;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnHitByMove
{
    /// <summary>
    /// Data class for held item effects that are called when the holder is hit by a move.
    /// </summary>
    public abstract class HitByMoveItemEffect : MonsterDatabaseScriptable<HitByMoveItemEffect>
    {
        /// <summary>
        /// Called when the holder is hit by a move.
        /// </summary>
        /// <param name="item">Item held.</param>
        /// <param name="move">The hitting move.</param>
        /// <param name="effectiveness">Its effectiveness.</param>
        /// <param name="battler">Battler holding the item.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="moveUser">User of the move.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Callback stating if it should be consumed and the new effectiveness.</param>
        public abstract IEnumerator OnHitByMove(Item item,
                                                DamageMove move,
                                                float effectiveness,
                                                Battler battler,
                                                BattleManager battleManager,
                                                Battler moveUser,
                                                ILocalizer localizer,
                                                Action<bool, float> finished);
    }
}