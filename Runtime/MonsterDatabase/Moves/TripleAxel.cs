using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move TripleAxel.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Ice/TripleAxel", fileName = "TripleAxel")]
    public class TripleAxel : DamageMove
    {
        /// <summary>
        /// Abilities that bypass accuracy checks after the first.
        /// </summary>
        [FoldoutGroup("Targeting")]
        [SerializeField]
        private List<Ability> AccuracyBypassingAbilities;

        /// <summary>
        /// Recheck the accuracy each hit after the first.
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
            if (hitNumber > 0
             && (!user.CanUseAbility(battleManager, false) || !AccuracyBypassingAbilities.Contains(user.GetAbility())))
            {
                float accuracy = battleManager.Moves.CalculateAccuracyForTarget(user, target, this, ignoresAbilities);

                if (accuracy > battleManager.RandomProvider.Value01())
                {
                    yield return DialogManager.ShowDialogAndWait("Battle/Move/Failed",
                                                                 switchToNextAfterSeconds: 1.5f
                                                                   / battleManager.BattleSpeed);

                    finishedCallback.Invoke(false);
                    yield break;
                }
            }

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
        }

        /// <summary>
        /// Get more powerful each hit.
        /// </summary>
        public override int GetMovePowerInBattle(BattleManager battleManager,
                                                 Battler user,
                                                 Battler target,
                                                 bool ignoresAbilities,
                                                 int hitNumber = 0) =>
            base.GetMovePowerInBattle(battleManager, user, target, ignoresAbilities, hitNumber) * (hitNumber + 1);
    }
}