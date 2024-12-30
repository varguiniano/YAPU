using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Rivalry ability implementation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Rivalry", fileName = "Rivalry")]
    public class Rivalry : Ability
    {
        /// <summary>
        /// Multiplier to apply to the power when user and target match gender.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private float SameGenderMultiplier = 1.25f;

        /// <summary>
        /// Multiplier to apply to the power when user and target have opposite gender.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private float OppositeGenderMultiplier = .75f;

        /// <summary>
        /// Get the power of a move that this battler is going to use.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Target of the move, if exists.</param>
        /// <param name="ignoresAbilities"></param>
        /// <returns>A multiplier to apply to the power.</returns>
        public override float GetMovePowerMultiplierWhenUsingMove(BattleManager battleManager,
                                                                  Move move,
                                                                  Battler user,
                                                                  Battler target,
                                                                  bool ignoresAbilities)
        {
            float multiplier =
                base.GetMovePowerMultiplierWhenUsingMove(battleManager, move, user, null, ignoresAbilities);

            if (target == null
             || user.PhysicalData.Gender == MonsterGender.NonBinary
             || target.PhysicalData.Gender == MonsterGender.NonBinary)
                return multiplier;

            if (user.PhysicalData.Gender == target.PhysicalData.Gender) return SameGenderMultiplier * multiplier;

            return OppositeGenderMultiplier * multiplier;
        }
    }
}