using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Side;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Base class for damage moves that also remove a status.
    /// </summary>
    public abstract class DamageAndRemoveStatusMove : StageChanceDamageMove
    {
        /// <summary>
        /// Remove the status on our own side?
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private bool RemoveVolatileStatusOnOwnSide;

        /// <summary>
        /// Status this move can remove.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private List<VolatileStatus> RemovableVolatileStatuses;

        /// <summary>
        /// Remove the status on our own side?
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private bool RemoveSideStatusOnOwnSide;

        /// <summary>
        /// Status this move can remove.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private List<SideStatus> RemovableSideStatuses;

        /// <summary>
        /// Remove the side status if it's on the list.
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

            yield return RemoveStatuses(battleManager,
                                        userType,
                                        userIndex,
                                        user,
                                        targets,
                                        hitNumber,
                                        expectedHits,
                                        externalPowerMultiplier,
                                        ignoresAbilities);
        }

        /// <summary>
        /// Remove the statuses in the field, side and battler.
        /// </summary>
        protected virtual IEnumerator RemoveStatuses(BattleManager battleManager,
                                                     BattlerType userType,
                                                     int userIndex,
                                                     Battler user,
                                                     List<(BattlerType Type, int Index)> targets,
                                                     int hitNumber,
                                                     int expectedHits,
                                                     float externalPowerMultiplier,
                                                     bool ignoreAbilities)
        {
            foreach (VolatileStatus removableVolatileStatus in RemovableVolatileStatuses)
                if (RemoveVolatileStatusOnOwnSide)
                {
                    if (battleManager.Statuses.HasStatus(removableVolatileStatus, userType, userIndex))
                        yield return battleManager.Statuses.RemoveStatus(removableVolatileStatus, userType, userIndex);
                }
                else
                    foreach ((BattlerType targetType, int targetIndex) in targets)
                    {
                        if (!battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex)
                                          .IsAffectedBySecondaryEffectsOfDamageMove(battleManager.Battlers
                                                  .GetBattlerFromBattleIndex(userType, userIndex),
                                               this,
                                               LastDamageMade,
                                               ignoreAbilities,
                                               battleManager))
                            yield break;

                        if (battleManager.Statuses.HasStatus(removableVolatileStatus, targetType, targetIndex))
                            yield return battleManager.Statuses.RemoveStatus(removableVolatileStatus,
                                                                             targetType,
                                                                             targetIndex);
                    }

            foreach (SideStatus removableSideStatus in RemovableSideStatuses)
                if (RemoveSideStatusOnOwnSide)
                {
                    if (battleManager.Statuses.GetSideStatuses(userType).ContainsKey(removableSideStatus))
                        yield return battleManager.Statuses.RemoveStatus(removableSideStatus, userType);
                }
                else
                    foreach ((BattlerType targetType, int targetIndex) in targets)
                    {
                        if (!battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex)
                                          .IsAffectedBySecondaryEffectsOfDamageMove(battleManager.Battlers
                                                  .GetBattlerFromBattleIndex(userType, userIndex),
                                               this,
                                               LastDamageMade,
                                               ignoreAbilities,
                                               battleManager))
                            yield break;

                        if (battleManager.Statuses.GetSideStatuses(targetType).ContainsKey(removableSideStatus))
                            yield return battleManager.Statuses.RemoveStatus(removableSideStatus, targetType);
                    }
        }
    }
}