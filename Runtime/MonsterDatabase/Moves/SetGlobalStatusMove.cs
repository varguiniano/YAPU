using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Global;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Base class for moves that set a global status.
    /// </summary>
    public abstract class SetGlobalStatusMove : Move
    {
        /// <summary>
        /// Status to add.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllGlobalStatuses))]
        #endif
        [FoldoutGroup("Effect")]
        [SerializeField]
        private GlobalStatus Status;

        /// <summary>
        /// Will the status last forever?
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private bool InfiniteDuration;

        /// <summary>
        /// The turns the status will last.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        [HideIf(nameof(InfiniteDuration))]
        private int Turns;

        /// <summary>
        /// Remove the status if it was already in place?
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private bool RemoveIfAlreadyInPlace;

        /// <summary>
        /// Check if the move will fail reasons other than accuracy.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="targetIndex">Index of the target.</param>
        /// <param name="ignoresAbilities"></param>
        internal override bool WillMoveFail(BattleManager battleManager,
                                            ILocalizer localizer,
                                            BattlerType userType,
                                            int userIndex,
                                            BattlerType targetType,
                                            int targetIndex,
                                            bool ignoresAbilities) =>
            (battleManager.Statuses.HasStatus(Status) && !RemoveIfAlreadyInPlace)
         || base.WillMoveFail(battleManager, localizer, userType, userIndex, targetType, targetIndex, ignoresAbilities);

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
            if (RemoveIfAlreadyInPlace && battleManager.Statuses.HasStatus(Status))
                yield return battleManager.Statuses.RemoveStatus(Status);
            else
                yield return battleManager.Statuses.AddStatus(Status,
                                                              battleManager.Battlers.GetBattlerFromBattleIndex(userType,
                                                                  userIndex),
                                                              Turns);

            finishedCallback.Invoke(true);
        }
    }
}