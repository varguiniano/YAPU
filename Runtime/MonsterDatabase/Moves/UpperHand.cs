using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move UpperHand.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Fighting/UpperHand", fileName = "UpperHand")]
    public class UpperHand : StatusChanceDamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// Fail if the target is not using a priority move or they have already moved.
        /// </summary>
        internal override bool WillMoveFail(BattleManager battleManager,
                                            ILocalizer localizer,
                                            BattlerType userType,
                                            int userIndex,
                                            BattlerType targetType,
                                            int targetIndex,
                                            bool ignoresAbilities)
        {
            Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex);

            return base.WillMoveFail(battleManager, localizer, userType, userIndex, targetType, targetIndex, ignoresAbilities)
                || !battleManager.CurrentTurnActionOrder.Contains(target)
                || battleManager.CurrentTurnPriorityBrackets[target] < 1;
        }
    }
}