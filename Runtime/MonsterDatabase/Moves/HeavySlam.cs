using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for HeavySlam.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Electric/HeavySlam", fileName = "HeavySlam")]
    public class HeavySlam : DamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// Get the move's power.
        /// https://bulbapedia.bulbagarden.net/wiki/Heavy_Slam_(move)
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
            if (target == null)
                return base.GetMovePowerInBattle(battleManager, user, null, ignoresAbilities, hitNumber);

            return (target.GetWeight(battleManager, ignoresAbilities) / user.GetWeight(battleManager, false)) switch
            {
                >= .5f => 40,
                > .33f => 60,
                > .25f => 80,
                > .2f => 100,
                _ => 120
            };
        }
    }
}