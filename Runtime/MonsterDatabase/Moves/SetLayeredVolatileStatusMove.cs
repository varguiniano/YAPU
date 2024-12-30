using System;
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
    /// Base class for a move that sets a layered volatile status on the field.
    /// </summary>
    public abstract class SetLayeredVolatileStatusMove : Move
    {
        /// <summary>
        /// Status to add.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllLayeredVolatileStatuses))]
        #endif
        [FoldoutGroup("Effect")]
        [SerializeField]
        private LayeredVolatileStatus Status;

        /// <summary>
        /// The turns the status will last.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private int Turns;

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

            if (!battleManager.Statuses.HasStatus(Status, target))
                yield return battleManager.Statuses.AddStatus(Status,
                                                              Turns,
                                                              target,
                                                              userType,
                                                              userIndex,
                                                              ignoresAbilities);

            yield return Status.AddLayer(target, user, battleManager);

            finishedCallback.Invoke(true);
        }
    }
}