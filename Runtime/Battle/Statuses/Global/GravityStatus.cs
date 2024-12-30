using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Global
{
    /// <summary>
    /// Data class for the global status of Gravity.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Global/Gravity", fileName = "GravityStatus")]
    public class GravityStatus : GlobalStatus
    {
        /// <summary>
        /// Multiplier for the accuracy of all moves.
        /// </summary>
        [SerializeField]
        private float AccuracyMultiplier = 1;

        /// <summary>
        /// Does this status ground the monster?
        /// </summary>
        /// <param name="battler">Battler to check.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>True if this status forces the monster to be grounded.</returns>
        public override bool IsGrounded(Battler battler, BattleManager battleManager) => true;

        /// <summary>
        /// Get the multiplier to apply to the accuracy when using a move on a target.
        /// </summary>
        public override float GetMoveAccuracyMultiplierWhenUsed(BattleManager battleManager,
                                                                Battler user,
                                                                Battler target,
                                                                Move move) =>
            base.GetMoveAccuracyMultiplierWhenUsed(battleManager, user, target, move) * AccuracyMultiplier;
    }
}