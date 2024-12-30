using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCalculateVolatileStatusEffectDuration
{
    /// <summary>
    /// Data class for an item effect that modifies the duration of a volatile status effect. 
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/OnCalculateVolatileStatusEffectDuration/BasicModifier",
                     fileName = "OnCalculateVolatileStatusEffectDurationItemEffect")]
    public class OnCalculateVolatileStatusEffectDurationItemEffect : MonsterDatabaseScriptable<
        OnCalculateVolatileStatusEffectDurationItemEffect>
    {
        /// <summary>
        /// Durations for specific weathers.
        /// </summary>
        [SerializeField]
        private SerializableDictionary<VolatileStatus, int> CustomDurations;

        /// <summary>
        /// Calculate the random countdown of a volatile status to inflict on a target.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="status">Status to inflict.</param>
        /// <param name="holder">Holder of the item.</param>
        /// <param name="item">Item that has this effect.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="targetIndex">Index of the target.</param>
        /// <returns>The duration to use. -2 if not changed.</returns>
        public int CalculateRandomCountdownOfVolatileStatus(BattleManager battleManager,
                                                            VolatileStatus status,
                                                            Battler holder,
                                                            Item item,
                                                            BattlerType targetType,
                                                            int targetIndex)
        {
            if (!CustomDurations.TryGetValue(status, out int volatileStatus)) return -2;
            item.ShowItemNotification(holder, battleManager.Localizer);
            return volatileStatus;
        }
    }
}