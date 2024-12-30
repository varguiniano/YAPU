using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move LashOut.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Dark/LashOut", fileName = "LashOut")]
    public class LashOut : DamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// Double power if the user has decreased stats this turn.
        /// </summary>
        public override int GetMovePowerInBattle(BattleManager battleManager,
                                                 Battler user,
                                                 Battler target,
                                                 bool ignoresAbilities,
                                                 int hitNumber = 0) =>
            base.GetMovePowerInBattle(battleManager, user, target, ignoresAbilities, hitNumber)
          * (user is
                {
                    DecreasedStatsThisTurn: true
                }
                    ? 2
                    : 1);
    }
}