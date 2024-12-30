using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for Spite.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Psychic/EerieSpell", fileName = "EerieSpell")]
    public class EerieSpell : DamageMove
    {
        /// <summary>
        /// Reduce PP of the last move used by the target by 3.
        /// </summary>
        protected override IEnumerator ExecuteDamageEffect(BattleManager battleManager,
                                                           ILocalizer localizer,
                                                           BattlerType userType,
                                                           int userIndex,
                                                           Battler user,
                                                           BattlerType targetType,
                                                           int targetIndex,
                                                           Battler target,
                                                           List<(BattlerType Type, int Index)> targets,
                                                           int hitNumber,
                                                           int expectedMoveHits,
                                                           float externalPowerMultiplier,
                                                           bool ignoresAbilities,
                                                           Action<bool> finishedCallback,
                                                           bool forceSurvive = false)
        {
            yield return base.ExecuteDamageEffect(battleManager,
                                                  localizer,
                                                  userType,
                                                  userIndex,
                                                  user,
                                                  targetType,
                                                  targetIndex,
                                                  target,
                                                  targets,
                                                  hitNumber,
                                                  expectedMoveHits,
                                                  externalPowerMultiplier,
                                                  ignoresAbilities,
                                                  finishedCallback,
                                                  forceSurvive);

            if (target.LastPerformedAction.LastMove == null) yield break;

            for (int i = 0; i < target.CurrentMoves.Length; i++)
            {
                if (target.CurrentMoves[i].Move != target.LastPerformedAction.LastMove) continue;

                target.CurrentMoves[i].CurrentPP = (byte) Mathf.Max(0, target.CurrentMoves[i].CurrentPP - 3);
            }
        }
    }
}