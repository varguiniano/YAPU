using UnityEngine;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCalculatePriceMoney
{
    /// <summary>
    /// Data class for item effects that modify the amount of price money won in battles.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/OnCalculatePriceMoney/SimpleMultiplierOnCalculatePriceMoney",
                     fileName = "MultiplyPriceMoney")]
    public class OnCalculatePriceMoneyItemEffect : MonsterDatabaseScriptable<OnCalculatePriceMoneyItemEffect>
    {
        /// <summary>
        /// Multiplier to modify the price money with.
        /// </summary>
        [SerializeField]
        private float Multiplier = 1f;

        /// <summary>
        /// Get a multiplier for the price money.
        /// </summary>
        /// <returns></returns>
        public float GetMultiplierForPriceMoney() => Multiplier;
    }
}