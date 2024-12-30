using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move HardPress.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Steel/HardPress", fileName = "HardPress")]
    public class HardPress : DamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// Formula: https://bulbapedia.bulbagarden.net/wiki/Hard_Press_(move)
        /// </summary>
        public override int GetMovePowerInBattle(BattleManager battleManager,
                                                 Battler user,
                                                 Battler target,
                                                 bool ignoresAbilities,
                                                 int hitNumber = 0) =>
            target == null
                ? base.GetMovePowerInBattle(battleManager, user, null, ignoresAbilities, hitNumber)
                : Mathf.RoundToInt(100f * target.CurrentHP / target.GetStats(battleManager)[Stat.Hp]);
    }
}