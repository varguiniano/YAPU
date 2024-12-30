using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability InnerFocus.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/InnerFocus", fileName = "InnerFocus")]
    public class InnerFocus : Ability
    {
        /// <summary>
        /// Statuses prevented by this ability.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllVolatileStatuses))]
        #endif
        [SerializeField]
        private List<VolatileStatus> PreventedStatuses;

        /// <summary>
        /// Prevent the set statuses.
        /// </summary>
        public override IEnumerator CanAddStatus(VolatileStatus status,
                                                 BattlerType targetType,
                                                 int targetIndex,
                                                 BattleManager battleManager,
                                                 BattlerType userType,
                                                 int userIndex,
                                                 Action<bool> callback)
        {
            bool canAdd = !PreventedStatuses.Contains(status);

            if (!canAdd)
                ShowAbilityNotification(battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex));

            callback.Invoke(canAdd);
            yield break;
        }
    }
}