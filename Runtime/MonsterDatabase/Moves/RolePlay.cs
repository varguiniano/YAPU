using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the RolePlay move.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Psychic/RolePlay", fileName = "RolePlay")]
    public class RolePlay : SetVolatileStatusMove
    {
        // TODO: Animation.

        /// <summary>
        /// Fail if user has one of the immune abilities.
        /// </summary>
        public override bool WillMoveFail(BattleManager battleManager,
                                          ILocalizer localizer,
                                          BattlerType userType,
                                          int userIndex,
                                          ref List<(BattlerType Type, int Index)> targets,
                                          int hitNumber,
                                          int expectedHits,
                                          bool ignoresAbilities,
                                          out string customFailMessage) =>
            base.WillMoveFail(battleManager,
                              localizer,
                              userType,
                              userIndex,
                              ref targets,
                              hitNumber,
                              expectedHits,
                              ignoresAbilities,
                              out customFailMessage)
         || ImmuneAbilities.Contains(battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex)
                                                  .GetAbility());

        /// <summary>
        /// Pass the ability of the target to the status.
        /// </summary>
        protected override object[] PrepareExtraData(BattleManager battleManager,
                                                     BattlerType userType,
                                                     int userIndex,
                                                     BattlerType targetType,
                                                     int targetIndex) =>
            new object[] {battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex).GetAbility()};
    }
}