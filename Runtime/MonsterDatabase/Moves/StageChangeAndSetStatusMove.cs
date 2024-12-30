using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Base class for moves that both change a stage and set a status.
    /// Copied from the SetStatusMove class.
    /// </summary>
    public class StageChangeAndSetStatusMove : StageChangeMove
    {
        /// <summary>
        /// Status to set.
        /// </summary>
        [FoldoutGroup("Status")]
        [SerializeField]
        protected Status Status;

        /// <summary>
        /// Set a volatile status on the targets.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="user"></param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="hitNumber"></param>
        /// <param name="expectedHits"></param>
        /// <param name="externalPowerMultiplier"></param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="finishedCallback">Callback stating if the move successfully executed.</param>
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

            if (Status == null) yield break;

            foreach ((BattlerType targetType, int targetIndex) in targets)
            {
                Battler target = battleManager.Battlers
                                              .GetBattlerFromBattleIndex(targetType,
                                                                         targetIndex);

                target.GetEffectivenessOfMove(user,
                                              this,
                                              ignoresAbilities,
                                              battleManager,
                                              true,
                                              out float effectiveness);

                if (effectiveness == 0
                 || WillMoveFail(battleManager,
                                 localizer,
                                 userType,
                                 userIndex,
                                 targetType,
                                 targetIndex,
                                 ignoresAbilities))
                {
                    finishedCallback.Invoke(false);

                    yield break;
                }

                yield return battleManager.Statuses.AddStatus(Status,
                                                              target,
                                                              userType,
                                                              userIndex,
                                                              ignoreAbilities: ignoresAbilities);
            }

            finishedCallback.Invoke(true);
        }
    }
}