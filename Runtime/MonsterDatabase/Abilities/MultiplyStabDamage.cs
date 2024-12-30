using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Base class for an ability that multiplies the stab damage.
    /// </summary>
    public abstract class MultiplyStabDamage : Ability
    {
        /// <summary>
        /// Multiplier to apply.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private float Multiplier = 1;

        /// <summary>
        /// Called when calculating a move's stab damage.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="multiplier">Current move multiplier.</param>
        /// <param name="user">Battler using the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="effectiveness">Move's effectiveness.</param>
        /// <param name="isCritical">Is it a critical hit?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Callback stating the new multiplier.</param>
        public override IEnumerator OnCalculateStabDamageWhenUsing(DamageMove move,
                                                                   float multiplier,
                                                                   Battler user,
                                                                   Battler target,
                                                                   float effectiveness,
                                                                   bool isCritical,
                                                                   BattleManager battleManager,
                                                                   ILocalizer localizer,
                                                                   Action<float> finished)
        {
            ShowAbilityNotification(user);
            finished.Invoke(multiplier * Multiplier);
            yield break;
        }
    }
}