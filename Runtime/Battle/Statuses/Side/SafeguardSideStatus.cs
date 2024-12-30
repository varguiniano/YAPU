using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Side
{
    /// <summary>
    /// Class representing the Safeguard battle side status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Side/Safeguard", fileName = "SafeguardSideStatus")]
    public class SafeguardSideStatus : SideStatus
    {
        /// <summary>
        /// Status this safeguard grants immunity to.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllStatuses))]
        #endif
        [FoldoutGroup("Effect")]
        [SerializeField]
        private List<Status> StatusImmunities;

        /// <summary>
        /// Status this safeguard grants immunity to.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllVolatileStatuses))]
        #endif
        [FoldoutGroup("Effect")]
        [SerializeField]
        private List<VolatileStatus> VolatileStatusImmunities;

        /// <summary>
        /// Abilities that are immune to this status.
        /// </summary>
        [FoldoutGroup("Immunities")]
        [SerializeField]
        private List<Ability> ImmuneAbilities;

        ///  <summary>
        ///  Check if a status can be added to the monster.
        ///  </summary>
        ///  <param name="status">Status to add.</param>
        ///  <param name="targetType">Type of the battler to add the status to.</param>
        ///  <param name="targetIndex">Index of the battler to add the status to.</param>
        ///  <param name="battleManager">Reference to the battle manager.</param>
        ///  <param name="userType">The type of the monster that triggered this change.</param>
        ///  <param name="userIndex">The index of the monster that triggered this change.</param>
        ///  <param name="callback">Callback telling if it can be added</param>
        public override IEnumerator CanAddStatus(Status status,
                                                 BattlerType targetType,
                                                 int targetIndex,
                                                 BattleManager battleManager,
                                                 BattlerType userType,
                                                 int userIndex,
                                                 Action<bool> callback)
        {
            Battler user = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            if (user.CanUseAbility(battleManager, false) && ImmuneAbilities.Contains(user.GetAbility()))
            {
                user.GetAbility().ShowAbilityNotification(user);
                callback.Invoke(true);
                yield break;
            }

            if (targetType == userType || !StatusImmunities.Contains(status))
            {
                callback.Invoke(true);
                yield break;
            }

            callback.Invoke(false);

            yield return DialogManager.ShowDialogAndWait("Status/Side/SafeguardStatus/Protection",
                                                         localizableModifiers: false,
                                                         modifiers: battleManager.Battlers
                                                            .GetBattlerFromBattleIndex(targetType, targetIndex)
                                                            .GetNameOrNickName(battleManager.Localizer),
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);
        }

        /// <summary>
        /// Check if a status can be added to the monster.
        /// </summary>
        /// <param name="status">Status to add.</param>
        /// <param name="targetType">Type of the battler to add the status to.</param>
        /// <param name="targetIndex">Index of the battler to add the status to.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="callback">Callback telling if it can be added</param>
        public override IEnumerator CanAddStatus(VolatileStatus status,
                                                 BattlerType targetType,
                                                 int targetIndex,
                                                 BattleManager battleManager,
                                                 BattlerType userType,
                                                 int userIndex,
                                                 Action<bool> callback)
        {
            Battler user = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            if (user.CanUseAbility(battleManager, false) && ImmuneAbilities.Contains(user.GetAbility()))
            {
                callback.Invoke(true);
                yield break;
            }

            if (targetType == userType || !VolatileStatusImmunities.Contains(status))
            {
                callback.Invoke(true);
                yield break;
            }

            callback.Invoke(false);

            yield return DialogManager.ShowDialogAndWait("Status/Side/SafeguardStatus/Protection",
                                                         localizableModifiers: false,
                                                         modifiers: battleManager.Battlers
                                                            .GetBattlerFromBattleIndex(targetType, targetIndex)
                                                            .GetNameOrNickName(battleManager.Localizer),
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);
        }
    }
}