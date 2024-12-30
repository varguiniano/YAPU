using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move BloodMoon.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/BloodMoon", fileName = "BloodMoon")]
    public class BloodMoon : DamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// Status that disables the move.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllVolatileStatuses))]
        #endif
        private VolatileStatus DisableStatus;

        /// <summary>
        /// Set the Volatile status to prevent the use of the move next turn.
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
            yield return base.ExecuteEffect(battleManager,
                                            localizer,
                                            userType,
                                            userIndex,
                                            user,
                                            targets,
                                            hitNumber,
                                            expectedHits,
                                            externalPowerMultiplier,
                                            ignoresAbilities,
                                            finishedCallback);

            yield return battleManager.Statuses.AddStatus(DisableStatus,
                                                          DisableStatus.CalculateRandomCountdown(battleManager,
                                                                   userType,
                                                                   userIndex,
                                                                   userType,
                                                                   userIndex),
                                                          userType,
                                                          userIndex,
                                                          userType,
                                                          userIndex,
                                                          ignoresAbilities,
                                                          this);
        }
    }
}