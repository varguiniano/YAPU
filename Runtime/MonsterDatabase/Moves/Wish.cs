using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for Wish.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/Wish", fileName = "Wish")]
    public class Wish : SetSideStatusMove
    {
        // TODO: Animation.

        /// <summary>
        /// Fail if can't heal.
        /// </summary>
        internal override bool WillMoveFail(BattleManager battleManager,
                                            ILocalizer localizer,
                                            BattlerType userType,
                                            int userIndex,
                                            BattlerType targetType,
                                            int targetIndex,
                                            bool ignoresAbilities) =>
            base.WillMoveFail(battleManager, localizer, userType, userIndex, targetType, targetIndex, ignoresAbilities)
         || !battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex).CanHeal(battleManager);

        /// <summary>
        /// Prepare the extra data for the volatile status.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="targetIndex">Index of the target.</param>
        protected override object[] PrepareExtraData(BattleManager battleManager,
                                                     BattlerType userType,
                                                     int userIndex,
                                                     BattlerType targetType,
                                                     int targetIndex) =>
            new object[] {targetType, targetIndex};
    }
}