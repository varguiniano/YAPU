using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability SteelySpirit.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/SteelySpirit", fileName = "SteelySpirit")]
    public class SteelySpirit : Ability
    {
        /// <summary>
        /// Type of moves to boost.
        /// </summary>
        [SerializeField]
        private MonsterType TypeToBoost;

        /// <summary>
        /// Multiplier to apply to the moves.
        /// </summary>
        [SerializeField]
        private float Multiplier = 1.5f;

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
            if (move.GetMoveTypeInBattle(user, battleManager))
            {
                ShowAbilityNotification(user, true);
                multiplier *= Multiplier;
            }

            finished.Invoke(multiplier);
            yield break;
        }

        /// <summary>
        /// Called when calculating a move's damage when an ally uses it.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="move">The hitting move.</param>
        /// <param name="multiplier">Current move multiplier.</param>
        /// <param name="user">Ally using the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="effectiveness"></param>
        /// <param name="isCritical">Is it a critical hit?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Callback stating the new multiplier.</param>
        public override IEnumerator OnCalculateMoveDamageWhenAllyUsing(Battler owner,
                                                                       DamageMove move,
                                                                       float multiplier,
                                                                       Battler user,
                                                                       Battler target,
                                                                       float effectiveness,
                                                                       bool isCritical,
                                                                       BattleManager battleManager,
                                                                       ILocalizer localizer,
                                                                       Action<float> finished)
        {
            if (move.GetMoveTypeInBattle(user, battleManager))
            {
                ShowAbilityNotification(user, true);
                multiplier *= Multiplier;
            }

            finished.Invoke(multiplier);
            yield break;
        }
    }
}