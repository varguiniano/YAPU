using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move HealBell.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/HealBell", fileName = "HealBell")]
    public class HealBell : Move
    {
        // TODO: Animation.

        /// <summary>
        /// Heal itself, allies and rest of own rooster.
        /// </summary>
        public override IEnumerator ExecuteEffect(BattleManager battleManager,
                                                  ILocalizer localizer,
                                                  BattlerType userType,
                                                  int userIndex,
                                                  Battler user,
                                                  List<(BattlerType Type, int Index)> targets,
                                                  int hitNumber,
                                                  int expectedHits,
                                                  float externalPowerMultiplier,
                                                  bool ignoresAbilities,
                                                  Action<bool> finishedCallback)
        {
            List<Battler> alliesFighting = battleManager.Battlers.GetBattlersFighting(userType);

            foreach (Battler ally in alliesFighting.Where(ally => ally.GetStatus() != null))
                yield return battleManager.Statuses.RemoveStatus(ally);

            (BattlerType Type, int RosterIndex, int BattlerIndex) rosterData =
                battleManager.Battlers.GetTypeAndRosterIndexOfBattler(user);

            foreach (Battler rosterMember in battleManager.Rosters.GetRoster(rosterData.Type, rosterData.RosterIndex)
                                                          .Where(rosterMember =>
                                                                     !alliesFighting.Contains(rosterMember)
                                                                  && rosterMember.GetStatus() != null))
                yield return battleManager.Statuses.RemoveStatus(rosterMember);
        }
    }
}