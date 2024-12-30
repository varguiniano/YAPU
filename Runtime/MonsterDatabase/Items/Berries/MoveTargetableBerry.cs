using UnityEngine;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Berries
{
    /// <summary>
    /// Data class for a berry that can be used on a target move.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Berries/MoveTargetableBerry", fileName = "MoveTargetableBerry")]
    public class MoveTargetableBerry : Berry
    {
        /// <summary>
        /// Only used on moves.
        /// </summary>
        public override bool CanBeUsed => false;

        /// <summary>
        /// Only used on moves.
        /// </summary>
        public override bool CanBeUsedOnTarget => false;

        /// <summary>
        /// Only used on moves.
        /// </summary>
        public override bool CanBeUsedOnTargetMove => true;

        /// <summary>
        /// Only used on moves.
        /// </summary>
        public override bool CanBeUsedInBattle => false;

        /// <summary>
        /// Only used on moves.
        /// </summary>
        public override bool CanBeUsedInBattleOnTarget => false;

        /// <summary>
        /// Only used on moves.
        /// </summary>
        public override bool CanBeUsedInBattleOnTargetMove => true;
    }
}