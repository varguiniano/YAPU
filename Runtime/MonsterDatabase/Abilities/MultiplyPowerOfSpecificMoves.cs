using UnityEngine;
using UnityEngine.Rendering;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for an ability that modifies the power of specific moves.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/MultiplyPowerOfSpecificMoves",
                     fileName = "MultiplyPowerOfSpecificMoves")]
    public class MultiplyPowerOfSpecificMoves : Ability
    {
        /// <summary>
        /// Multipliers to apply to specific damage moves.
        /// </summary>
        [SerializeField]
        private SerializedDictionary<DamageMove, float> PowerMultipliers;

        /// <summary>
        /// Include the multiplier when it's a contact move.
        /// </summary>
        public override float GetMovePowerMultiplierWhenUsingMove(BattleManager battleManager,
                                                                  Move move,
                                                                  Battler user,
                                                                  Battler target,
                                                                  bool ignoresAbilities) =>
            (move is DamageMove damageMove && PowerMultipliers.ContainsKey(damageMove)
                 ? PowerMultipliers[damageMove]
                 : 1)
          * base.GetMovePowerMultiplierWhenUsingMove(battleManager, move, user, target, ignoresAbilities);
    }
}