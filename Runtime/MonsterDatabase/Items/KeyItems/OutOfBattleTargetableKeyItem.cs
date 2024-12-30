using UnityEngine;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.KeyItems
{
    /// <summary>
    /// Data class for a key item that can be used on a monster out of battle.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Key/OutOfBattleTargetable", fileName = "OutOfBattleTargetableKeyItem")]
    public class OutOfBattleTargetableKeyItem : KeyItem
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
        /// Can be used on a target.
        /// </summary>
        public override bool CanBeUsedOnTarget => true;

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