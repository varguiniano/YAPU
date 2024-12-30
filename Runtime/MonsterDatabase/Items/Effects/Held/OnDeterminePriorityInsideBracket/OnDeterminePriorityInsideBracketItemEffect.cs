using System;
using System.Collections;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnDeterminePriorityInsideBracket
{
    /// <summary>
    /// Data class representing a held item effect that is called when determining priority inside the same priority bracket.
    /// </summary>
    public abstract class
        OnDeterminePriorityInsideBracketItemEffect : MonsterDatabaseScriptable<
        OnDeterminePriorityInsideBracketItemEffect>
    {
        /// <summary>
        /// Called each time an action has been performed in battle.
        /// </summary>
        /// <param name="item">Reference to the held item.</param>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed and true if it should go first (1), last (-1) or normal (0).</param>
        public abstract IEnumerator OnDeterminePriority(Item item,
                                                        Battler battler,
                                                        BattleManager battleManager,
                                                        ILocalizer localizer,
                                                        Action<bool, int> finished);
    }
}