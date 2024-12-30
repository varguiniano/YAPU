using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for Poltergeist.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Ghost/Poltergeist", fileName = "Poltergeist")]
    public class Poltergeist : DamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// Fail if target doesn't have a held item.
        /// </summary>
        internal override bool WillMoveFail(BattleManager battleManager,
                                            ILocalizer localizer,
                                            BattlerType userType,
                                            int userIndex,
                                            BattlerType targetType,
                                            int targetIndex,
                                            bool ignoresAbilities) =>
            base.WillMoveFail(battleManager, localizer, userType, userIndex, targetType, targetIndex, ignoresAbilities)
         || battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex).HeldItem == null;
    }
}