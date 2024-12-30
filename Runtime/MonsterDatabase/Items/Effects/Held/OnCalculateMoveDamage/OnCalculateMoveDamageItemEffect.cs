using System;
using System.Collections;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCalculateMoveDamage
{
    /// <summary>
    /// Data class for held item effects that are called when calculating the damage of a move the user is about to perform.
    /// </summary>
    public abstract class OnCalculateMoveDamageItemEffect : MonsterDatabaseScriptable<OnCalculateMoveDamageItemEffect>
    {
        /// <summary>
        /// Called when calculating a move's damage.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="item">Item held.</param>
        /// <param name="multiplier"></param>
        /// <param name="user">Battler holding the item.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Callback stating if it should be consumed and the new multiplier.</param>
        public abstract IEnumerator OnCalculateMoveDamage(DamageMove move,
                                                          Item item,
                                                          float multiplier,
                                                          Battler user,
                                                          Battler target,
                                                          BattleManager battleManager,
                                                          ILocalizer localizer,
                                                          Action<bool, float> finished);
    }
}