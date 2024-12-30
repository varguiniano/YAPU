using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move LastResort.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/LastResort", fileName = "LastResort")]
    public class LastResort : DamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// Extra failing conditions.
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

            // If it only knows last resort, fail.
            if (user.CurrentMoves.Count(slot => slot.Move != null) == 1) return true;

            // If it doesn't know last resort, fail.
            if (user.CurrentMoves.All(slot => slot.Move != this)) return true;

            // If it hasn't used all of its other moves, fail.
            foreach (Move move in user.CurrentMoves.Where(slot => slot.Move != this).Select(slot => slot.Move))
            {
                if (move == null) continue;
                if (!user.MovesUsedSinceEnteringBattle.Contains(move)) return true;
            }

            return base.WillMoveFail(battleManager, localizer, userType, userIndex, targetType, targetIndex, ignoresAbilities);
        }
    }
}