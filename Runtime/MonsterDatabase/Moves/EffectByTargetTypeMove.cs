using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for moves that have different effects depending on the battler type of their user.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/General/EffectByTargetTypeMove",
                     fileName = "EffectByTargetTypeMove")]
    public class EffectByTargetTypeMove : Move
    {
        /// <summary>
        /// Move to use when targeting an ally.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMoves))]
        #endif
        private Move AllyTargetingMove;

        /// <summary>
        /// Move to use when targeting an opponent.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMoves))]
        #endif
        private Move OpponentTargetingMove;

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
            GetMoveForType(user, target, battleManager).CalculateAccuracy(user, target, ignoresAbilities, battleManager);

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
            GetMoveForType(userType, targetType)
               .WillMoveFail(battleManager, localizer, userType, userIndex, targetType, targetIndex, ignoresAbilities);

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
            List<(BattlerType Type, int Index)> allyTargets = targets.Where(tuple => tuple.Item1 == userType).ToList();

            List<(BattlerType Type, int Index)> opponentTargets =
                targets.Where(tuple => tuple.Item1 != userType).ToList();

            if (allyTargets.Count > 0)
                yield return GetMoveForType(userType, allyTargets[0].Item1)
                   .PlayAnimation(battleManager,
                                  speed,
                                  userType,
                                  userIndex,
                                  user,
                                  userPosition,
                                  allyTargets,
                                  targetPositions,
                                  ignoresAbilities);

            if (opponentTargets.Count > 0)
                yield return GetMoveForType(userType, opponentTargets[0].Item1)
                   .PlayAnimation(battleManager,
                                  speed,
                                  userType,
                                  userIndex,
                                  user,
                                  userPosition,
                                  opponentTargets,
                                  targetPositions,
                                  ignoresAbilities);
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
            List<(BattlerType Type, int Index)> allyTargets = targets.Where(tuple => tuple.Item1 == userType).ToList();

            List<(BattlerType Type, int Index)> opponentTargets =
                targets.Where(tuple => tuple.Item1 != userType).ToList();

            if (allyTargets.Count > 0)
                yield return GetMoveForType(userType, allyTargets[0].Item1)
                   .ExecuteEffect(battleManager,
                                  localizer,
                                  userType,
                                  userIndex,
                                  user,
                                  allyTargets,
                                  hitNumber,
                                  expectedHits,
                                  externalPowerMultiplier,
                                  ignoresAbilities,
                                  finishedCallback);

            if (opponentTargets.Count > 0)
                yield return GetMoveForType(userType, opponentTargets[0].Item1)
                   .ExecuteEffect(battleManager,
                                  localizer,
                                  userType,
                                  userIndex,
                                  user,
                                  opponentTargets,
                                  hitNumber,
                                  expectedHits,
                                  externalPowerMultiplier,
                                  ignoresAbilities,
                                  finishedCallback);
        }

        /// <summary>
        /// Get the move to use based on if they are allies or opponents.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="target">Their target.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The move to use.</returns>
        private Move GetMoveForType(Battler user, Battler target, BattleManager battleManager) =>
            GetMoveForType(battleManager.Battlers.GetTypeAndIndexOfBattler(user).Type,
                           battleManager.Battlers.GetTypeAndIndexOfBattler(target).Type);

        /// <summary>
        /// Get the move to use based on if they are allies or opponents.
        /// </summary>
        /// <param name="userType">Type of the user.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <returns>The move to use.</returns>
        private Move GetMoveForType(BattlerType userType, BattlerType targetType) =>
            userType == targetType ? AllyTargetingMove : OpponentTargetingMove;
    }
}