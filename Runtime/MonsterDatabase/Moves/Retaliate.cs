using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move Retaliate.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/Retaliate", fileName = "Retaliate")]
    public class Retaliate : DamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// Duplicate if an ally fainted last turn.
        /// </summary>
        public override int GetMovePowerInBattle(BattleManager battleManager,
                                                 Battler user,
                                                 Battler target,
                                                 bool ignoresAbilities,
                                                 int hitNumber = 0)
        {
            (BattlerType userType, int userRoster, int _) = battleManager.Battlers.GetTypeAndRosterIndexOfBattler(user);

            return base.GetMovePowerInBattle(battleManager, user, target, ignoresAbilities, hitNumber)
                 * (battleManager.Rosters.HadABattlerFaintLastTurn.Contains((userType, userRoster)) ? 2 : 1);
        }
    }
}