using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Terrain = Varguiniano.YAPU.Runtime.Battle.Statuses.Terrain.Terrain;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Base class for abilities that replace the type of a move.
    /// </summary>
    public abstract class MoveTypeReplacingAbility : Ability
    {
        /// <summary>
        /// Dictionary of the types to replace and the type to replace them with.
        /// </summary>
        [SerializeField]
        private SerializedDictionary<MonsterType, MonsterType> TypesToReplace;

        /// <summary>
        /// Power multiplier to apply to replaced moves.
        /// </summary>
        [SerializeField]
        private float BaseReplacementMultiplier = 1.2f;

        /// <summary>
        /// Dictionary of terrains that apply multipliers when the types have been replaced.
        /// </summary>
        [SerializeField]
        private SerializedDictionary<Terrain, float> TerrainMultipliers;

        /// <summary>
        /// Get the type of a move out of battle.
        /// </summary>
        /// <param name="move">Move to calculate.</param>
        /// <param name="monster">Owner of the move.</param>
        /// <param name="currentType">Current type of the move.</param>
        /// <returns>The type of the move.</returns>
        public override MonsterType GetMoveType(Move move, MonsterInstance monster, MonsterType currentType) =>
            TypesToReplace.GetValueOrDefault(currentType, currentType);

        /// <summary>
        /// Get the type of this a in battle.
        /// </summary>
        /// <param name="move">Move to calculate.</param>
        /// <param name="battler">Owner of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="currentType">Current type of the move.</param>
        /// <returns>The type of the move.</returns>
        public override MonsterType GetMoveTypeInBattle(Move move,
                                                        Battler battler,
                                                        BattleManager battleManager,
                                                        MonsterType currentType) =>
            TypesToReplace.GetValueOrDefault(currentType, currentType);

        /// <summary>
        /// Apply multipliers to the power of the move when using it if it was replaced.
        /// </summary>
        public override float GetMovePowerMultiplierWhenUsingMove(BattleManager battleManager,
                                                                  Move move,
                                                                  Battler user,
                                                                  Battler target,
                                                                  bool ignoresAbilities)
        {
            float multiplier = base.GetMovePowerMultiplierWhenUsingMove(battleManager, move, user, target, ignoresAbilities);

            if (!TypesToReplace.ContainsKey(move.GetOriginalMoveType())) return multiplier;

            multiplier *= BaseReplacementMultiplier;

            if (battleManager.Scenario.Terrain != null
             && TerrainMultipliers.TryGetValue(battleManager.Scenario.Terrain, out float terrainMultiplier))
                multiplier *= terrainMultiplier;

            return multiplier;
        }
    }
}