using System;
using System.Collections;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Configuration;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnYield
{
    /// <summary>
    /// Data class representing a held item effect that is called when yielding xp and ev.
    /// </summary>
    public abstract class OnYieldCallbackItemEffect : MonsterDatabaseScriptable<OnYieldCallbackItemEffect>
    {
        /// <summary>
        /// Called when yielding xp and ev.
        /// </summary>
        /// <param name="item">Reference to the held item.</param>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public abstract IEnumerator OnYield(Item item,
                                            Battler battler,
                                            YAPUSettings settings,
                                            BattleManager battleManager,
                                            ILocalizer localizer,
                                            Action<bool> finished);
    }
}