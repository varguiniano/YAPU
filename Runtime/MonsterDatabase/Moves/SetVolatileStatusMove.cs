using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for a move that sets a volatile status on the targets.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/General/SetVolatileStatusMove",
                     fileName = "SetVolatileStatusMove")]
    public class SetVolatileStatusMove : Move
    {
        /// <summary>
        /// Status to set.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllVolatileStatuses))]
        #endif
        protected VolatileStatus Status;

        /// <summary>
        /// Should the status be forever?
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        protected bool InfiniteDuration;

        /// <summary>
        /// Useful for moves that need a target but set on the user, like Mind Reader.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private bool SetStatusOnSelf;

        /// <summary>
        /// Set a volatile status on the targets.
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
            if (Status == null) yield break;

            foreach ((BattlerType targetType, int targetIndex) in targets)
            {
                Battler target = battleManager.Battlers
                                              .GetBattlerFromBattleIndex(targetType,
                                                                         targetIndex);

                if (WillMoveFail(battleManager,
                                 localizer,
                                 userType,
                                 userIndex,
                                 targetType,
                                 targetIndex,
                                 ignoresAbilities))
                {
                    yield return DialogManager.ShowDialogAndWait("Battle/Move/NoEffect",
                                                                 switchToNextAfterSeconds: 1.5f
                                                                   / battleManager.BattleSpeed);

                    finishedCallback.Invoke(false);
                    yield break;
                }

                object[] extraData =
                    PrepareExtraData(battleManager, userType, userIndex, targetType, targetIndex);

                yield return battleManager.Statuses.AddStatus(Status,
                                                              CalculateCountdown(battleManager,
                                                                  userType,
                                                                  userIndex,
                                                                  targetType,
                                                                  targetIndex),
                                                              SetStatusOnSelf ? user : target,
                                                              userType,
                                                              userIndex,
                                                              ignoresAbilities,
                                                              extraData);
            }

            finishedCallback.Invoke(true);
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