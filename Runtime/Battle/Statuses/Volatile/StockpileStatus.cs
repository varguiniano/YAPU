using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Data class for the Stockpile Status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/StockpileStatus", fileName = "StockpileStatus")]
    public class StockpileStatus : LayeredVolatileStatus
    {
        /// <summary>
        /// Stats affected by this status.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private List<Stat> AffectedStats;

        /// <summary>
        /// Called when a layer is added to the status.
        /// </summary>
        /// <param name="target">Target of the status.</param>
        /// <param name="user">Mon that added the status.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        protected override IEnumerator OnLayerAdded(Battler target, Battler user, BattleManager battleManager)
        {
            yield return base.OnLayerAdded(target, user, battleManager);

            (BattlerType targetType, int targetIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(target);
            (BattlerType userType, int userIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(user);

            foreach (Stat stat in AffectedStats)
                yield return battleManager.BattlerStats.ChangeStatStage(targetType,
                                                                        targetIndex,
                                                                        stat,
                                                                        1,
                                                                        userType,
                                                                        userIndex);
        }

        /// <summary>
        /// Called when all the layers are removed at once.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="layersRemoved">Number of layers removed.</param>
        protected override IEnumerator OnAllLayersRemoved(BattleManager battleManager,
                                                          Battler battler,
                                                          int layersRemoved)
        {
            yield return base.OnAllLayersRemoved(battleManager, battler, layersRemoved);

            (BattlerType userType, int userIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            foreach (Stat stat in AffectedStats)
                yield return battleManager.BattlerStats.ChangeStatStage(userType,
                                                                        userIndex,
                                                                        stat,
                                                                        (short) -layersRemoved,
                                                                        userType,
                                                                        userIndex);
        }
    }
}