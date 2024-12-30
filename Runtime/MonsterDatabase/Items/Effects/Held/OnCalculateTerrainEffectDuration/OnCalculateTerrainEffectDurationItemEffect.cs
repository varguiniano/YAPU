using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Core.Runtime.DataStructures;
using Terrain = Varguiniano.YAPU.Runtime.Battle.Statuses.Terrain.Terrain;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCalculateTerrainEffectDuration
{
    /// <summary>
    /// Data class for an item effect that modifies the duration of a terrain effect. 
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/OnCalculateTerrainEffectDuration/BasicModifier",
                     fileName = "OnCalculateTerrainEffectDuration")]
    public class
        OnCalculateTerrainEffectDurationItemEffect : MonsterDatabaseScriptable<
            OnCalculateTerrainEffectDurationItemEffect>
    {
        /// <summary>
        /// Durations for specific terrains.
        /// </summary>
        [SerializeField]
        private SerializableDictionary<Terrain, int> CustomDurations;

        /// <summary>
        /// Calculate the terrain duration of the given terrain.
        /// -2 if not modified.
        /// </summary>
        /// <param name="item">Item that has this effect.</param>
        /// <param name="user">Battler setting the terrain.</param>
        /// <param name="terrain">terrain.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The duration to have.</returns>
        public int CalculateTerrainDuration(Item item, Battler user, Terrain terrain, BattleManager battleManager)
        {
            if (!CustomDurations.TryGetValue(terrain, out int duration)) return -2;

            item.ShowItemNotification(user, battleManager.Localizer);
            return duration;
        }
    }
}