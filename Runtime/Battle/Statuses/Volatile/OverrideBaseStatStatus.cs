using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Base class for a status that overrides base stats of a mon.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/OverrideBaseStatStatus",
                     fileName = "OverrideBaseStatStatus")]
    public class OverrideBaseStatStatus : VolatileStatus
    {
        /// <summary>
        /// Overrides to apply to each monster.
        /// </summary>
        private readonly SerializableDictionary<MonsterInstance, SerializableDictionary<Stat, uint>> overrides = new();

        /// <summary>
        /// Callback for when this status is added.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="extraData">Extra data this status may need.</param>
        public override IEnumerator OnAddStatus(BattleManager battleManager, Battler battler, params object[] extraData)
        {
            for (int i = 0; i < extraData.Length; i += 2)
            {
                Stat stat = (Stat) extraData[i];
                uint statOverride = (uint) extraData[i + 1];

                if (!overrides.ContainsKey(battler)) overrides[battler] = new SerializableDictionary<Stat, uint>();

                overrides[battler][stat] = statOverride;
            }

            yield break;
        }

        /// <summary>
        /// Callback for when this status is removed.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="playAnimation">Play the remove animation?</param>
        public override IEnumerator OnRemoveStatus(BattleManager battleManager,
                                                   Battler battler,
                                                   bool playAnimation = true)
        {
            overrides.Remove(battler);

            yield break;
        }

        /// <summary>
        /// Called when the battler has ended.
        /// </summary>
        /// <param name="battler">Battler the status is attached to.</param>
        public override IEnumerator OnBattleEnded(Battler battler)
        {
            overrides.Clear();

            yield break;
        }

        /// <summary>
        /// Override the stat.
        /// </summary>
        public override float OnCalculateStat(Battler monster, Stat stat, out uint overrideBaseValue)
        {
            float multiplier = base.OnCalculateStat(monster, stat, out overrideBaseValue);
            overrideBaseValue = 0;

            if (overrides.ContainsKey(monster) && overrides[monster].ContainsKey(stat))
                overrideBaseValue = overrides[monster][stat];

            return multiplier;
        }
    }
}