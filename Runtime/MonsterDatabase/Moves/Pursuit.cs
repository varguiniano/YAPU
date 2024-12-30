using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move Pursuit.
    /// Pursuit will always go before a monster switching out.
    /// It will also change the target to the monster switching out.
    /// Multiply the power and always hit if the target is switching out.
    /// https://bulbapedia.bulbagarden.net/wiki/Pursuit_(move)
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Dark/Pursuit", fileName = "Pursuit")]
    public class Pursuit : DamageMove
    {
        /// <summary>
        /// Multiplier to apply when the target is switching out.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private float SwitchPowerMultiplier = 2;

        /// <summary>
        /// Apart from switching out, moves that are also affected.
        /// This is mainly U-turn and the like.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private List<Move> AffectedMoves;

        /// <summary>
        /// Dictionary to keep a reference to the target overrides that happen when a monster wants to switch out.
        /// If the user is not on this dictionary, the target is not overriden, the power is not multiplied and the accuracy is not infinite.
        /// </summary>
        private readonly Dictionary<Battler, Battler> targetOverride = new();

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
                                                 BattleManager battleManager)
        {
            // Target has already been overriden.
            if (targetOverride.ContainsKey(moveOwner)) return 0;

            (BattlerType userType, int _) = battleManager.Battlers.GetTypeAndIndexOfBattler(moveOwner);

            (BattlerType lastAddedType, int _) = battleManager.Battlers.GetTypeAndIndexOfBattler(lastAdded);

            // Doesn't affect allies.
            if (userType == lastAddedType) return 0;

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (actions[lastAdded].ActionType)
            {
                // If the last added battler is switching out, override the target.
                case BattleAction.Type.Switch:
                    targetOverride.Add(moveOwner, lastAdded);
                    return -1;
                // If the last added battler is using a move that is affected by Pursuit, override the target.
                case BattleAction.Type.Move:
                {
                    int moveIndex = actions[lastAdded].Parameters[0];

                    if (moveIndex is >= 0 and <= 3 && AffectedMoves.Contains(lastAdded.CurrentMoves[moveIndex].Move))
                    {
                        targetOverride.Add(moveOwner, lastAdded);
                        return -1;
                    }

                    break;
                }
            }

            return 0;
        }

        /// <summary>
        /// Called when the final targets are about to be selected.
        /// This allows the move to reselect different targets on certain conditions.
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
            Battler user = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            if (!targetOverride.ContainsKey(user))
                return base.SelectFinalTargets(battleManager, userType, userIndex, targets);

            Battler finalTarget = targetOverride[user];

            // Change the target if it was overriden and is still fighting.
            return battleManager.Battlers.GetBattlersFighting().Contains(finalTarget)
                       ? new List<(BattlerType Type, int Index)> { battleManager.Battlers.GetTypeAndIndexOfBattler(finalTarget) }
                       : base.SelectFinalTargets(battleManager, userType, userIndex, targets);
        }

        /// <summary>
        /// Has infinite accuracy if the target overriden (and still in battle).
        /// </summary>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Originally selected target.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>True if it has.</returns>
        public override bool HasInfiniteAccuracy(Battler user, Battler target, BattleManager battleManager) =>
            targetOverride.ContainsKey(user)
         && battleManager.Battlers.GetBattlersFighting().Contains(targetOverride[user]);

        /// <summary>
        /// Multiply if target overriden (and still in battle).
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Originally selected target.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="hitNumber"></param>
        /// <returns>The move's power.</returns>
        public override int GetMovePowerInBattle(BattleManager battleManager,
                                                 Battler user,
                                                 Battler target,
                                                 bool ignoresAbilities,
                                                 int hitNumber = 0)
        {
            int basePower = base.GetMovePowerInBattle(battleManager, user, target, ignoresAbilities, hitNumber);

            if (!targetOverride.ContainsKey(user)) return basePower;

            Battler newTarget = targetOverride[user];

            return Mathf.RoundToInt(battleManager.Battlers.GetBattlersFighting().Contains(newTarget)
                                        ? SwitchPowerMultiplier
                                        : 1)
                 * basePower;
        }

        /// <summary>
        /// Called once after each turn after statuses have ticked.
        /// </summary>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator AfterTurnPostStatus(Battler battler, BattleManager battleManager)
        {
            targetOverride.Clear();

            yield return base.AfterTurnPostStatus(battler, battleManager);
        }
    }
}