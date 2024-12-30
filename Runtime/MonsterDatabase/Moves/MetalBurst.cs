using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for MetalBurst.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Steel/MetalBurst", fileName = "MetalBurst")]
    public class MetalBurst : DamageMove
    {
        /// <summary>
        /// Check if the move will fail reasons other than accuracy.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="hitNumber"></param>
        /// <param name="expectedHits"></param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="customFailMessage">Custom message to be used when the move fails.</param>
        public override bool WillMoveFail(BattleManager battleManager,
                                          ILocalizer localizer,
                                          BattlerType userType,
                                          int userIndex,
                                          ref List<(BattlerType Type, int Index)> targets,
                                          int hitNumber,
                                          int expectedHits,
                                          bool ignoresAbilities,
                                          out string customFailMessage)
        {
            customFailMessage = "";

            Battler user = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            if (!user.ReceivedDamageThisTurn) return true;

            return base.WillMoveFail(battleManager,
                                     localizer,
                                     userType,
                                     userIndex,
                                     ref targets,
                                     hitNumber,
                                     expectedHits,
                                     ignoresAbilities,
                                     out customFailMessage);
        }

        /// <summary>
        /// Reselect the targets to the last received damage move user.
        /// </summary>
        internal override List<(BattlerType Type, int Index)> SelectFinalTargets(BattleManager battleManager,
            BattlerType userType,
            int userIndex,
            List<(BattlerType Type, int Index)> targets)
        {
            Battler user = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            if (user.LastReceivedDamageMove == null) return targets;

            Battler target = user.LastReceivedDamageMove.User;

            (BattlerType targetType, int targetIndex) =
                battleManager.Battlers.GetTypeAndIndexOfBattler(target);

            return new List<(BattlerType Type, int Index)> {(targetType, targetIndex)};
        }

        /// <summary>
        /// Last received damage * 1.5.
        /// Effectiveness cancellation is the only thing that affects this move.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="hitNumber"></param>
        /// <param name="totalHits"></param>
        /// <param name="isCritical">Is it a critical move?</param>
        /// <param name="typeEffectiveness">Type effectiveness.</param>
        /// <param name="externalPowerMultiplier">External multiplier applied to the power.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="allTargets">All of the move's targets.</param>
        /// <param name="finished">Callback with the amount of damage it deals.</param>
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
            finished.Invoke(typeEffectiveness == 0 ? 0 : user.LastReceivedDamageMove.Damage * 1.5f);
            yield break;
        }
    }
}