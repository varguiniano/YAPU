using UnityEngine;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.BattleItems
{
    /// <summary>
    /// Data class for a battle item that can be used in battle without a target.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/BattleItem/NonTargetableBattleItem", fileName = "NonTargetableBattleItem")]
    public class NonTargetableBattleItem : BattleItem
    {
        /// <summary>
        /// Needs a target.
        /// </summary>
        public override bool CanBeUsedInBattle => true;

        /// <summary>
        /// Can be used in battle on a target.
        /// </summary>
        public override bool CanBeUsedInBattleOnTarget => false;

        /// <summary>
        /// Can't be used on a move.
        /// </summary>
        public override bool CanBeUsedInBattleOnTargetMove => false;
    }
}