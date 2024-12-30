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
    /// Class representing the move Recycle.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/Recycle", fileName = "Recycle")]
    public class Recycle : Move
    {
        /// <summary>
        /// Fail if there is no item to restore.
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
            Battler user = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            return base.WillMoveFail(battleManager,
                                     localizer,
                                     userType,
                                     userIndex,
                                     ref targets,
                                     hitNumber,
                                     expectedHits,
                                     ignoresAbilities,
                                     out customFailMessage)
                || !user.ConsumedItemData.HasConsumedItem
                || !user.ConsumedItemData.CanBeRecycled;
        }

        /// <summary>
        /// Recycle the user's consumed item.
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
            user.HeldItem = user.ConsumedItemData.ConsumedItem;

            user.ConsumedItemData = new ConsumedItemData();

            yield return DialogManager.ShowDialogAndWait("Moves/Recycle/Effect",
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed,
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        user.GetLocalizedName(localizer),
                                                                        user.HeldItem.GetLocalizedName(localizer)
                                                                    });

            yield return user.HeldItem.OnItemReceivedInBattle(user, battleManager);
        }
    }
}