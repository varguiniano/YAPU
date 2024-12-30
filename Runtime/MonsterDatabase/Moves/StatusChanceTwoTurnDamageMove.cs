using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Mix of a status chance move and a two turn damage move.
    /// </summary>
    public abstract class StatusChanceTwoTurnDamageMove : TwoTurnDamageMove
    {
        /// <summary>
        /// Statuses  and their chance to be set.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        protected SerializedDictionary<Status, float> StatusChance;

        /// <summary>
        /// Volatile statuses, their countdown and their chance to be set.
        /// -1 is random countdown.
        /// </summary>
        [Tooltip("Volatile statuses, their countdown and their chance to be set. -1 is random countdown.")]
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializedDictionary<float, SerializedDictionary<VolatileStatus, int>> VolatileStatusChance;

        /// <summary>
        /// Does this move have a secondary effect?
        /// </summary>
        public override bool HasSecondaryEffect() => true;

        /// <summary>
        /// Execute the secondary effect of the move.
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

            foreach ((BattlerType targetType, int targetIndex) in targets)
            {
                Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex);

                if (!target.IsAffectedBySecondaryEffectsOfDamageMove(user,
                                                                     this,
                                                                     LastDamageMade,
                                                                     ignoresAbilities,
                                                                     battleManager))
                    yield break;

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (KeyValuePair<Status, float> pair in StatusChance)
                {
                    float chance = battleManager.RandomProvider.Value01()
                                 / user.GetMultiplierForChanceOfSecondaryEffectOfMove(targets, this, battleManager);

                    if (!(chance <= pair.Value)) continue;
                    bool canAdd = true;

                    yield return target.CanAddStatus(pair.Key,
                                                     battleManager,
                                                     userType,
                                                     userIndex,
                                                     ignoresAbilities,
                                                     true,
                                                     add => canAdd = add);

                    if (canAdd)
                        yield return
                            battleManager.Statuses.AddStatus(pair.Key,
                                                             target,
                                                             userType,
                                                             userIndex,
                                                             ignoreAbilities: ignoresAbilities);
                }

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (KeyValuePair<float, SerializedDictionary<VolatileStatus, int>> pair in VolatileStatusChance)
                {
                    float chance = battleManager.RandomProvider.Value01()
                                 / user.GetMultiplierForChanceOfSecondaryEffectOfMove(targets, this, battleManager);

                    if (!(chance <= pair.Key)) continue;
                    bool canAdd = true;

                    foreach (KeyValuePair<VolatileStatus, int> statusToAdd in pair.Value)
                    {
                        yield return target.CanAddStatus(statusToAdd.Key,
                                                         battleManager,
                                                         userType,
                                                         userIndex,
                                                         ignoresAbilities,
                                                         add => canAdd = add);

                        if (canAdd)
                            yield return
                                battleManager.Statuses.AddStatus(statusToAdd.Key,
                                                                 statusToAdd.Value == -1
                                                                     ? CalculateRandomCountdown(battleManager,
                                                                         statusToAdd.Key,
                                                                         userType,
                                                                         userIndex,
                                                                         targetType,
                                                                         targetIndex)
                                                                     : statusToAdd.Value,
                                                                 targetType,
                                                                 targetIndex,
                                                                 userType,
                                                                 userIndex,
                                                                 ignoreAbilities: ignoresAbilities);
                    }
                }
            }
        }

        /// <summary>
        /// Calculate the countdown 
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="status">Status to add.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="targetIndex">Index of the target.</param>
        private static int CalculateRandomCountdown(BattleManager battleManager,
                                                    VolatileStatus status,
                                                    BattlerType userType,
                                                    int userIndex,
                                                    BattlerType targetType,
                                                    int targetIndex) =>
            status.CalculateRandomCountdown(battleManager, userType, userIndex, targetType, targetIndex);
    }
}