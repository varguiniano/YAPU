using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for an ability that modifies the power that contact moves have.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/MultiplyPowerOfContactMoves",
                     fileName = "MultiplyPowerOfContactMoves")]
    public class MultiplyPowerOfContactMoves : Ability
    {
        /// <summary>
        /// Multiplier to apply on contact moves.
        /// </summary>
        [SerializeField]
        private float Multiplier = 1;

        /// <summary>
        /// Include the multiplier when it's a contact move.
        /// </summary>
        public override float GetMovePowerMultiplierWhenUsingMove(BattleManager battleManager,
                                                                  Move move,
                                                                  Battler user,
                                                                  Battler target,
                                                                  bool ignoresAbilities) =>
            (move.DoesMoveMakeContact(user, target, battleManager, ignoresAbilities) ? Multiplier : 1)
          * base.GetMovePowerMultiplierWhenUsingMove(battleManager, move, user, target, ignoresAbilities);
    }
}