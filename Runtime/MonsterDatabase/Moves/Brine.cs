using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move Brine.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Water/Brine", fileName = "Brine")]
    public class Brine : DamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// Double if target is at or bellow 50%.
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
            (target != null && (float) target.CurrentHP / target.GetStats(battleManager)[Stat.Hp] <= .5f ? 2 : 1)
          * base.GetMovePowerInBattle(battleManager, user, target, ignoresAbilities, hitNumber);
    }
}