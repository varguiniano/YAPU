using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Base class for an ability that boosts a move when the move is of a type and the user has less than a percentage of its HP.
    /// </summary>
    public abstract class BoostMoveWhenTypeAndBelowHealthAbility : Ability
    {
        /// <summary>
        /// Threshold from which to apply the ability.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        [PropertyRange(0, 1)]
        private float HPThreshold = .5f;

        /// <summary>
        /// Type of moves to boost.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsterTypes))]
        #endif
        [FoldoutGroup("Effect")]
        [SerializeField]
        private MonsterType MoveType;

        /// <summary>
        /// Boost to apply to the moves.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private float Boost = 1.5f;

        /// <summary>
        /// Called when calculating a move's damage.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="multiplier"></param>
        /// <param name="user">Battler using the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="effectiveness">Move effectiveness.</param>
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
            if (move.GetMoveTypeInBattle(user, battleManager) == MoveType
             && user.CurrentHP <= MonsterMathHelper.CalculateStat(user, Stat.Hp, battleManager) * HPThreshold)
            {
                ShowAbilityNotification(user);
                multiplier *= Boost;
            }

            finished.Invoke(multiplier);
            yield break;
        }
    }
}