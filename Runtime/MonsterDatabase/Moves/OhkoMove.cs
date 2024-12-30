using System;
using System.Collections;
using System.Collections.Generic;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Base class for moves that KO the target in one hit.
    /// </summary>
    public abstract class OhkoMove : DamageMove
    {
        /// <summary>
        /// It's not affected by accuracy or evasion stages.
        /// </summary>
        public override float CalculateAccuracy(Battler user,
                                                Battler target,
                                                bool ignoresAbilities,
                                                BattleManager battleManager) =>
            GetPreStageAccuracy(user, target, ignoresAbilities, battleManager);

        /// <summary>
        /// Formula: https://bulbapedia.bulbagarden.net/wiki/Horn_Drill_(move)#Generation_III_onwards
        /// </summary>
        public override int GetPreStageAccuracy(Battler user,
                                                Battler target,
                                                bool ignoresAbilities,
                                                BattleManager battleManager)
        {
            if (target == null) return BaseAccuracy;

            if (user.StatData.Level < target.StatData.Level) return 0;

            return user.StatData.Level - target.StatData.Level + BaseAccuracy;
        }

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
            finished.Invoke(target.CurrentHP);
            yield break;
        }
    }
}