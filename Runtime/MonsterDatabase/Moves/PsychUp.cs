using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move PsychUp.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/PsychUp", fileName = "PsychUp")]
    public class PsychUp : Move
    {
        /// <summary>
        /// Statuses that this move copies.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllVolatileStatuses))]
        #endif
        private List<VolatileStatus> StatusesToCopy;

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
            foreach ((BattlerType targetType, int targetIndex) in targets)
            {
                Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex);

                foreach (KeyValuePair<Stat, short> statStage in target.StatStage)
                    user.StatStage[statStage.Key] = statStage.Value;

                foreach (KeyValuePair<BattleStat, short> statStage in target.BattleStatStage)
                    user.BattleStatStage[statStage.Key] = statStage.Value;

                user.CriticalStage = target.CriticalStage;

                foreach (KeyValuePair<VolatileStatus, int> statusSlot in target.VolatileStatuses.Where(statusSlot =>
                             StatusesToCopy.Contains(statusSlot.Key)
                          && !user.HasVolatileStatus(statusSlot.Key)))
                    user.VolatileStatuses[statusSlot.Key] = statusSlot.Value;

                yield return DialogManager.ShowDialogAndWait("Dialogs/Moves/PsychUp/Copied",
                                                             switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed,
                                                             localizableModifiers: false,
                                                             modifiers: new[]
                                                                        {
                                                                            user.GetNameOrNickName(localizer),
                                                                            target.GetNameOrNickName(localizer)
                                                                        });
            }

            finishedCallback.Invoke(true);
        }
    }
}