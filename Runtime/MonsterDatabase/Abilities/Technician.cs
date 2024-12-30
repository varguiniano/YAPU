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
    /// Technician ability implementation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Technician", fileName = "Technician")]
    public class Technician : Ability
    {
        /// <summary>
        /// Threshold to boost moves.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private short PowerThreshold = 60;

        /// <summary>
        /// Multiplier to apply to moves.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private float Multiplier = 1.5f;

        /// <summary>
        /// Boost the move if below the threshold.
        /// </summary>
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
            int power = move.GetMovePowerInBattle(battleManager, user, target, ignoresAbilities);

            if (power > PowerThreshold)
                yield return base.OnCalculateMoveDamageWhenUsing(move,
                                                                 multiplier,
                                                                 user,
                                                                 target,
                                                                 effectiveness,
                                                                 isCritical,
                                                                 hitNumber,
                                                                 expectedHitNumber,
                                                                 ignoresAbilities,
                                                                 allTargets,
                                                                 battleManager,
                                                                 localizer,
                                                                 finished);
            else
            {
                ShowAbilityNotification(user, true);

                finished.Invoke(multiplier * Multiplier);
            }
        }
    }
}