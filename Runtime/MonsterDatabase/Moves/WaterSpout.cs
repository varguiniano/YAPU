using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move WaterSpout.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Water/WaterSpout", fileName = "WaterSpout")]
    public class WaterSpout : DamageMove
    {
        /// <summary>
        /// Get the move's power.
        /// Formula: https://bulbapedia.bulbagarden.net/wiki/Water_Spout_(move)
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
            (int) Mathf.Clamp(150f * user.CurrentHP / user.GetStats(battleManager)[Stat.Hp], 1, float.MaxValue);

        // TODO: Animation.
    }
}