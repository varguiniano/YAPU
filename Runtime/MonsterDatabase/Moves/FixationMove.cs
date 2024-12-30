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
    /// Class for moves that make the user fixate and use it for several turns.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/General/FixationMove", fileName = "FixationMove")]
    public class FixationMove : DamageMove
    {
        /// <summary>
        /// Reference to the status that will store the build up for this move.
        /// </summary>
        [FoldoutGroup("Fixation Effect")]
        [SerializeField]
        private Fixation FixationStatus;

        /// <summary>
        /// Will the monster get confused when ending the fixation?
        /// </summary>
        [FoldoutGroup("Fixation Effect")]
        [SerializeField]
        private bool GetConfusedOnEnd = true;

        /// <summary>
        /// Reference to confusion to apply after the fixation is done.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllVolatileStatuses))]
        #endif
        [FoldoutGroup("Fixation Effect")]
        [SerializeField]
        [ShowIf(nameof(GetConfusedOnEnd))]
        private VolatileStatus Confusion;

        /// <summary>
        /// Dictionary used to keep track of which where the last targets of this move
        /// when used by a specific battler.
        /// </summary>
        public Dictionary<Battler, (BattlerType Type, int Index)> LastTargets = new();

        /// <summary>
        /// Check if the move will fail for reasons other than accuracy.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="hitNumber"></param>
        /// <param name="expectedHits"></param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="customFailMessage"></param>
        public override bool WillMoveFail(BattleManager battleManager,
                                          ILocalizer localizer,
                                          BattlerType userType,
                                          int userIndex,
                                          ref List<(BattlerType Type, int Index)> targets,
                                          int hitNumber,
                                          int expectedHits,
                                          bool ignoresAbilities,
                                          out string customFailMessage)
        {
            bool fail = base.WillMoveFail(battleManager,
                                          localizer,
                                          userType,
                                          userIndex,
                                          ref targets,
                                          hitNumber,
                                          expectedHits,
                                          ignoresAbilities,
                                          out customFailMessage);

            // Don't remove the status if this is the last hit of the row.
            // The target might have fainted but the combo hasn't been broken.
            if (fail && hitNumber != expectedHits - 1)
                battleManager.Statuses.ScheduleRemoveStatus(FixationStatus,
                                                            battleManager.Battlers.GetBattlerFromBattleIndex(userType,
                                                                userIndex));

            return fail;
        }

        /// <summary>
        /// Targets are selected at random if the original target was the user.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">User type.</param>
        /// <param name="userIndex">User index.</param>
        /// <param name="targets">Current preselected targets.</param>
        internal override List<(BattlerType Type, int Index)> SelectFinalTargets(BattleManager battleManager,
            BattlerType userType,
            int userIndex,
            List<(BattlerType Type, int Index)> targets)
        {
            List<(BattlerType Type, int Index)> newTargets;

            if (targets[0] != (userType, userIndex))
                newTargets = targets;
            else
            {
                List<Battler> candidates =
                    battleManager.Battlers.GetBattlersFighting(userType == BattlerType.Ally
                                                                   ? BattlerType.Enemy
                                                                   : BattlerType.Ally);

                if (candidates.Count == 0) candidates = battleManager.Battlers.GetBattlersFighting();

                if (candidates.Count == 0)
                {
                    Logger.Warn("No candidates found for fixation move.");
                    newTargets = targets;
                }
                else
                    newTargets = new List<(BattlerType Type, int Index)>
                                 {
                                     battleManager.Battlers
                                                  .GetTypeAndIndexOfBattler(battleManager.RandomProvider
                                                                               .RandomElement(candidates))
                                 };
            }

            LastTargets[battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex)] = newTargets[0];

            return newTargets;
        }

        /// <summary>
        /// Set the Volatile status to use the move next turn.
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

            if (!battleManager.Statuses.HasStatus(FixationStatus, userType, userIndex))
                yield return battleManager.Statuses.AddStatus(FixationStatus,
                                                              FixationStatus.CalculateRandomCountdown(battleManager,
                                                                  userType,
                                                                  userIndex,
                                                                  userType,
                                                                  userIndex),
                                                              userType,
                                                              userIndex,
                                                              userType,
                                                              userIndex,
                                                              ignoresAbilities);
            else if (user.VolatileStatuses[FixationStatus] == 1)
            {
                yield return battleManager.Statuses.RemoveStatus(FixationStatus, user);

                yield return OnStatusRemoved(battleManager, user, userType, userIndex);
            }
        }

        /// <summary>
        /// Called when the fixations status is removed from the battler.
        /// </summary>
        protected virtual IEnumerator OnStatusRemoved(BattleManager battleManager,
                                                      Battler user,
                                                      BattlerType userType,
                                                      int userIndex)
        {
            LastTargets.Remove(user);

            if (GetConfusedOnEnd)
                yield return battleManager.Statuses.AddStatus(Confusion,
                                                              Confusion.CalculateRandomCountdown(battleManager,
                                                                  userType,
                                                                  userIndex,
                                                                  userType,
                                                                  userIndex),
                                                              user,
                                                              userType,
                                                              userIndex,
                                                              false);
        }

        /// <summary>
        /// Called when the battle ends to clean up any data the move may have stored.
        /// </summary>
        /// <param name="battler">Battler owner of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerable OnBattleEnded(Battler battler, BattleManager battleManager)
        {
            yield return base.OnBattleEnded(battler, battleManager);
            LastTargets.Clear();
        }
    }
}