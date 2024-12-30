using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move NightShade.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Ghost/NightShade", fileName = "NightShade")]
    public class NightShade : DamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// Damage equal to the user level.
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