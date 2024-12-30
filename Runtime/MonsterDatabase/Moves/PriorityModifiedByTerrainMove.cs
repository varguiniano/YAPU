using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using Varguiniano.YAPU.Runtime.Battle;
using Terrain = Varguiniano.YAPU.Runtime.Battle.Statuses.Terrain.Terrain;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Base class for moves which priority is modified by terrains.
    /// </summary>
    public abstract class PriorityModifiedByTerrainMove : DamageMove
    {
        /// <summary>
        /// Priority to be applied by terrain.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializedDictionary<Terrain, int> PriorityByTerrain;

        /// <summary>
        /// Get the move's priority.
        /// </summary>
        /// <param name="owner">Move owner.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="showNotifications">Show notifications for abilities or items that modify the priority.</param>
        /// <returns>The priority modifier.</returns>
        public override int GetPriority(Battler owner,
                                        List<Battler> targets,
                                        BattleManager battleManager,
                                        bool showNotifications = true)
        {
            // ReSharper disable once InvertIf
            if (battleManager.Scenario.Terrain != null)
                foreach (KeyValuePair<Terrain, int> pair in
                         PriorityByTerrain.Where(pair => battleManager.Scenario.Terrain == pair.Key
                                                      && pair.Key.IsAffected(owner, battleManager)))
                    return pair.Value;

            return base.GetPriority(owner, targets, battleManager);
        }
    }
}