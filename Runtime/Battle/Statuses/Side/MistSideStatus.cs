using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Side
{
    /// <summary>
    /// Class representing the mist battle side status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Side/Mist", fileName = "MistSideStatus")]
    public class MistSideStatus : SideStatus
    {
        /// <summary>
        /// Abilities that are immune to this status.
        /// </summary>
        [FoldoutGroup("Immunities")]
        [SerializeField]
        private List<Ability> ImmuneAbilities;

        /// <summary>
        /// Callback for when a stat stage is about to be changed. Allows the side status to change the modifier.
        /// </summary>
        /// <param name="targetType">Type of battler.</param>
        /// <param name="targetIndex">In battle index.</param>
        /// <param name="stat">Stat to change.</param>
        /// <param name="modifier">Modifier to apply.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="callback">Callback with the new modifier to use.</param>
        public override IEnumerator OnStatChange(BattlerType targetType,
                                                 int targetIndex,
                                                 Stat stat,
                                                 short modifier,
                                                 BattlerType userType,
                                                 int userIndex,
                                                 BattleManager battleManager,
                                                 ILocalizer localizer,
                                                 Action<short> callback)
        {
            yield return OnStageChange(targetType,
                                       targetIndex,
                                       modifier,
                                       userType,
                                       userIndex,
                                       battleManager,
                                       localizer,
                                       callback);
        }

        /// <summary>
        /// Callback for when a stat stage is about to be changed. Allows the side status to change the modifier.
        /// </summary>
        /// <param name="targetType">Type of battler.</param>
        /// <param name="targetIndex">In battle index.</param>
        /// <param name="stat">Stat to change.</param>
        /// <param name="modifier">Modifier to apply.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="callback">Callback with the new modifier to use.</param>
        public override IEnumerator OnStatChange(BattlerType targetType,
                                                 int targetIndex,
                                                 BattleStat stat,
                                                 short modifier,
                                                 BattlerType userType,
                                                 int userIndex,
                                                 BattleManager battleManager,
                                                 ILocalizer localizer,
                                                 Action<short> callback)
        {
            yield return OnStageChange(targetType,
                                       targetIndex,
                                       modifier,
                                       userType,
                                       userIndex,
                                       battleManager,
                                       localizer,
                                       callback);
        }

        /// <summary>
        /// Callback for when the critical stage is about to be changed. Allows the side status to change the modifier.
        /// </summary>
        /// <param name="targetType">Type of battler.</param>
        /// <param name="targetIndex">In battle index.</param>
        /// <param name="modifier">Modifier to apply.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="callback">Callback with the new modifier to use.</param>
        public override IEnumerator OnCriticalStageChange(BattlerType targetType,
                                                          int targetIndex,
                                                          short modifier,
                                                          BattlerType userType,
                                                          int userIndex,
                                                          BattleManager battleManager,
                                                          ILocalizer localizer,
                                                          Action<short> callback)
        {
            yield return OnStageChange(targetType,
                                       targetIndex,
                                       modifier,
                                       userType,
                                       userIndex,
                                       battleManager,
                                       localizer,
                                       callback);
        }

        /// <summary>
        /// Callback for when a stage stage is about to be changed. Allows the side status to change the modifier.
        /// </summary>
        /// <param name="targetType">Type of battler.</param>
        /// <param name="targetIndex">In battle index.</param>
        /// <param name="modifier">Modifier to apply.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">Index of the monster that triggered this change.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="callback">Callback with the new modifier to use.</param>
        private IEnumerator OnStageChange(BattlerType targetType,
                                          int targetIndex,
                                          short modifier,
                                          BattlerType userType,
                                          int userIndex,
                                          BattleManager battleManager,
                                          ILocalizer localizer,
                                          Action<short> callback)
        {
            Battler user = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            if (user.CanUseAbility(battleManager, false) && ImmuneAbilities.Contains(user.GetAbility()))
            {
                user.GetAbility().ShowAbilityNotification(user);
                callback.Invoke(modifier);
                yield break;
            }

            // If they are friends or the modifier is not negative, mist doesn't apply.
            if (targetType == userType || modifier >= 0)
            {
                callback.Invoke(modifier);

                yield break;
            }

            yield return DialogManager.ShowDialogAndWait("Status/Side/MistSideStatus/Effect",
                                                         localizableModifiers: false,
                                                         modifiers: battleManager
                                                                   .Battlers.GetBattlerFromBattleIndex(targetType,
                                                                             targetIndex)
                                                                   .GetNameOrNickName(localizer),
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            callback.Invoke(0);
        }
    }
}