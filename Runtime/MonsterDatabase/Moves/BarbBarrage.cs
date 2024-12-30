using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move BarbBarrage.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Poison/BarbBarrage", fileName = "BarbBarrage")]
    public class BarbBarrage : StatusChanceDamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// Get the move's power.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="hitNumber"></param>
        /// <returns>The move's power.</returns>
        public override int GetMovePowerInBattle(BattleManager battleManager,
                                                 Battler user,
                                                 Battler target,
                                                 bool ignoresAbilities,
                                                 int hitNumber = 0) =>
            (target != null && target.GetStatus() == StatusChance.First().Key ? 2 : 1)
          * base.GetMovePowerInBattle(battleManager, user, target, ignoresAbilities, hitNumber);
    }
}