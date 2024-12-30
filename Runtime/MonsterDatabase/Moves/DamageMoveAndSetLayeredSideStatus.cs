using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Side;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Combination of a damage move and a layered side status.
    /// </summary>
    public abstract class DamageMoveAndSetLayeredSideStatus : DamageMove
    {
        /// <summary>
        /// Status to add.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllLayeredSideStatuses))]
        #endif
        [FoldoutGroup("Effect")]
        [SerializeField]
        private LayeredSideStatus Status;

        /// <summary>
        /// The turns the status will last.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private int Turns;

        /// <summary>
        /// The layers to add on a single move.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private uint Layers = 1;

        /// <summary>
        /// Set a side status on the targets.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="user"></param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="hitNumber"></param>
        /// <param name="expectedHits"></param>
        /// <param name="externalPowerMultiplier"></param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="finishedCallback">Callback stating if the move successfully executed.</param>
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

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach ((BattlerType Type, int Index) target in targets)
            {
                if (!battleManager.Battlers.GetBattlerFromBattleIndex(target)
                                  .IsAffectedBySecondaryEffectsOfDamageMove(user,
                                                                            this,
                                                                            LastDamageMade,
                                                                            ignoresAbilities,
                                                                            battleManager))
                    yield break;

                if (!battleManager.Statuses.HasStatus(Status, userType))
                    yield return battleManager.Statuses.AddStatus(Status,
                                                                  userType,
                                                                  userIndex,
                                                                  Turns,
                                                                  userType,
                                                                  userIndex);

                yield return Status.AddLayers(userType,
                                              GetNumberOfLayers(user, battleManager),
                                              user,
                                              user,
                                              battleManager);
            }
        }

        /// <summary>
        /// Get the number of layers to set.
        /// </summary>
        protected virtual uint GetNumberOfLayers(Battler user, BattleManager battleManager) => Layers;
    }
}