using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Berries;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Base class for a damage move that also consumes the target's berry.
    /// </summary>
    public abstract class DamageAndConsumeBerryMove : DamageMove
    {
        /// <summary>
        /// Abilities that will prevent stealing.
        /// </summary>
        [FoldoutGroup("Steal Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllAbilities))]
        #endif
        private List<Ability> StealImmuneAbilities;

        /// <summary>
        /// Consume the berry after the damage.
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

            if (LastDamageMade <= 0
             || !target.CanUseHeldItemInBattle(battleManager)
             || target.HeldItem is not Berry berry)
                yield break;

            if (target.CanUseAbility(battleManager, ignoresAbilities)
             && StealImmuneAbilities.Contains(target.GetAbility()))
            {
                target.GetAbility().ShowAbilityNotification(target);

                yield break;
            }

            yield return berry.UseOnTarget(user,
                                           battleManager,
                                           battleManager.YAPUSettings,
                                           battleManager.ExperienceLookupTable,
                                           battleManager.Localizer,
                                           _ =>
                                           {
                                           });

            user.HasEatenBerryThisBattle = true;

            yield return target.ConsumeItemInBattle(battleManager,
                                                    "Dialogs/Battle/AteBerry",
                                                    hasBeenConsumedByOwner: false);
        }
    }
}