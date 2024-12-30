using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move TemperFlare.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Fire/TemperFlare", fileName = "TemperFlare")]
    public class TemperFlare : DamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// Double if target missed last move.
        /// </summary>
        public override int GetMovePowerInBattle(BattleManager battleManager,
                                                 Battler user,
                                                 Battler target,
                                                 bool ignoresAbilities,
                                                 int hitNumber = 0) =>
            base.GetMovePowerInBattle(battleManager, user, target, ignoresAbilities, hitNumber)
          * (target is {LastPerformedAction: {LastAction: BattleAction.Type.Move, LastMoveSuccessful: false}}
                 ? 2
                 : 1);
    }
}