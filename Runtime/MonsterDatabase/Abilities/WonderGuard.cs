using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability WonderGuard.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/WonderGuard", fileName = "WonderGuard")]
    public class WonderGuard : Ability
    {
        /// <summary>
        /// Called to modify the effectiveness after doing the type calculation.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="move">Move used.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showNotifications">Show notifications when calculating?</param>
        /// <param name="effectiveness">Current effectiveness.</param>
        public override void ModifyEffectivenessAfterTypeCalculationWhenTargeted(Battler owner,
            Battler user,
            Move move,
            BattleManager battleManager,
            bool showNotifications,
            ref float effectiveness)
        {
            if (!(effectiveness < 2)) return;
            
            if (showNotifications) ShowAbilityNotification(owner);
            
            effectiveness = 0;
        }
    }
}