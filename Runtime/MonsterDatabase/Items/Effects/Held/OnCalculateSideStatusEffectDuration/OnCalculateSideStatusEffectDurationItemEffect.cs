using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Side;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCalculateSideStatusEffectDuration
{
    /// <summary>
    /// Data class for an item effect that modifies the duration of a side status effect. 
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/OnCalculateSideStatusEffectDuration/BasicModifier",
                     fileName = "OnCalculateSideStatusEffectDurationItemEffect")]
    public class
        OnCalculateSideStatusEffectDurationItemEffect : MonsterDatabaseScriptable<
            OnCalculateSideStatusEffectDurationItemEffect>
    {
        /// <summary>
        /// Durations for specific weathers.
        /// </summary>
        [SerializeField]
        private SerializableDictionary<SideStatus, int> CustomDurations;

        /// <summary>
        /// Calculate the duration of a new side status effect created by the holder.
        /// </summary>
        /// <param name="statusToAdd">Status to add.</param>
        /// <param name="side">Side to add it on.</param>
        /// <param name="inBattleIndex">In battle index of the affected roster. Only used for dialogs.</param>
        /// <param name="holder">Holder of the item and the one setting the status.</param>
        /// <param name="item">Owner of this effect.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The duration to use. -2 if not changed.</returns>
        public int CalculateSideStatusDuration(SideStatus statusToAdd,
                                               BattlerType side,
                                               int inBattleIndex,
                                               Battler holder,
                                               Item item,
                                               BattleManager battleManager)
        {
            if (!CustomDurations.TryGetValue(statusToAdd, out int duration)) return -2;

            item.ShowItemNotification(holder, battleManager.Localizer);
            return duration;
        }
    }
}