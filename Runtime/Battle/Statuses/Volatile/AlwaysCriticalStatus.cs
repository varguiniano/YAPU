using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Data class representing a status that makes the monster always land critical hits.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/AlwaysCriticalStatus",
                     fileName = "AlwaysCriticalStatus")]
    public class AlwaysCriticalStatus : VolatileStatus
    {
        /// <summary>
        /// No message.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="playAnimation">Play the remove animation?</param>
        public override IEnumerator OnRemoveStatus(BattleManager battleManager, Battler battler, bool playAnimation = true)
        {
            yield break;
        }

        /// <summary>
        /// Called when calculating the critical chance of this battler.
        /// </summary>
        /// <param name="battler">Owner of the ability.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="multiplier">Multiplier to apply to the chance.</param>
        /// <param name="alwaysHit">Change it to always hit?</param>
        /// <returns>Has the chance been changed?</returns>
        public override bool OnCalculateCriticalChance(Battler battler,
                                                       Battler target,
                                                       BattleManager battleManager,
                                                       Move move,
                                                       ref float multiplier,
                                                       ref bool alwaysHit)
        {
            alwaysHit = true;
            return true;
        }
    }
}