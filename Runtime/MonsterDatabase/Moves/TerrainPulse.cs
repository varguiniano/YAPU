using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Core.Runtime.DataStructures;
using Terrain = Varguiniano.YAPU.Runtime.Battle.Statuses.Terrain.Terrain;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move TerrainPulse.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/TerrainPulse", fileName = "TerrainPulse")]
    public class TerrainPulse : DamageMove
    {
        /// <summary>
        /// Types of this move in specific weathers.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializableDictionary<Terrain, MonsterType> TerrainTypeOverrides;

        /// <summary>
        /// Get the type of this move in battle.
        /// </summary>
        /// <param name="battler">Owner of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The type of the move.</returns>
        public override MonsterType GetMoveTypeInBattle(Battler battler, BattleManager battleManager) =>
            TerrainTypeOverrides.ContainsKey(battleManager.Scenario.Terrain)
         && battleManager.Scenario.Terrain.IsAffected(battler, battleManager)
                ? TerrainTypeOverrides[battleManager.Scenario.Terrain]
                : base.GetMoveTypeInBattle(battler, battleManager);

        /// <summary>
        /// Get the move's power.
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
            (TerrainTypeOverrides.ContainsKey(battleManager.Scenario.Terrain)
          && battleManager.Scenario.Terrain.IsAffected(user, battleManager)
                 ? 2
                 : 1)
          * base.GetMovePowerInBattle(battleManager, user, target, ignoresAbilities, hitNumber);
    }
}