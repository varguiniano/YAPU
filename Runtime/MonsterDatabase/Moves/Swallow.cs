using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move Swallow.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/Swallow", fileName = "Swallow")]
    public class Swallow : Move
    {
        // TODO: Animation.

        /// <summary>
        /// Reference to the stockpile status.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllLayeredVolatileStatuses))]
        #endif
        [FoldoutGroup("Effect")]
        [SerializeField]
        private StockpileStatus Stockpile;

        /// <summary>
        /// Percentage to heal depending on the amount of layers.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializableDictionary<int, float> HealPercentagePerLayer;

        /// <summary>
        /// Execute the effect of the move.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="user"></param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="hitNumber">This is the number of hits already made in this move. It will be always 0 unless it's a multihit move.</param>
        /// <param name="expectedHits">Expected move hits.</param>
        /// <param name="externalPowerMultiplier">External multiplier applied to the power.</param>
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
            foreach ((BattlerType targetType, int targetIndex) in targets)
            {
                Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex);

                if (!battleManager.Statuses.HasStatus(Stockpile, targetType, targetIndex))
                {
                    yield return DialogManager.ShowDialogAndWait("Battle/Move/NoEffect",
                                                                 switchToNextAfterSeconds: 1.5f
                                                                   / battleManager.BattleSpeed);

                    finishedCallback.Invoke(false);
                    continue;
                }

                int stockpiledLayers = Stockpile.LayerCount[target];

                yield return battleManager.Statuses.RemoveStatus(Stockpile, targetType, targetIndex);

                int hpToHeal = Mathf.RoundToInt(HealPercentagePerLayer[stockpiledLayers]
                                              * target.GetStats(battleManager)[Stat.Hp]);

                int healedAmount = 0;

                yield return battleManager.BattlerHealth.ChangeLife(targetType,
                                                                    targetIndex,
                                                                    userType,
                                                                    userIndex,
                                                                    hpToHeal,
                                                                    this,
                                                                    finished: (amount, _) => healedAmount = amount);

                yield return DialogManager.ShowDialogAndWait("Battle/RecoverHP",
                                                             switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed,
                                                             localizableModifiers: false,
                                                             modifiers: new[]
                                                                        {
                                                                            target.GetNameOrNickName(localizer),
                                                                            healedAmount.ToString()
                                                                        });
            }
        }
    }
}