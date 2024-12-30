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
    /// Data class for the ability ParentalBond.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/ParentalBond", fileName = "ParentalBond")]
    public class ParentalBond : Ability
    {
        /// <summary>
        /// Get the number of hits the move will do.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <returns>Tuple stating if the number has been modified and the new value.</returns>
        public override (bool, int) GetNumberOfHitsOfMultihitMove(BattleManager battleManager,
                                                                  Battler owner,
                                                                  Move move,
                                                                  List<(BattlerType Type, int Index)> targets)
        {
            if (!IsMoveAffected(move, targets)) return (false, 1);

            ShowAbilityNotification(owner);

            return (true, 2);
        }

        /// <summary>
        /// Second hit is 25%.
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
            finished.Invoke(multiplier * (IsMoveAffected(move, allTargets) && hitNumber == 1 ? 0.25f : 1));

            yield break;
        }

        /// <summary>
        /// Is the move affected?
        /// </summary>
        private static bool IsMoveAffected(Move move, ICollection targets) =>
            !move.IsMultiHit && move is DamageMove && targets.Count == 1;
    }
}