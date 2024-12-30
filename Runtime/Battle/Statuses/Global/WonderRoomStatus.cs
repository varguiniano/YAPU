using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Global
{
    /// <summary>
    /// Data class for the global status of WonderRoom.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Global/WonderRoom", fileName = "WonderRoomStatus")]
    public class WonderRoomStatus : GlobalStatus
    {
        /// <summary>
        /// Replacements for the stats.
        /// </summary>
        [SerializeField]
        private SerializableDictionary<Stat, Stat> StatReplacements;

        /// <summary>
        /// Called when calculating stats.
        /// Allows the status to rearrange them.
        /// Example: Wonder Room.
        /// </summary>
        public override SerializableDictionary<Stat, Stat> OnCalculateStatReplacement(
            MonsterInstance battler,
            BattleManager battleManager) =>
            StatReplacements;
    }
}