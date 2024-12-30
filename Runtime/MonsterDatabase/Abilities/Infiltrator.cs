using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.World.Encounters;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Infiltrator.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Infiltrator", fileName = "Infiltrator")]
    public class Infiltrator : Ability
    {
        /// <summary>
        /// Moves that won't bypass the substitute.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private List<Move> MovesThatDontBypassSubstitute;

        /// <summary>
        /// Does this ability bypass the substitute?
        /// </summary>
        /// <param name="targetType">Target of the effect.</param>
        /// <param name="targetIndex">Target of the effect.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">User of the effect, owner of the ability.</param>
        /// <param name="userIndex">User of the effect, owner of the ability.</param>
        /// <param name="move">If the effect is a move, reference to it.</param>
        /// <returns>True if it bypasses.</returns>
        public override bool ByPassesSubstitute(BattlerType targetType,
                                                int targetIndex,
                                                BattleManager battleManager,
                                                BattlerType userType,
                                                int userIndex,
                                                Move move = null)
        {
            bool bypass = !MovesThatDontBypassSubstitute.Contains(move);

            if (bypass
             && battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex).Substitute.SubstituteEnabled)
                ShowAbilityNotification(battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex), true);

            return bypass;
        }

        /// <summary>
        /// Called when the encounter chances are calculated and modifies them.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="encounterType">Type of encounter to calculate.</param>
        public override float
            OnCalculateEncounterChance(PlayerCharacter playerCharacter, EncounterType encounterType) =>
            0.125f;
    }
}