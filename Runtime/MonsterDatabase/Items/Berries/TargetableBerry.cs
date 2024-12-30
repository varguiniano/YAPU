using UnityEngine;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Berries
{
    /// <summary>
    /// Data class for a berry that can be used on a target.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Berries/TargetableBerry", fileName = "TargetableBerry")]
    public class TargetableBerry : Berry
    {
        /// <summary>
        /// Only used on targets.
        /// </summary>
        public override bool CanBeUsed => false;

        /// <summary>
        /// Can be used on targets.
        /// </summary>
        public override bool CanBeUsedOnTarget => true;
        
        /// <summary>
        /// Can't be used on a move.
        /// </summary>
        public override bool CanBeUsedOnTargetMove => false;

        /// <summary>
        /// Only used on targets.
        /// </summary>
        public override bool CanBeUsedInBattle => false;

        /// <summary>
        /// Can be used on targets.
        /// </summary>
        public override bool CanBeUsedInBattleOnTarget => true;
        
        /// <summary>
        /// Can't be used on a move.
        /// </summary>
        public override bool CanBeUsedInBattleOnTargetMove => false;
    }
}