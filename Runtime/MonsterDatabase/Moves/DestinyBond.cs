using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.AI;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move DestinyBond.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Ghost/DestinyBond", fileName = "DestinyBond")]
    public class DestinyBond : SetVolatileStatusMove
    {
        // TODO: Animation.

        /// <summary>
        /// Fail if used consecutively.
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
            LastPerformedActionData lastAction = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex)
                                                              .LastPerformedAction;

            return base.WillMoveFail(battleManager,
                                     localizer,
                                     userType,
                                     userIndex,
                                     ref targets,
                                     hitNumber,
                                     expectedHits,
                                     ignoresAbilities,
                                     out customFailMessage)
                || (lastAction.LastAction == BattleAction.Type.Move
                 && lastAction.LastMove == this
                 && lastAction.LastMoveSuccessful);
        }
    }
}