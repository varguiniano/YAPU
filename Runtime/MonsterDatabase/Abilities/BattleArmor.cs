using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability BattleArmor.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/BattleArmor", fileName = "BattleArmor")]
    public class BattleArmor : Ability
    {
        /// <summary>
        /// Called when calculating the critical chance of this battler.
        /// </summary>
        /// <param name="owner">Target of the move.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="alwaysHit">Change it to always hit?</param>
        /// <returns>Multiplier to apply to the chance.</returns>
        public override float OnCalculateCriticalChanceWhenTargeted(Battler owner,
                                                                    Battler user,
                                                                    BattleManager battleManager,
                                                                    Move move,
                                                                    out bool alwaysHit)
        {
            alwaysHit = false;

            return AffectsUserOfEffect(user, owner, IgnoresOtherAbilities(battleManager, owner, null), battleManager)
                       ? 0
                       : 1;
        }
    }
}