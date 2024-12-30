using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move StoredPower.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Psychic/StoredPower", fileName = "StoredPower")]
    public class StoredPower : DamageMove
    {
        /// <summary>
        /// Add power for each raised stat.
        /// </summary>
        public override int GetMovePowerInBattle(BattleManager battleManager,
                                                 Battler user,
                                                 Battler target,
                                                 bool ignoresAbilities,
                                                 int hitNumber = 0)
        {
            int basePower = base.GetMovePowerInBattle(battleManager, user, target, ignoresAbilities, hitNumber);
            int power = basePower;

            foreach (short value in user.StatStage.Values)
                if (value > 0)
                    power += basePower * value;

            foreach (short value in user.BattleStatStage.Values)
                if (value > 0)
                    power += basePower * value;

            return power;
        }
    }
}