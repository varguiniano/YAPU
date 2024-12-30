using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Terrain = Varguiniano.YAPU.Runtime.Battle.Statuses.Terrain.Terrain;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move RisingVoltage.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Electric/RisingVoltage", fileName = "RisingVoltage")]
    public class RisingVoltage : DamageMove
    {
        /// <summary>
        /// Reference to the electric terrain.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllTerrains))]
        #endif
        private Terrain ElectricTerrain;

        /// <summary>
        /// Get the move's power.
        /// Multiply by an additional 2 on Electric Terrain.
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
            Mathf.FloorToInt((battleManager.Scenario.Terrain == ElectricTerrain
                           && target != null
                           && ElectricTerrain.IsAffected(target, battleManager)
                                  ? 2f
                                  : 1)
                           * base.GetMovePowerInBattle(battleManager, user, target, ignoresAbilities, hitNumber));
    }
}