using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move Comeuppance.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Dark/Comeuppance", fileName = "Comeuppance")]
    public class Comeuppance : DamageMove
    {
        /// <summary>
        /// Fail if it wasn't hit this turn.
        /// </summary>
        internal override bool WillMoveFail(BattleManager battleManager,
                                            ILocalizer localizer,
                                            BattlerType userType,
                                            int userIndex,
                                            BattlerType targetType,
                                            int targetIndex,
                                            bool ignoresAbilities)
        {
            Battler user = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);
            LastDamageMoveReceivedData lastMoveData = user.LastReceivedDamageMove;

            return !user.ReceivedDamageThisTurn
                || lastMoveData == null
                || lastMoveData.User.LastPerformedAction.LastAction != BattleAction.Type.Move
                || !battleManager.Battlers.IsBattlerFighting(battleManager.Battlers
                                                                          .GetTypeAndIndexOfBattler(lastMoveData.User))
                || base.WillMoveFail(battleManager,
                                     localizer,
                                     userType,
                                     userIndex,
                                     targetType,
                                     targetIndex,
                                     ignoresAbilities);
        }

        /// <summary>
        /// Reselect the targets to the last received damage move user.
        /// </summary>
        internal override List<(BattlerType Type, int Index)> SelectFinalTargets(BattleManager battleManager,
            BattlerType userType,
            int userIndex,
            List<(BattlerType Type, int Index)> targets)
        {
            LastDamageMoveReceivedData lastDamage = battleManager
                                                   .Battlers.GetBattlerFromBattleIndex(userType, userIndex)
                                                   .LastReceivedDamageMove;

            if (lastDamage == null) return new List<(BattlerType Type, int Index)>();

            Battler target = lastDamage.User;

            (BattlerType targetType, int targetIndex) =
                battleManager.Battlers.GetTypeAndIndexOfBattler(target);

            return new List<(BattlerType Type, int Index)> {(targetType, targetIndex)};
        }

        /// <summary>
        /// Double the last received damage.
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

        // TODO: Animation.
    }
}