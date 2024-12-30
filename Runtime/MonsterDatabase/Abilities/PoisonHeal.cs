using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability PoisonHeal.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/PoisonHeal", fileName = "PoisonHeal")]
    public class PoisonHeal : Ability
    {
        /// <summary>
        /// Invert the multiplier to apply when this mon takes poison damage.
        /// </summary>
        public override float CalculatePoisonDamageMultiplier(Battler owner, BattleManager battleManager)
        {
            ShowAbilityNotification(owner);

            return -1;
        }
    }
}