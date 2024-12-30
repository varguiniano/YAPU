using System;
using System.Collections;
using System.Collections.Generic;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Same as DamageAndRemoveStatusMove but inverted.
    /// </summary>
    public abstract class RemoveStatusAndDamageMove : DamageAndRemoveStatusMove
    {
        /// <summary>
        /// Remove the statuses before damaging.
        /// </summary>
        public override IEnumerator ExecuteEffect(BattleManager battleManager,
                                                  ILocalizer localizer,
                                                  BattlerType userType,
                                                  int userIndex,
                                                  Battler user,
                                                  List<(BattlerType Type, int Index)> targets,
                                                  int hitNumber,
                                                  int expectedHits,
                                                  float externalPowerMultiplier,
                                                  bool ignoresAbilities,
                                                  Action<bool> finishedCallback)
        {
            yield return RemoveStatuses(battleManager,
                                        userType,
                                        userIndex,
                                        user,
                                        targets,
                                        hitNumber,
                                        expectedHits,
                                        externalPowerMultiplier,
                                        ignoresAbilities);

            yield return base.ExecuteEffect(battleManager,
                                            localizer,
                                            userType,
                                            userIndex,
                                            user,
                                            targets,
                                            hitNumber,
                                            expectedHits,
                                            externalPowerMultiplier,
                                            ignoresAbilities,
                                            finishedCallback);
        }

        /// <summary>
        /// No secondary effect since this is done on the main effect.
        /// </summary>
        public override IEnumerator ExecuteSecondaryEffect(BattleManager battleManager,
                                                           ILocalizer localizer,
                                                           BattlerType userType,
                                                           int userIndex,
                                                           Battler user,
                                                           List<(BattlerType Type, int Index)> targets,
                                                           int hitNumber,
                                                           int expectedHits,
                                                           float externalPowerMultiplier,
                                                           bool ignoresAbilities)
        {
            yield break;
        }
    }
}