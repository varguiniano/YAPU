using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Base class for abilities that multiply the accuracy of a used move.
    /// </summary>
    public abstract class MultiplyAccuracyOfUsedMove : Ability
    {
        /// <summary>
        /// Multiplier to apply.
        /// </summary>
        [SerializeField]
        private float Multiplier = 1;

        /// <summary>
        /// Get the multiplier to apply to the accuracy when using a move on a target.
        /// </summary>
        public override float GetMoveAccuracyMultiplierWhenUsed(BattleManager battleManager,
                                                                Battler user,
                                                                Battler target,
                                                                Move move,
                                                                bool ignoresAbilities) =>
            Multiplier;
    }
}