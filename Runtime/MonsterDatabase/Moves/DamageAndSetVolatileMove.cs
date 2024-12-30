using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Base class for a move that damages and then sets a volatile status on the user or target.
    /// </summary>
    public abstract class DamageAndSetVolatileMove : DamageMove
    {
        /// <summary>
        /// Status to set.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        protected VolatileStatus Status;

        /// <summary>
        /// Should the status be forever?
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private bool InfiniteDuration;

        /// <summary>
        /// Set the status on the user (or the target)?
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private bool SetOnUser;

        /// <summary>
        /// Does this move have a secondary effect?
        /// </summary>
        public override bool HasSecondaryEffect() => true;

        /// <summary>
        /// Set a volatile status on the targets.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="user"></param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="hitNumber">This is the number of hits already made in this move. It will be always 0 unless it's a multihit move.</param>
        /// <param name="expectedHits">Expected move hits.</param>
        /// <param name="externalPowerMultiplier">External multiplier applied to the power.</param>
        /// <param name="ignoresAbilities"></param>
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
            yield return base.ExecuteSecondaryEffect(battleManager,
                                                     localizer,
                                                     userType,
                                                     userIndex,
                                                     user,
                                                     targets,
                                                     hitNumber,
                                                     expectedHits,
                                                     externalPowerMultiplier,
                                                     ignoresAbilities);

            object[] extraData =
                PrepareExtraData(battleManager, userType, userIndex, userType, userIndex);

            foreach ((BattlerType targetType, int targetIndex) in targets)
            {
                BattlerType statusTargetType = SetOnUser ? userType : targetType;
                int statusTargetIndex = SetOnUser ? userIndex : targetIndex;

                Battler target = battleManager.Battlers
                                              .GetBattlerFromBattleIndex(statusTargetType,
                                                                         statusTargetIndex);

                if (!SetOnUser
                 && !target.IsAffectedBySecondaryEffectsOfDamageMove(battleManager.Battlers
                                                                        .GetBattlerFromBattleIndex(userType, userIndex),
                                                                     this,
                                                                     LastDamageMade,
                                                                     ignoresAbilities,
                                                                     battleManager))
                    yield break;

                if (!battleManager.Statuses.HasStatus(Status, statusTargetType, statusTargetIndex))
                    yield return battleManager.Statuses.AddStatus(Status,
                                                                  CalculateCountdown(battleManager,
                                                                      userType,
                                                                      userIndex,
                                                                      statusTargetType,
                                                                      statusTargetIndex),
                                                                  target,
                                                                  userType,
                                                                  userIndex,
                                                                  ignoresAbilities,
                                                                  extraData);
            }
        }

        /// <summary>
        /// Prepare the extra data for the volatile status.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="targetIndex">Index of the target.</param>
        protected virtual object[] PrepareExtraData(BattleManager battleManager,
                                                    BattlerType userType,
                                                    int userIndex,
                                                    BattlerType targetType,
                                                    int targetIndex) =>
            null;

        /// <summary>
        /// Calculate the countdown 
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="targetIndex">Index of the target.</param>
        protected virtual int CalculateCountdown(BattleManager battleManager,
                                                 BattlerType userType,
                                                 int userIndex,
                                                 BattlerType targetType,
                                                 int targetIndex) =>
            InfiniteDuration
                ? -1
                : Status.CalculateRandomCountdown(battleManager, userType, userIndex, targetType, targetIndex);
    }
}