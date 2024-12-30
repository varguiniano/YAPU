using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability IronFist.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/IronFist", fileName = "IronFist")]
    public class IronFist : Ability
    {
        /// <summary>
        /// Multiplier to apply on punch moves.
        /// </summary>
        [SerializeField]
        private float Multiplier = 1.2f;

        /// <summary>
        /// Include the multiplier when it's a punch move.
        /// </summary>
        public override float GetMovePowerMultiplierWhenUsingMove(BattleManager battleManager,
                                                                  Move move,
                                                                  Battler user,
                                                                  Battler target,
                                                                  bool ignoresAbilities) =>
            (move is DamageMove {IsPunchingMove: true}
                 ? Multiplier
                 : 1)
          * base.GetMovePowerMultiplierWhenUsingMove(battleManager, move, user, target, ignoresAbilities);
    }
}