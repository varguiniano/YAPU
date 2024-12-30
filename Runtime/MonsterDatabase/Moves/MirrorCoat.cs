using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move MirrorCoat.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Psychic/MirrorCoat", fileName = "MirrorCoat")]
    public class MirrorCoat : DamageMove
    {
        /// <summary>
        /// Fail if it wasn't last hit by a special move.
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

            return lastMoveData == null
                || lastMoveData.Move.GetMoveCategory(user,
                                                     battleManager.Battlers.GetBattlerFromBattleIndex(targetType,
                                                         targetIndex),
                                                     ignoresAbilities,
                                                     battleManager)
                != Category.Special
                || !battleManager.Battlers.IsBattlerFighting(battleManager.Battlers
                                                                          .GetTypeAndIndexOfBattler(lastMoveData.User))
                || base.WillMoveFail(battleManager, localizer, userType, userIndex, targetType, targetIndex, ignoresAbilities);
        }

        /// <summary>
        /// Deal damage to the targets.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="user"></param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="hitNumber"></param>
        /// <param name="expectedMoveHits"></param>
        /// <param name="externalPowerMultiplier">External multiplier applied to the power.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="finishedCallback">Callback stating if the move successfully executed.</param>
        /// <param name="forceSurvive">Force the monster to survive to the hit?</param>
        /// <param name="user1"></param>
        protected override IEnumerator ExecuteDamageEffect(BattleManager battleManager,
                                                           ILocalizer localizer,
                                                           BattlerType userType,
                                                           int userIndex,
                                                           Battler user,
                                                           List<(BattlerType Type, int Index)> targets,
                                                           int hitNumber,
                                                           int expectedMoveHits,
                                                           float externalPowerMultiplier,
                                                           bool ignoresAbilities,
                                                           Action<bool> finishedCallback,
                                                           bool forceSurvive = false)
        {
            Battler target = user.LastReceivedDamageMove.User;

            (BattlerType targetType, int targetIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(target);

            yield return ExecuteDamageEffect(battleManager,
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
            finished.Invoke(typeEffectiveness == 0 ? 0 : user.LastReceivedDamageMove.Damage * 2);
            yield break;
        }

        // TODO: Animation instead of fallback.
    }
}