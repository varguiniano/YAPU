using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Levitate.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Levitate", fileName = "Levitate")]
    public class Levitate : Ability
    {
        /// <summary>
        /// Does this ability ground the monster?
        /// </summary>
        /// <param name="battler">Battler to check.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showNotificationIfPrevented"></param>
        /// <returns>True if this ability forces the monster to be grounded and true if this ability prevents grounding.</returns>
        public override (bool, bool) IsGrounded(Battler battler,
                                                BattleManager battleManager,
                                                bool showNotificationIfPrevented)
        {
            if (showNotificationIfPrevented) ShowAbilityNotification(battler, true);

            return (false, true);
        }
    }
}