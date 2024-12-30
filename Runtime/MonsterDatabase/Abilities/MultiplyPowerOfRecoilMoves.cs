using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Base class for abilities that modify the power of recoil moves.
    /// </summary>
    public abstract class MultiplyPowerOfRecoilMoves : Ability
    {
        /// <summary>
        /// Multiplier to apply on recoil moves.
        /// </summary>
        [SerializeField]
        private float Multiplier = 1.2f;

        /// <summary>
        /// Include the multiplier when it's a recoil move.
        /// </summary>
        public override float GetMovePowerMultiplierWhenUsingMove(BattleManager battleManager,
                                                                  Move move,
                                                                  Battler user,
                                                                  Battler target,
                                                                  bool ignoresAbilities) =>
            (move is DamageWithRecoilMove
                 ? Multiplier
                 : 1)
          * base.GetMovePowerMultiplierWhenUsingMove(battleManager, move, user, target, ignoresAbilities);
    }
}