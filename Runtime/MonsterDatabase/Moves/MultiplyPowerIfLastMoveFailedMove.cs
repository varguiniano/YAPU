using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Base class for moves that multiply their power if the user failed the move last turn.
    /// </summary>
    public abstract class MultiplyPowerIfLastMoveFailedMove : DamageMove
    {
        /// <summary>
        /// Multiplier to apply when last move failed.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private float MultiplierWhenLastMoveFailed = 2;

        /// <summary>
        /// Get the move's power.
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
            (int)(base.GetMovePowerInBattle(battleManager, user, target, ignoresAbilities, hitNumber)
                * (!user.LastPerformedAction.LastMoveSuccessful
                && user.LastPerformedAction.LastAction == BattleAction.Type.Move
                       ? MultiplierWhenLastMoveFailed
                       : 1));
    }
}