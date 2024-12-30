using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// LiquidOoze ability implementation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/LiquidOoze", fileName = "LiquidOoze")]
    public class LiquidOoze : Ability
    {
        /// <summary>
        /// Calculate the multiplier to use this monster's HP is being drained.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="drainer">Monster draining the HP.</param>
        /// <returns>The multiplier to apply.</returns>
        public override float CalculateDrainerDrainHPMultiplier(Battler owner,
                                                                Battler drainer,
                                                                BattleManager battleManager)
        {
            ShowAbilityNotification(owner);

            return -1;
        }
    }
}