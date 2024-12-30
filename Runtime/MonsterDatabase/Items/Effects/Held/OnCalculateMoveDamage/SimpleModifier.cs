using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCalculateMoveDamage
{
    /// <summary>
    /// Data class for a held item effect that multiplies the damage of a move by a value of it matches the given type.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/OnCalculateMoveDamageWhenUsing/SimpleModifier",
                     fileName = "SimpleModifier")]
    public class SimpleModifier : OnCalculateMoveDamageItemEffect
    {
        /// <summary>
        /// Multiplier to apply.
        /// </summary>
        [SerializeField]
        private float Multiplier = 1f;

        /// <summary>
        /// Should consume item?
        /// </summary>
        [SerializeField]
        private bool ConsumeItem;

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
        /// <param name="finished">Callback stating if it should be consumed.</param>
        public override IEnumerator OnCalculateMoveDamage(DamageMove move,
                                                          Item item,
                                                          float multiplier,
                                                          Battler user,
                                                          Battler target,
                                                          BattleManager battleManager,
                                                          ILocalizer localizer,
                                                          Action<bool, float> finished)
        {
            Logger.Info("Multiplying damage of "
                      + localizer[move.LocalizableName]
                      + " by "
                      + Multiplier
                      + " using "
                      + item.GetName(localizer)
                      + ".");

            item.ShowItemNotification(user, localizer);

            finished.Invoke(ConsumeItem, multiplier * Multiplier);

            yield break;
        }
    }
}