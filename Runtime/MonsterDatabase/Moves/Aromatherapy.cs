using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move Aromatherapy.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Grass/Aromatherapy", fileName = "Aromatherapy")]
    public class Aromatherapy : Move
    {
        // TODO: Animation.

        /// <summary>
        /// Cure all status conditions.
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
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (List<Battler> roster in userType == BattlerType.Ally
                                                 ? battleManager.Rosters.AllyRosters
                                                 : battleManager.Rosters.EnemyRosters)
            {
                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (Battler target in roster)
                {
                    if (target.GetStatus() == null) continue;

                    if (!target.Substitute.SubstituteEnabled || target == user)
                        yield return battleManager.Statuses.RemoveStatus(target);
                }
            }
        }
    }
}