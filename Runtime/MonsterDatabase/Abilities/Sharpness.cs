using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Sharpness.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Sharpness", fileName = "Sharpness")]
    public class Sharpness : Ability
    {
        /// <summary>
        /// Multiplier to apply on sharp moves.
        /// </summary>
        [SerializeField]
        private float Multiplier = 1.5f;

        /// <summary>
        /// Include the multiplier when it's a slicing move.
        /// </summary>
        public override float GetMovePowerMultiplierWhenUsingMove(BattleManager battleManager,
                                                                  Move move,
                                                                  Battler user,
                                                                  Battler target,
                                                                  bool ignoresAbilities) =>
            (move is DamageMove {IsSlicingMove: true}
                 ? Multiplier
                 : 1)
          * base.GetMovePowerMultiplierWhenUsingMove(battleManager, move, user, target, ignoresAbilities);
    }
}