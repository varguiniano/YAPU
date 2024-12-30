using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move SuckerPunch.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Dark/SuckerPunch", fileName = "SuckerPunch")]
    public class SuckerPunch : DamageMove
    {
        /// <summary>
        /// Other moves that can be bypassed with Sucker Punch.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private List<Move> OtherBypassedMoves;

        /// <summary>
        /// Fail if the target hasn't selected a damage move or they moved before the user.
        /// </summary>
        internal override bool WillMoveFail(BattleManager battleManager,
                                            ILocalizer localizer,
                                            BattlerType userType,
                                            int userIndex,
                                            BattlerType targetType,
                                            int targetIndex,
                                            bool ignoresAbilities)
        {
            Battler user = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);
            Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex);

            List<Battler> order = battleManager.CurrentTurnActionOrder.ToList();

            // Fail if target moved before.
            if (order.IndexOf(target) < order.IndexOf(user)) return true;

            // Fail if target didn't even have an action (maybe they are fainted).
            if (!battleManager.CurrentTurnActions.TryGetValue(target, out BattleAction action)) return true;

            // Fail if target didn't select a damage move.
            if (action.ActionType != BattleAction.Type.Move) return true;

            int moveIndex = action.Parameters[0];

            if (moveIndex < 4
             && (target.CurrentMoves[moveIndex].Move is not DamageMove
              || !OtherBypassedMoves.Contains(target.CurrentMoves[moveIndex].Move)))
                return true;

            return base.WillMoveFail(battleManager,
                                     localizer,
                                     userType,
                                     userIndex,
                                     targetType,
                                     targetIndex,
                                     ignoresAbilities);
        }
    }
}