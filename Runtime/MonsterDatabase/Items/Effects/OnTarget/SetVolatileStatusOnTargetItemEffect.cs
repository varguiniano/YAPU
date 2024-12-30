using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Experience;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.OnTarget
{
    /// <summary>
    /// Data class for an item effect that sets a status on the target.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/OnTarget/SetVolatileStatusOnTargetItemEffect",
                     fileName = "SetVolatileStatusOnTargetItemEffect")]
    public class SetVolatileStatusOnTargetItemEffect : UseOnTargetItemEffect
    {
        /// <summary>
        /// Status to be healed by this effect.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllVolatileStatuses))]
        #endif
        [SerializeField]
        private VolatileStatus Status;

        /// <summary>
        /// Should the status be forever?
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private bool InfiniteDuration;

        /// <summary>
        /// Set the status.
        /// </summary>
        /// <param name="item">Reference to the used item.</param>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="experienceLookupTable">Reference to the experience look up table.</param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        /// <param name="wasFlung">Was the item flung to this battler?</param>
        public override IEnumerator UseOnBattler(Item item,
                                                 Battler battler,
                                                 BattleManager battleManager,
                                                 YAPUSettings settings,
                                                 ExperienceLookupTable experienceLookupTable,
                                                 ILocalizer localizer,
                                                 Action<bool> finished,
                                                 bool wasFlung = false)
        {
            (BattlerType userType, int userIndex) =
                battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            if (battleManager.Statuses.HasStatus(Status, userType, userIndex)) yield break;

            yield return battleManager.Statuses.AddStatus(Status,
                                                          CalculateCountdown(battleManager,
                                                                             userType,
                                                                             userIndex,
                                                                             userType,
                                                                             userIndex),
                                                          userType,
                                                          userIndex,
                                                          userType,
                                                          userIndex,
                                                          false);
        }

        /// <summary>
        /// Calculate the countdown 
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="targetIndex">Index of the target.</param>
        private int CalculateCountdown(BattleManager battleManager,
                                       BattlerType userType,
                                       int userIndex,
                                       BattlerType targetType,
                                       int targetIndex) =>
            InfiniteDuration
                ? -1
                : Status.CalculateRandomCountdown(battleManager, userType, userIndex, targetType, targetIndex);
    }
}