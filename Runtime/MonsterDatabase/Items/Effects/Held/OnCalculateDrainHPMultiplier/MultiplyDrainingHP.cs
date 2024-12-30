using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCalculateDrainHPMultiplier
{
    /// <summary>
    /// Data class for a held item effect that multiplies the Hp being drained.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/OnMultiplyHPDrain/MultiplyDrainingHP",
                     fileName = "MultiplyDrainingHP")]
    public class MultiplyDrainingHP : OnCalculateDrainHPMultiplierItemEffect
    {
        /// <summary>
        /// Multiplier to apply.
        /// </summary>
        [SerializeField]
        private float Multiplier = 1;

        /// <summary>
        /// Calculate the multiplier to use when draining HP.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="item">Item owner of this effect.</param>
        /// <param name="user">Battler draining.</param>
        /// <param name="target">Target to drain from, can be null.</param>
        /// <returns>The multiplier to apply.</returns>
        public override float CalculateDrainHPMultiplier(BattleManager battleManager,
                                                         Item item,
                                                         Battler user,
                                                         Battler target)
        {
            item.ShowItemNotification(user, battleManager.Localizer);
            return Multiplier;
        }
    }
}