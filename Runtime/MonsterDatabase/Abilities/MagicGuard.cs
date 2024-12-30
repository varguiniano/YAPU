using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// MagicGuard ability implementation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/MagicGuard", fileName = "MagicGuard")]
    public class MagicGuard : Ability
    {
        /// <summary>
        /// Is the monster affected by secondary damage effects?
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="user">Owner of the effect.</param>
        /// <param name="damageDealt">Damage that the move dealt.</param>
        /// <param name="battleManager"></param>
        /// <returns>True if it is affected.</returns>
        public override bool IsAffectedBySecondaryDamageEffects(Battler owner,
                                                                Battler user,
                                                                int damageDealt,
                                                                BattleManager battleManager)
        {
            ShowAbilityNotification(owner);

            return false;
        }
    }
}