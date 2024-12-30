﻿using UnityEngine;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Other
{
    /// <summary>
    /// Data class for an other item that can be used on a monster out of battle.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Other/OutOfBattleMoveTargetable", fileName = "OutOfBattleMoveTargetable")]
    public class OutOfBattleMoveTargetableItem : OtherItem
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
        /// Can be used on a move.
        /// </summary>
        public override bool CanBeUsedOnTargetMove => true;

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