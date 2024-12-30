using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move NaturePower.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/NaturePower", fileName = "NaturePower")]
    public class NaturePower : Move
    {
        /// <summary>
        /// Get the type of this move in battle.
        /// </summary>
        /// <param name="battler">Owner of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The type of the move.</returns>
        public override MonsterType GetMoveTypeInBattle(Battler battler, BattleManager battleManager) =>
            GetMoveToUse(battleManager).GetMoveTypeInBattle(battler, battleManager);

        /// <summary>
        /// Calculate the precision of this move before performing it.
        /// </summary>
        /// <param name="user">The user of this move.</param>
        /// <param name="target">The target of this move.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The calculated precision.</returns>
        public override float CalculateAccuracy(Battler user,
                                                Battler target,
                                                bool ignoresAbilities,
                                                BattleManager battleManager) =>
            GetMoveToUse(battleManager).CalculateAccuracy(user, target, ignoresAbilities, battleManager);

        /// <summary>
        /// Get the pre stage accuracy of the move.
        /// </summary>
        /// <param name="user">The user of this move.</param>
        /// <param name="target">The target of this move.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The calculated precision.</returns>
        public override int GetPreStageAccuracy(Battler user,
                                                Battler target,
                                                bool ignoresAbilities,
                                                BattleManager battleManager) =>
            GetMoveToUse(battleManager).GetPreStageAccuracy(user, target, ignoresAbilities, battleManager);

        /// <summary>
        /// Check if this move has infinite accuracy in battle.
        /// </summary>
        /// <param name="user">The user of this move.</param>
        /// <param name="target">The target of this move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>True if it has.</returns>
        public override bool HasInfiniteAccuracy(Battler user, Battler target, BattleManager battleManager) =>
            GetMoveToUse(battleManager).HasInfiniteAccuracy(user, target, battleManager);

        /// <summary>
        /// Cast this to a damage move if it is.
        /// </summary>
        /// <param name="user">The user of this move.</param>
        /// <param name="target">The target of this move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The object as a damage move.</returns>
        public override DamageMove GetDamageMove(Battler user, Battler target, BattleManager battleManager) =>
            GetMoveToUse(battleManager).GetDamageMove(user, target, battleManager);

        /// <summary>
        /// Get the number of hits the move will do.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="ignoresAbilities"></param>
        public override int GetNumberOfHits(BattleManager battleManager,
                                            BattlerType userType,
                                            int userIndex,
                                            List<(BattlerType Type, int Index)> targets,
                                            bool ignoresAbilities) =>
            GetMoveToUse(battleManager).GetNumberOfHits(battleManager, userType, userIndex, targets, ignoresAbilities);

        /// <summary>
        /// Check if the move will fail reasons other than accuracy.
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
                                          out string customFailMessage) =>
            GetMoveToUse(battleManager)
               .WillMoveFail(battleManager,
                             localizer,
                             userType,
                             userIndex,
                             ref targets,
                             hitNumber,
                             expectedHits,
                             ignoresAbilities,
                             out customFailMessage);

        /// <summary>
        /// Play the move animation.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// <param name="userType">Type of battler the user is.</param>
        /// <param name="userIndex">User index.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="userPosition">Position of the user.</param>
        /// <param name="targets">Targets types and indexes.</param>
        /// <param name="targetPositions">Position of the targets.</param>
        /// <param name="ignoresAbilities"></param>
        public override IEnumerator PlayAnimation(BattleManager battleManager,
                                                  float speed,
                                                  BattlerType userType,
                                                  int userIndex,
                                                  Battler user,
                                                  Transform userPosition,
                                                  List<(BattlerType Type, int Index)> targets,
                                                  List<Transform> targetPositions,
                                                  bool ignoresAbilities)
        {
            yield return GetMoveToUse(battleManager)
               .PlayAnimation(battleManager, speed, userType, userIndex, user, userPosition, targets, targetPositions, ignoresAbilities);
        }

        /// <summary>
        /// Execute the effect of the move.
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
            yield return GetMoveToUse(battleManager)
               .ExecuteEffect(battleManager,
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
        /// Callback that can be used to alter the normal order of battle.
        /// </summary>
        /// <param name="moveOwner">Owner of the move.</param>
        /// <param name="lastAdded">Last added battler.</param>
        /// <param name="order">Current calculated order.</param>
        /// <param name="battlers">Battlers that will perform actions this turn.</param>
        /// <param name="actions">Actions that will be performed.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns> An int value.
        /// -1 -> Go immediately before the last action added.
        /// 0 -> Follow normal ordering.
        /// 1 -> Go immediately after the last action added.
        /// </returns>
        public override int OnActionAddedToOrder(Battler moveOwner,
                                                 Battler lastAdded,
                                                 ref Queue<Battler> order,
                                                 List<Battler> battlers,
                                                 SerializableDictionary<Battler, BattleAction> actions,
                                                 BattleManager battleManager) =>
            GetMoveToUse(battleManager)
               .OnActionAddedToOrder(moveOwner, lastAdded, ref order, battlers, actions, battleManager);

        /// <summary>
        /// Get the move to use.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>Move to use.</returns>
        private static Move GetMoveToUse(BattleManager battleManager) =>
            battleManager.Scenario.Terrain != null
                ? battleManager.Scenario.Terrain.NaturePowerMove
                : battleManager.Scenario.BattleScenario.NaturePowerMove;
    }
}