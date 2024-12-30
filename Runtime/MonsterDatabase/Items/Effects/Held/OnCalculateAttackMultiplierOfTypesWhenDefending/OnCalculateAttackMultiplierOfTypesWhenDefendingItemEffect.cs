using UnityEngine;
using UnityEngine.Rendering;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCalculateAttackMultiplierOfTypesWhenDefending
{
    /// <summary>
    /// Data class for an item effect that modifies the attack multiplier of two types.
    /// </summary>
    [CreateAssetMenu(menuName =
                         "YAPU/Items/Effects/Held/OnCalculateAttackMultiplierOfTypesWhenDefendingItemEffect/SimpleModifier",
                     fileName = "OnCalculateAttackMultiplierOfTypesWhenDefendingItemEffect")]
    public class OnCalculateAttackMultiplierOfTypesWhenDefendingItemEffect : MonsterDatabaseScriptable<
        OnCalculateAttackMultiplierOfTypesWhenDefendingItemEffect>
    {
        /// <summary>
        /// Dictionary for the type pairs and their overrides.
        /// </summary>
        [SerializeField]
        private SerializedDictionary<MonsterType, SerializedDictionary<MonsterType, float>> Overrides;

        /// <summary>
        /// Calculate the multiplier.
        /// </summary>
        /// <param name="item">Item having this effect.</param>
        /// <param name="monster">Monster being attacked.</param>
        /// <param name="attackerType">Type attacking.</param>
        /// <param name="userType">Type of the defender to calculate the multiplier from.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The multiplier, -1 if unchanged.</returns>
        public float CalculateAttackMultiplierOfTypesWhenDefending(Item item,
                                                                   Battler monster,
                                                                   MonsterType attackerType,
                                                                   MonsterType userType,
                                                                   BattleManager battleManager)
        {
            if (!Overrides.ContainsKey(attackerType) || !Overrides[attackerType].ContainsKey(userType)) return -1;

            item.ShowItemNotification(monster, battleManager.Localizer);
            return Overrides[attackerType][userType];
        }
    }
}