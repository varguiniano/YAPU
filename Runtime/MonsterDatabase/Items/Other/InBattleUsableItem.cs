using UnityEngine;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Other
{
    /// <summary>
    /// Item that can be used only in battle.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Other/InBattleUsableItem", fileName = "InBattleUsableItem")]
    public class InBattleUsableItem : OtherItem
    {
        /// <summary>
        /// Can't be used without a target.
        /// </summary>
        public override bool CanBeUsed => false;
        
        /// <summary>
        /// This type of items can't be registered.
        /// </summary>
        public override bool CanBeRegistered => false;

        /// <summary>
        /// Can't be used on a target.
        /// </summary>
        public override bool CanBeUsedOnTarget => false;

        /// <summary>
        /// Can't be used on a move.
        /// </summary>
        public override bool CanBeUsedOnTargetMove => false;

        /// <summary>
        /// Can be used without a target.
        /// </summary>
        public override bool CanBeUsedInBattle => true;

        /// <summary>
        /// Can't be used on a target.
        /// </summary>
        public override bool CanBeUsedInBattleOnTarget => false;

        /// <summary>
        /// Can't be used on a move.
        /// </summary>
        public override bool CanBeUsedInBattleOnTargetMove => false;
    }
}