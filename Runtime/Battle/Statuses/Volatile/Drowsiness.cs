using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Data class for the Drowsiness volatile status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/Drowsiness", fileName = "Drowsiness")]
    public class Drowsiness : VolatileStatus
    {
        /// <summary>
        /// Reference to the sleep status.
        /// </summary>
        [SerializeField]
        private Sleep Sleep;

        /// <summary>
        /// Callback for when this status is removed.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="playAnimation">Play the remove animation?</param>
        public override IEnumerator OnRemoveStatus(BattleManager battleManager,
                                                   Battler battler,
                                                   bool playAnimation = true)
        {
            bool canSleep = true;

            (BattlerType battlerType, int battlerIndex) =
                battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            yield return battler.CanAddStatus(Sleep,
                                              battleManager,
                                              battlerType,
                                              battlerIndex,
                                              false,
                                              true,
                                              can => canSleep = can);

            if (!canSleep) yield break;

            yield return battleManager.Statuses.AddStatus(Sleep,
                                                          battler,
                                                          battlerType,
                                                          battlerIndex);
        }
    }
}