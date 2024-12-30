using System;
using System.Collections;
using System.Collections.Generic;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Damage move that deals damage equal to the user's level.
    /// </summary>
    public abstract class DamageEqualToUserLevelMove : DamageMove
    {
        /// <summary>
        /// Damage is always the target's current HP.
        /// </summary>
        protected override IEnumerator CalculateMoveDamage(BattleManager battleManager,
                                                           ILocalizer localizer,
                                                           Battler user,
                                                           Battler target,
                                                           int hitNumber,
                                                           int totalHits,
                                                           bool isCritical,
                                                           float typeEffectiveness,
                                                           float externalPowerMultiplier,
                                                           bool ignoresAbilities,
                                                           List<(BattlerType Type, int Index)> allTargets,
                                                           Action<float> finished)
        {
            finished.Invoke(user.StatData.Level);
            yield break;
        }
    }
}