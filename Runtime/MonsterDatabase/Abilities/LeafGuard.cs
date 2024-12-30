using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability LeafGuard.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/LeafGuard", fileName = "LeafGuard")]
    public class LeafGuard : Ability
    {
        /// <summary>
        /// Weathers that make the ability work.
        /// </summary>
        [SerializeField]
        private List<Weather> CompatibleWeathers;

        /// <summary>
        /// Volatile statuses that the ability is immune to.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllVolatileStatuses))]
        #endif
        [SerializeField]
        private List<VolatileStatus> VolatilesImmuneTo;

        /// <summary>
        /// Prevent status.
        /// </summary>
        public override IEnumerator CanAddStatus(Status status,
                                                 BattlerType targetType,
                                                 int targetIndex,
                                                 BattleManager battleManager,
                                                 BattlerType userType,
                                                 int userIndex,
                                                 Action<bool> callback)
        {
            if (!battleManager.Scenario.GetWeather(out Weather weather)
             || !CompatibleWeathers.Contains(weather)
             || userType == targetType)
            {
                callback.Invoke(true);
                yield break;
            }

            ShowAbilityNotification(battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex), true);

            callback.Invoke(false);
        }

        /// <summary>
        /// Prevent volatile.
        /// </summary>
        public override IEnumerator CanAddStatus(VolatileStatus status,
                                                 BattlerType targetType,
                                                 int targetIndex,
                                                 BattleManager battleManager,
                                                 BattlerType userType,
                                                 int userIndex,
                                                 Action<bool> callback)
        {
            if (!battleManager.Scenario.GetWeather(out Weather weather)
             || !CompatibleWeathers.Contains(weather)
             || userType == targetType
             || !VolatilesImmuneTo.Contains(status))
            {
                callback.Invoke(true);
                yield break;
            }

            ShowAbilityNotification(battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex), true);

            callback.Invoke(false);
        }
    }
}