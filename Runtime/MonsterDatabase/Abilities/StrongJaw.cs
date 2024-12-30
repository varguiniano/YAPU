using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability StrongJaw.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/StrongJaw", fileName = "StrongJaw")]
    public class StrongJaw : Ability
    {
        /// <summary>
        /// Multiplier to apply on biting moves.
        /// </summary>
        [SerializeField]
        private float Multiplier = 1.5f;

        /// <summary>
        /// Include the multiplier when it's a biting move.
        /// </summary>
        public override float GetMovePowerMultiplierWhenUsingMove(BattleManager battleManager,
                                                                  Move move,
                                                                  Battler user,
                                                                  Battler target,
                                                                  bool ignoresAbilities) =>
            (move is DamageMove {IsBitingMove: true}
                 ? Multiplier
                 : 1)
          * base.GetMovePowerMultiplierWhenUsingMove(battleManager, move, user, target, ignoresAbilities);
    }
}