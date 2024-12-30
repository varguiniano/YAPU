using System;
using System.Collections;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.PostTurn
{
    /// <summary>
    /// Data class representing a held item effect that is called once after each turn in battle.
    /// </summary>
    public abstract class PostTurnCallbackItemEffect : MonsterDatabaseScriptable<PostTurnCallbackItemEffect>
    {
        /// <summary>
        /// Called once after each turn before statuses have ticked.
        /// </summary>
        /// <param name="item">Reference to the held item.</param>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public abstract IEnumerator AfterTurnPreStatus(Item item,
                                                       Battler battler,
                                                       BattleManager battleManager,
                                                       ILocalizer localizer,
                                                       Action<bool> finished);

        /// <summary>
        /// Called once after each turn after statuses have ticked.
        /// </summary>
        /// <param name="item">Reference to the held item.</param>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public abstract IEnumerator AfterTurnPostStatus(Item item,
                                                        Battler battler,
                                                        BattleManager battleManager,
                                                        ILocalizer localizer,
                                                        Action<bool> finished);
    }
}