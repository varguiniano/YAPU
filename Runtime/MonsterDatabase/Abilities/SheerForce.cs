using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability SheerForce.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/SheerForce", fileName = "SheerForce")]
    public class SheerForce : Ability
    {
        /// <summary>
        /// Moves that are affected by this ability.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMoves))]
        #endif
        private List<Move> AffectedMoves;

        /// <summary>
        /// Multiplier to apply to the move's power.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private float PowerMultiplier = 1.3f;

        /// <summary>
        /// Get the power of a move that this battler is going to use.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Target of the move, if exists.</param>
        /// <param name="ignoresAbilities"></param>
        /// <returns>A multiplier to apply to the power.</returns>
        public override float
            GetMovePowerMultiplierWhenUsingMove(BattleManager battleManager,
                                                Move move,
                                                Battler user,
                                                Battler target,
                                                bool ignoresAbilities)
        {
            float multiplier = base.GetMovePowerMultiplierWhenUsingMove(battleManager, move, user, target, ignoresAbilities);

            if (!AffectedMoves.Contains(move)) return multiplier;

            multiplier *= PowerMultiplier;

            return multiplier;
        }

        /// <summary>
        /// Called to check if the battler can perform the secondary effect of a move when using it.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="battleManager"></param>
        /// <returns>True if it can.</returns>
        public override bool CanPerformSecondaryEffectOfMove(Battler owner,
                                                             List<(BattlerType Type, int Index)> targets,
                                                             Move move,
                                                             BattleManager battleManager)
        {
            if (!AffectedMoves.Contains(move))
                return base.CanPerformSecondaryEffectOfMove(owner, targets, move, battleManager);

            ShowAbilityNotification(owner);

            return false;
        }
    }
}