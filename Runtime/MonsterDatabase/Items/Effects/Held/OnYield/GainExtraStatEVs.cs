using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnYield
{
    /// <summary>
    /// Data class for an item effect to gain extra EVs when yielding XP and Evs in battle.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/OnYield/GainExtraStatEVs", fileName = "GainExtraStatEVs")]
    public class GainExtraStatEVs : OnYieldCallbackItemEffect
    {
        /// <summary>
        /// Stat to change.
        /// </summary>
        [SerializeField]
        private Stat Stat;

        /// <summary>
        /// Amount of EVs to change.
        /// </summary>
        [SerializeField]
        private short AmountToChange;

        /// <summary>
        /// Called when yielding xp and ev.
        /// </summary>
        /// <param name="item">Reference to the held item.</param>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public override IEnumerator OnYield(Item item,
                                            Battler battler,
                                            YAPUSettings settings,
                                            BattleManager battleManager,
                                            ILocalizer localizer,
                                            Action<bool> finished)
        {
            Logger.Info("Applying "
                      + item.GetName(localizer)
                      + " effect to gain "
                      + AmountToChange
                      + " "
                      + Stat
                      + " EVs to "
                      + battler.GetNameOrNickName(localizer)
                      + ".");

            battler.ChangeEV(settings, Stat, AmountToChange, localizer);

            finished.Invoke(false);

            yield break;
        }
    }
}