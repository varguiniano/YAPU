using UnityEngine;
using UnityEngine.Rendering;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Unburden.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Unburden", fileName = "Unburden")]
    public class Unburden : Ability
    {
        /// <summary>
        /// Multipliers to apply to stats.
        /// </summary>
        [SerializeField]
        private SerializedDictionary<Stat, float> StatMultipliers;

        /// <summary>
        /// Multiply stats when it has consumed or used an item.
        /// </summary>
        public override float OnCalculateStat(MonsterInstance monster, Stat stat, BattleManager battleManager)
        {
            float multiplier = base.OnCalculateStat(monster, stat, battleManager);

            if (battleManager == null
             || monster is not Battler battler
             || battler.HeldItem != null
             || !battler.ConsumedItemData.HasConsumedItem)
                return multiplier;

            if (StatMultipliers.TryGetValue(stat, out float additionalMultiplier)) multiplier *= additionalMultiplier;

            return multiplier;
        }
    }
}