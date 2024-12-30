using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move PsychoShift.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Psychic/PsychoShift", fileName = "PsychoShift")]
    public class PsychoShift : Move
    {
        /// <summary>
        /// Can the status be added?
        /// </summary>
        internal override bool WillMoveFail(BattleManager battleManager,
                                            ILocalizer localizer,
                                            BattlerType userType,
                                            int userIndex,
                                            BattlerType targetType,
                                            int targetIndex,
                                            bool ignoresAbilities)
        {
            Battler user = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            return base.WillMoveFail(battleManager,
                                     localizer,
                                     userType,
                                     userIndex,
                                     targetType,
                                     targetIndex,
                                     ignoresAbilities)
                || user.GetStatus() == null
                || battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex).GetStatus() != null;
        }

        /// <summary>
        /// Transfer the status.
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
            if (WillMoveFail(battleManager,
                             localizer,
                             userType,
                             userIndex,
                             targets[0].Type,
                             targets[0].Index,
                             ignoresAbilities))
            {
                yield return DialogManager.ShowDialogAndWait("Battle/Move/Failed",
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);

                finishedCallback.Invoke(false);
                yield break;
            }

            foreach ((BattlerType Type, int Index) targetData in targets)
            {
                Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetData);

                bool canAdd = true;

                yield return target.CanAddStatus(user.GetStatus(),
                                                 battleManager,
                                                 userType,
                                                 userIndex,
                                                 ignoresAbilities,
                                                 true,
                                                 add => canAdd &= add);

                if (!canAdd)
                {
                    yield return DialogManager.ShowDialogAndWait("Battle/Move/Failed",
                                                                 switchToNextAfterSeconds: 1.5f
                                                                   / battleManager.BattleSpeed);

                    finishedCallback.Invoke(false);
                    yield break;
                }

                yield return battleManager.Statuses.AddStatus(user.GetStatus(),
                                                              target,
                                                              userType,
                                                              userIndex,
                                                              ignoreAbilities: ignoresAbilities);

                yield return battleManager.Statuses.RemoveStatus(user);
            }

            finishedCallback.Invoke(true);
        }
    }
}