using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Based class for moves in which the damage is based on the speed difference between the user and the target.
    /// Formula: https://bulbapedia.bulbagarden.net/wiki/Gyro_Ball_(move)
    /// </summary>
    public abstract class DamageBasedOnSpeedDifferenceDamageMove : DamageMove
    {
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
                                                 int hitNumber = 0)
        {
            uint userSpeed = user.GetStats(battleManager)[Stat.Speed];

            if (userSpeed < 1 || target == null) return 1;

            return (int) Mathf.Min(150, 25f * target.GetStats(battleManager)[Stat.Speed] / userSpeed + 1);
        }
    }
}