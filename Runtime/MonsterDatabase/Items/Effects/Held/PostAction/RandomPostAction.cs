using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.PostAction
{
    /// <summary>
    /// Data class for a held item effect that will call a random effect from a list after an action has been performed.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/PostAction/RandomPostAction", fileName = "RandomPostAction")]
    public class RandomPostAction : PostActionCallbackItemEffect
    {
        /// <summary>
        /// List of effects from which the random effect will be pulled from.
        /// </summary>
        [SerializeField]
        private List<PostActionCallbackItemEffect> EffectPool = new();

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
            yield return battleManager.RandomProvider.RandomElement(EffectPool)
                                      .AfterAction(item, battler, action, battleManager, localizer, finished);
        }
    }
}