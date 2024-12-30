using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move Payback.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Dark/Payback", fileName = "Payback")]
    public class Payback : DamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// Duplicate the move power if the target went first.
        /// </summary>
        public override int GetMovePowerInBattle(BattleManager battleManager,
                                                 Battler user,
                                                 Battler target,
                                                 bool ignoresAbilities,
                                                 int hitNumber = 0)
        {
            if (target == null
             || battleManager.CurrentTurnActionOrder == null
             || !battleManager.CurrentTurnActionOrder.Contains(target))
                return base.GetMovePowerInBattle(battleManager, user, target, ignoresAbilities, hitNumber);

            List<Battler> order = battleManager.CurrentTurnActionOrder.ToList();

            return (order.IndexOf(target) < order.IndexOf(user)
                        ? 2
                        : 1)
                 * base.GetMovePowerInBattle(battleManager, user, target, ignoresAbilities, hitNumber);
        }
    }
}