using UnityEngine;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Other
{
    /// <summary>
    /// Item that can be used outside of battle.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Other/OutOfBattleUsableItem", fileName = "OutOfBattleUsableItem")]
    public class OutOfBattleUsableItem : OtherItem
    {
        /// <summary>
        /// Can be used without a target.
        /// </summary>
        public override bool CanBeUsed => true;
        
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
        /// Can't be used without a target.
        /// </summary>
        public override bool CanBeUsedInBattle => false;

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