using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Base class for abilities that power the damage of critical moves.
    /// </summary>
    public abstract class MultiplyDamageOfCriticalMoves : Ability
    {
        /// <summary>
        /// Multiplier to apply to the damage.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private float DamageMultiplier = 1.5f;

        /// <summary>
        /// Called when calculating a move's damage.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="multiplier">Current move multiplier.</param>
        /// <param name="user">Battler using the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="effectiveness">Move's effectiveness.</param>
        /// <param name="isCritical">Is it a critical hit?</param>
        /// <param name="hitNumber">Number of the current hit.</param>
        /// <param name="expectedHitNumber">Expected number of hits.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="allTargets">All of the move's targets.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Callback stating the new multiplier.</param>
        public override IEnumerator OnCalculateMoveDamageWhenUsing(DamageMove move,
                                                                   float multiplier,
                                                                   Battler user,
                                                                   Battler target,
                                                                   float effectiveness,
                                                                   bool isCritical,
                                                                   int hitNumber,
                                                                   int expectedHitNumber,
                                                                   bool ignoresAbilities,
                                                                   List<(BattlerType Type, int Index)> allTargets,
                                                                   BattleManager battleManager,
                                                                   ILocalizer localizer,
                                                                   Action<float> finished)
        {
            if (isCritical)
            {
                multiplier *= DamageMultiplier;
                ShowAbilityNotification(user);
            }

            finished.Invoke(multiplier);
            yield break;
        }
    }
}