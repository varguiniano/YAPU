using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using Varguiniano.YAPU.Runtime.Battle;
using Terrain = Varguiniano.YAPU.Runtime.Battle.Statuses.Terrain.Terrain;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Base class for moves whose power is affect by the terrain.
    /// I can also change stages of targets.
    /// </summary>
    public abstract class DamageByTerrainMove : StageChanceDamageMove
    {
        /// <summary>
        /// Multiplier to be applied by terrain.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializedDictionary<Terrain, float> MultiplierByTerrain;

        /// <summary>
        /// Multiply the power if one of the terrains is there.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="hitNumber"></param>
        /// <returns>The power of the move.</returns>
        public override int GetMovePowerInBattle(BattleManager battleManager,
                                                 Battler user,
                                                 Battler target,
                                                 bool ignoresAbilities,
                                                 int hitNumber = 0)
        {
            int power = base.GetMovePowerInBattle(battleManager, user, target, ignoresAbilities, hitNumber);

            if (battleManager.Scenario.Terrain != null
             && MultiplierByTerrain.TryGetValue(battleManager.Scenario.Terrain, out float value))
                power = (int) (power * value);

            return power;
        }
    }
}