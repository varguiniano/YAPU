using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// SuperLuck ability implementation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/SuperLuck", fileName = "SuperLuck")]
    public class SuperLuck : Ability
    {
        /// <summary>
        /// Called when calculating the critical chance of this battler.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="multiplier">Multiplier to apply to the chance.</param>
        /// <param name="modifier">Critical stage modifier to use.</param>
        /// <param name="alwaysHit">Change it to always hit?</param>
        /// <returns>Has the chance been changed?</returns>
        public override bool OnCalculateCriticalChance(Battler owner,
                                                       Battler target,
                                                       BattleManager battleManager,
                                                       Move move,
                                                       ref float multiplier,
                                                       ref byte modifier,
                                                       ref bool alwaysHit)
        {
            ShowAbilityNotification(owner);

            modifier++;
            return true;
        }
    }
}