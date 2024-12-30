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
    /// Base class for a move that sets a layered side status on the field.
    /// </summary>
    public abstract class SetLayeredSideStatusMove : Move
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
            (BattlerType targetType, int targetIndex) = targets[0];

            Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex);

            if (!battleManager.Statuses.HasStatus(Status, targetType))
                yield return battleManager.Statuses.AddStatus(Status,
                                                              targetType,
                                                              targetIndex,
                                                              Turns,
                                                              userType,
                                                              userIndex);

            yield return Status.AddLayers(targetType, Layers, target, user, battleManager);

            finishedCallback.Invoke(true);
        }
    }
}