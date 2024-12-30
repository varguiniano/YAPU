using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing a move that changes stages.
    /// </summary>
    public class StageChangeMove : Move
    {
        /// <summary>
        /// Does this move bypass substitute?
        /// </summary>
        [FoldoutGroup("Base Stats")]
        [SerializeField]
        private bool BypassesSubstitute;

        /// <summary>
        /// Does this move change stats?
        /// </summary>
        [FoldoutGroup("Stats")]
        [SerializeField]
        private bool ChangeStats;

        /// <summary>
        /// Stat changes it produces.
        /// </summary>
        [FoldoutGroup("Stats")]
        [SerializeField]
        [ShowIf(nameof(ChangeStats))]
        private SerializableDictionary<Stat, short> StatChanges;

        /// <summary>
        /// Does this move change battle stats?
        /// </summary>
        [FoldoutGroup("Battle Stats")]
        [SerializeField]
        private bool ChangeBattleStats;

        /// <summary>
        /// Battle stat changes it produces.
        /// </summary>
        [FoldoutGroup("Battle Stats")]
        [SerializeField]
        [ShowIf(nameof(ChangeBattleStats))]
        private SerializableDictionary<BattleStat, short> BattleStatChanges;

        /// <summary>
        /// Does this move change the critical stage?
        /// </summary>
        [FoldoutGroup("Critical")]
        [SerializeField]
        private bool ChangeCritical;

        /// <summary>
        /// Critical change it produces.
        /// </summary>
        [FoldoutGroup("Critical")]
        [SerializeField]
        [ShowIf(nameof(ChangeCritical))]
        private short CriticalChange;

        /// <summary>
        /// Change the stages.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of the move user.</param>
        /// <param name="userIndex">In battle index of the user.</param>
        /// <param name="user"></param>
        /// <param name="targets">Move targets.</param>
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
            if (ChangeStats)
                foreach ((BattlerType targetType, int targetIndex) in targets)
                {
                    foreach ((Stat stat, short value) in StatChanges)
                        yield return battleManager.BattlerStats.ChangeStatStage(targetType,
                                                                                    targetIndex,
                                                                                    stat,
                                                                                    (short) (value
                                                                                              * GetStageChangeMultiplier(battleManager,
                                                                                                    userType,
                                                                                                    userIndex,
                                                                                                    targetType,
                                                                                                    targetIndex,
                                                                                                    targets)),
                                                                                    userType,
                                                                                    userIndex,
                                                                                    bypassSubstitute:
                                                                                    BypassesSubstitute,
                                                                                    ignoreAbilities: ignoresAbilities);
                }

            if (ChangeBattleStats)
                foreach ((BattlerType targetType, int targetIndex) in targets)
                {
                    foreach ((BattleStat stat, short value) in BattleStatChanges)
                        yield return battleManager.BattlerStats.ChangeStatStage(targetType,
                                                                                    targetIndex,
                                                                                    stat,
                                                                                    (short) (value
                                                                                              * GetStageChangeMultiplier(battleManager,
                                                                                                    userType,
                                                                                                    userIndex,
                                                                                                    targetType,
                                                                                                    targetIndex,
                                                                                                    targets)),
                                                                                    userType,
                                                                                    userIndex,
                                                                                    ignoreAbilities: ignoresAbilities);
                }

            if (ChangeCritical)
                foreach ((BattlerType targetType, int targetIndex) in targets)
                    yield return battleManager.BattlerStats.ChangeCriticalStage(targetType,
                                                                                    targetIndex,
                                                                                    (short) (CriticalChange
                                                                                              * GetStageChangeMultiplier(battleManager,
                                                                                                    userType,
                                                                                                    userIndex,
                                                                                                    targetType,
                                                                                                    targetIndex,
                                                                                                    targets)),
                                                                                    userType,
                                                                                    userIndex);

            finishedCallback.Invoke(true);
        }

        /// <summary>
        /// Get a multiplier for the stage set in the data.
        /// Useful for inheritors to apply modifications.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">Type of the move user.</param>
        /// <param name="userIndex">In battle index of the user.</param>
        /// <param name="targetType">Current target.</param>
        /// <param name="targetIndex">Current target,</param>
        /// <param name="targets">Move targets.</param>
        /// <returns>The multiplier to apply.</returns>
        protected virtual float GetStageChangeMultiplier(BattleManager battleManager,
                                                         BattlerType userType,
                                                         int userIndex,
                                                         BattlerType targetType,
                                                         int targetIndex,
                                                         List<(BattlerType Type, int Index)> targets) =>
            1;
    }
}