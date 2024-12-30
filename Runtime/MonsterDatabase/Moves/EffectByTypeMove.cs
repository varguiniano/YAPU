using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for moves that have different effects depending on the type of their user.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/General/EffectByTypeMove", fileName = "EffectByTypeMove")]
    public class EffectByTypeMove : Move
    {
        /// <summary>
        /// Moves to execute per type.
        /// </summary>
        [FoldoutGroup("Moves")]
        [SerializeField]
        private SerializableDictionary<MonsterType, Move> MovesPerType;

        /// <summary>
        /// Default move to execute.
        /// </summary>
        [FoldoutGroup("Moves")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMoves))]
        #endif
        private Move DefaultMove;

        /// <summary>
        /// Get the type of this move out of battle.
        /// </summary>
        /// <param name="monster">Owner of the move.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <returns>The type of the move.</returns>
        public override MonsterType GetMoveType(MonsterInstance monster, YAPUSettings settings) =>
            monster == null
                ? base.GetMoveType(null, settings)
                : GetMoveForType(monster, settings).GetMoveType(monster, settings);

        /// <summary>
        /// Get the type of this move in battle.
        /// </summary>
        /// <param name="battler">Owner of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The type of the move.</returns>
        public override MonsterType GetMoveTypeInBattle(Battler battler, BattleManager battleManager) =>
            GetMoveForType(battler, battleManager.YAPUSettings).GetMoveTypeInBattle(battler, battleManager);

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
            GetMoveForType(user, battleManager.YAPUSettings).CalculateAccuracy(user, target, ignoresAbilities, battleManager);

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
            GetMoveForType(battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex),
                           battleManager.YAPUSettings)
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
                                                  bool ignoresAbilities) =>
            GetMoveForType(user, battleManager.YAPUSettings)
               .PlayAnimation(battleManager, speed, userType, userIndex, user, userPosition, targets, targetPositions, ignoresAbilities);

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
            yield return GetMoveForType(battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex),
                                        battleManager.YAPUSettings)
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
            GetMoveForType(moveOwner, battleManager.YAPUSettings)
               .OnActionAddedToOrder(moveOwner, lastAdded, ref order, battlers, actions, battleManager);

        /// <summary>
        /// Called when the battle ends to clean up any data the move may have stored.
        /// </summary>
        /// <param name="battler">Battler owner of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerable OnBattleEnded(Battler battler, BattleManager battleManager) =>
            GetMoveForType(battler, battleManager.YAPUSettings).OnBattleEnded(battler, battleManager);

        /// <summary>
        /// Get the move to use with this user's type.
        /// </summary>
        /// <param name="user">User to check.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <returns>The move to use.</returns>
        private Move GetMoveForType(MonsterInstance user, YAPUSettings settings)
        {
            foreach (KeyValuePair<MonsterType, Move> pair in MovesPerType)
                if (user.IsOfType(pair.Key, settings))
                    return pair.Value;

            return DefaultMove;
        }
    }
}