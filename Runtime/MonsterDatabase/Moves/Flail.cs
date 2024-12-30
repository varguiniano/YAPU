using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move Flail.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/Flail", fileName = "Flail")]
    public class Flail : DamageMove
    {
        /// <summary>
        /// Get the move's power.
        /// Lookup table based on: https://bulbapedia.bulbagarden.net/wiki/Flail_(move)#Generation_II
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
            ((float) user.CurrentHP / user.GetStats(battleManager)[Stat.Hp]) switch
            {
                >= .6875f => 20,
                >= .3542f => 40,
                >= .2083f => 80,
                >= .1042f => 100,
                >= .0417f => 150,
                _ => 200
            };
    }
}