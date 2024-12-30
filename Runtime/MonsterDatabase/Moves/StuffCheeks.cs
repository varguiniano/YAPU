using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Berries;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move Pluck.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/StuffCheeks", fileName = "StuffCheeks")]
    public class StuffCheeks : StageChangeMove
    {
        /// <summary>
        /// Fail if it doesn't have a berry.
        /// </summary>
        internal override bool WillMoveFail(BattleManager battleManager,
                                            ILocalizer localizer,
                                            BattlerType userType,
                                            int userIndex,
                                            BattlerType targetType,
                                            int targetIndex,
                                            bool ignoresAbilities) =>
            base.WillMoveFail(battleManager,
                              localizer,
                              userType,
                              userIndex,
                              targetType,
                              targetIndex,
                              ignoresAbilities)
         || battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex).HeldItem is not Berry;

        /// <summary>
        /// Consume its held berry to raise its Defense stat.
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
            foreach ((BattlerType Type, int Index) targetData in targets)
            {
                Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetData);

                if (target.HeldItem is not Berry berry) yield break;

                yield return battleManager.GetMonsterSprite(targetData).EatBerry(battleManager.BattleSpeed);

                yield return berry.UseOnTarget(user,
                                               battleManager,
                                               battleManager.YAPUSettings,
                                               battleManager.ExperienceLookupTable,
                                               battleManager.Localizer,
                                               _ =>
                                               {
                                               });

                user.HasEatenBerryThisBattle = true;

                yield return target.ConsumeItemInBattle(battleManager, "Dialogs/Battle/AteBerry");
            }

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
        }
    }
}