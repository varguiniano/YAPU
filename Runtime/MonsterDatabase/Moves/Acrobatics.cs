using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move Acrobatics.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Flying/Acrobatics", fileName = "Acrobatics")]
    public class Acrobatics : DamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// Double the power if it's not holding an item.
        /// </summary>
        public override int GetMovePowerInBattle(BattleManager battleManager,
                                                 Battler user,
                                                 Battler target,
                                                 bool ignoresAbilities,
                                                 int hitNumber = 0) =>
            (user.HeldItem != null ? 1 : 2) * base.GetMovePowerInBattle(battleManager, user, target, ignoresAbilities, hitNumber);
    }
}