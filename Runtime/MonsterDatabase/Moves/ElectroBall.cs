using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for ElectroBall.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Electric/ElectroBall", fileName = "ElectroBall")]
    public class ElectroBall : DamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// Get the move's power.
        /// https://bulbapedia.bulbagarden.net/wiki/Electro_Ball_(move)#Effect
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
            if (target == null) return base.GetMovePowerInBattle(battleManager, user, null, ignoresAbilities, hitNumber);

            return ((float)target.GetStats(battleManager)[Stat.Speed] / user.GetStats(battleManager)[Stat.Speed]) switch
            {
                >= 1 => 40,
                > .5f => 60,
                > .33f => 80,
                > .25f => 120,
                _ => 150
            };
        }
    }
}