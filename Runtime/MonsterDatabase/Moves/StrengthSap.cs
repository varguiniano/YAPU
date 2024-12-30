using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move StrengthSap.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Grass/StrengthSap", fileName = "StrengthSap")]
    public class StrengthSap : StageChangeMove
    {
        // TODO: Animation.

        /// <summary>
        /// Stat to drain from the target.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private Stat StatToDrain;

        /// <summary>
        /// Percentage of HP drain, based on the target's stat.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        [PropertyRange(0, 1)]
        private float HPDrain = 1f;

        /// <summary>
        /// Fail if the user can't heal.
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
         || !battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex).CanHeal(battleManager);

        /// <summary>
        /// Gain HP.
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
            Dictionary<Battler, int> statValues = new();

            foreach ((BattlerType targetType, int targetIndex) in targets)
            {
                Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex);

                statValues[target] = (int) target.GetStats(battleManager)[StatToDrain];
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

            foreach (KeyValuePair<Battler, int> pair in statValues)
            {
                int drain = (int) (pair.Value * HPDrain * user.CalculateDrainHPMultiplier(battleManager, pair.Key));

                drain =
                    (int) (drain * pair.Key.CalculateDrainerDrainHPMultiplier(battleManager, user, ignoresAbilities));

                yield return battleManager.BattlerHealth.ChangeLife(userType,
                                                                    userIndex,
                                                                    userType,
                                                                    userIndex,
                                                                    drain);

                battleManager.Animation.UpdatePanels();

                yield return DialogManager.ShowDialogAndWait("Battle/HealthDrained",
                                                             localizableModifiers: false,
                                                             modifiers: pair.Key.GetNameOrNickName(battleManager
                                                                .Localizer),
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);
            }
        }
    }
}