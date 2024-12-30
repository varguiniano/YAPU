using UnityEngine;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.BattleItems
{
    /// <summary>
    /// Data class for a battle item that can be used on a battler.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/BattleItem/TargetableBattleItem", fileName = "TargetableBattleItem")]
    public class TargetableBattleItem : BattleItem
    {
        /// <summary>
        /// Needs a target.
        /// </summary>
        public override bool CanBeUsedInBattle => false;

        /// <summary>
        /// Can be used in battle on a target.
        /// </summary>
        public override bool CanBeUsedInBattleOnTarget => true;
        
        /// <summary>
        /// Can't be used on a move.
        /// </summary>
        public override bool CanBeUsedInBattleOnTargetMove => false;
    }
}