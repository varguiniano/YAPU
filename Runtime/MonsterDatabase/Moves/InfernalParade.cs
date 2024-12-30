using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for InfernalParade.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Ghost/InfernalParade", fileName = "InfernalParade")]
    public class InfernalParade : StatusChanceDamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// Double the power when the target has a status.
        /// </summary>
        public override int GetMovePowerInBattle(BattleManager battleManager,
                                                 Battler user,
                                                 Battler target,
                                                 bool ignoresAbilities,
                                                 int hitNumber = 0) =>
            base.GetMovePowerInBattle(battleManager, user, target, ignoresAbilities, hitNumber)
          * (target != null && target.GetStatus() != null ? 2 : 1);
    }
}