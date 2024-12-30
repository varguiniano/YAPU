using UnityEngine;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Medicines
{
    /// <summary>
    /// Data class for a medicine item that can be used on a move.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Medicine/MoveTargetableMedicine", fileName = "MoveTargetableMedicine")]
    public class MoveTargetableMedicine : Medicine
    {
        /// <summary>
        /// Only used on moves.
        /// </summary>
        public override bool CanBeUsed => false;
        
        /// <summary>
        /// This type of items can't be registered.
        /// </summary>
        public override bool CanBeRegistered => false;

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