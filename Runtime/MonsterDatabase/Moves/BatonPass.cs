using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move BatonPass.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/BatonPass", fileName = "BatonPass")]
    public class BatonPass : Move
    {
        /// <summary>
        /// Statuses that can be passed.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllVolatileStatuses))]
        #endif
        private List<VolatileStatus> PassableStatuses;

        /// <summary>
        /// Fail if only one mon left in the target roster.
        /// </summary>
        internal override bool WillMoveFail(BattleManager battleManager,
                                            ILocalizer localizer,
                                            BattlerType userType,
                                            int userIndex,
                                            BattlerType targetType,
                                            int targetIndex,
                                            bool ignoresAbilities) =>
            battleManager.Rosters.GetNumberNotFainted(targetType, targetIndex) == 1
         || base.WillMoveFail(battleManager,
                              localizer,
                              userType,
                              userIndex,
                              targetType,
                              targetIndex,
                              ignoresAbilities);

        /// <summary>
        /// Switch out and pass stuff.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="user"></param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="hitNumber">This is the number of hits already made in this move. It will be always 0 unless it's a multihit move.</param>
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
            SerializableDictionary<Stat, short> statStages = user.StatStage.ShallowClone();
            SerializableDictionary<BattleStat, short> battleStatStages = user.BattleStatStage.ShallowClone();
            byte criticalStage = user.CriticalStage;

            SubstituteData substituteData = user.Substitute;

            Dictionary<VolatileStatus, int> statusesToPass = new();

            foreach (KeyValuePair<VolatileStatus, int> candidate in user.VolatileStatuses.Where(candidate =>
                         PassableStatuses.Contains(candidate.Key)))
                statusesToPass[candidate.Key] = candidate.Value;

            Battler newMonster = null;

            yield return battleManager.BattleManagerBattlerSwitch.ForceSwitchBattler(userType,
                userIndex,
                userType,
                userIndex,
                this,
                null,
                false,
                ignoresAbilities,
                true,
                index =>
                {
                    newMonster = battleManager.Battlers.GetBattlerFromBattleIndex(userType, index);

                    foreach (KeyValuePair<VolatileStatus, int> statusSlot in statusesToPass)
                        statusSlot.Key.OnMonsterChanged(user, newMonster, battleManager);
                });

            yield return DialogManager.WaitForDialog;

            if (newMonster == null)
            {
                finishedCallback.Invoke(false);
                yield break;
            }

            newMonster.StatStage = statStages;
            newMonster.BattleStatStage = battleStatStages;
            newMonster.CriticalStage = criticalStage;
            newMonster.Substitute = substituteData;

            newMonster.VolatileStatuses = statusesToPass;

            finishedCallback.Invoke(true);
        }
    }
}