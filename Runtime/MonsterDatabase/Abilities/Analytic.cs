using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Analytic.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Analytic", fileName = "Analytic")]
    public class Analytic : Ability
    {
        /// <summary>
        /// Multiplier to apply if the user is the last to act this turn.
        /// </summary>
        [SerializeField]
        private float Multiplier = 1.3f;

        /// <summary>
        /// Apply the multiplier if the user is the last to act this turn.
        /// </summary>
        public override float GetMovePowerMultiplierWhenUsingMove(BattleManager battleManager,
                                                                  Move move,
                                                                  Battler user,
                                                                  Battler target,
                                                                  bool ignoresAbilities)
        {
            float multiplier =
                base.GetMovePowerMultiplierWhenUsingMove(battleManager, move, user, target, ignoresAbilities);

            if (battleManager != null
             && battleManager.CurrentTurnActionOrder is {Count: 0})
                multiplier *= Multiplier;

            return multiplier;
        }
    }
}