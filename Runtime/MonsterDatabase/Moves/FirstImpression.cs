using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move FirstImpression.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Bug/FirstImpression", fileName = "FirstImpression")]
    public class FirstImpression : DamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// Fail if the monster didn't enter the battle this turn.
        /// </summary>
        public override bool WillMoveFail(BattleManager battleManager,
                                          ILocalizer localizer,
                                          BattlerType userType,
                                          int userIndex,
                                          ref List<(BattlerType Type, int Index)> targets,
                                          int hitNumber,
                                          int expectedHits,
                                          bool ignoresAbilities,
                                          out string customFailMessage)
        {
            customFailMessage = "";

            return !battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex).EnteredBattleThisTurn
                || base.WillMoveFail(battleManager,
                                     localizer,
                                     userType,
                                     userIndex,
                                     ref targets,
                                     hitNumber,
                                     expectedHits,
                                     ignoresAbilities,
                                     out customFailMessage);
        }
    }
}