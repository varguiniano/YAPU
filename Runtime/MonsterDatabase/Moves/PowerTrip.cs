using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move tackle.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Dark/PowerTrip", fileName = "PowerTrip")]
    public class PowerTrip : DamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// More power for each stage.
        /// </summary>
        /// <param name="battleManager"></param>
        /// <param name="user"></param>
        /// <param name="target"></param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="hitNumber"></param>
        /// <returns></returns>
        public override int GetMovePowerInBattle(BattleManager battleManager,
                                                 Battler user,
                                                 Battler target,
                                                 bool ignoresAbilities,
                                                 int hitNumber = 0)
        {
            int power = base.GetMovePowerInBattle(battleManager, user, target, ignoresAbilities, hitNumber);

            if (user == null) return power;

            int multiplier = 1;

            foreach (KeyValuePair<Stat, short> pair in user.StatStage)
                if (pair.Value > 0)
                    multiplier += pair.Value;

            foreach (KeyValuePair<BattleStat, short> pair in user.BattleStatStage)
                if (pair.Value > 0)
                    multiplier += pair.Value;

            return power * multiplier;
        }
    }
}