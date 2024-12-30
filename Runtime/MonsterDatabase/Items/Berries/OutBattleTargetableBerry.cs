using UnityEngine;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Berries
{
    /// <summary>
    /// Data class for a berry that can be used on a target, but only out of battle.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Berries/OutBattleTargetableBerry", fileName = "OutBattleTargetableBerry")]
    public class OutBattleTargetableBerry : Berry
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
        /// Only used out of battle.
        /// </summary>
        public override bool CanBeUsedInBattleOnTarget => false;
        
        /// <summary>
        /// Can't be used on a move.
        /// </summary>
        public override bool CanBeUsedInBattleOnTargetMove => false;
    }
}