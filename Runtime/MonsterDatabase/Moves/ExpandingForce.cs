using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Terrain = Varguiniano.YAPU.Runtime.Battle.Statuses.Terrain.Terrain;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move ExpandingForce.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Psychic/ExpandingForce", fileName = "ExpandingForce")]
    public class ExpandingForce : DamageMove
    {
        /// <summary>
        /// Reference to the psychic terrain.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllTerrains))]
        #endif
        private Terrain PsychicTerrain;

        /// <summary>
        /// Called when the final targets are about to be selected.
        /// This allows the move to reselect different targets on certain conditions.
        /// Switch to attack all adjacent opponents if the terrain is active.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">User type.</param>
        /// <param name="userIndex">User index.</param>
        /// <param name="targets">Current preselected targets.</param>
        internal override List<(BattlerType Type, int Index)> SelectFinalTargets(BattleManager battleManager,
                                                                                 BattlerType userType,
                                                                                 int userIndex,
                                                                                 List<(BattlerType Type, int Index)> targets)
        {
            targets = base.SelectFinalTargets(battleManager, userType, userIndex, targets);

            if (!battleManager.Scenario.Terrain == PsychicTerrain) return targets;

            PossibleTargets originalPossibleTargets = MovePossibleTargets;
            bool couldMultiTargetOriginally = CanHaveMultipleTargets;

            MovePossibleTargets = PossibleTargets.AdjacentEnemies;
            CanHaveMultipleTargets = true;

            List<(BattlerType Type, int Index)> validTargets =
                MoveUtils.GenerateValidTargetsForMove(battleManager, userType, userIndex, this, StaticLogger)
                         .Select(target => battleManager.Battlers.GetTypeAndIndexOfBattler(target))
                         .ToList();

            MovePossibleTargets = originalPossibleTargets;
            CanHaveMultipleTargets = couldMultiTargetOriginally;

            return validTargets;
        }

        /// <summary>
        /// Get the move's power.
        /// Multiply by an additional 1.5 on Psychic Terrain.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="hitNumber"></param>
        /// <returns>The move's power.</returns>
        public override int GetMovePowerInBattle(BattleManager battleManager,
                                                 Battler user,
                                                 Battler target,
                                                 bool ignoresAbilities,
                                                 int hitNumber = 0) =>
            Mathf.FloorToInt((battleManager.Scenario.Terrain == PsychicTerrain
                           && PsychicTerrain.IsAffected(user, battleManager)
                                  ? 1.5f
                                  : 1)
                           * base.GetMovePowerInBattle(battleManager, user, target, ignoresAbilities, hitNumber));
    }
}