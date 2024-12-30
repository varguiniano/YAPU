using System.Collections;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Volatile status for the move SmackDown.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/SmackDown", fileName = "SmackDown")]
    public class SmackedDown : VolatileStatus
    {
        /// <summary>
        /// Does this status ground the monster?
        /// </summary>
        /// <param name="battler">Battler to check.</param>
        /// <returns>True if this status forces the monster to be grounded and true if this status prevents grounding.</returns>
        public override (bool, bool) IsGrounded(Battler battler) => (true, false);

        /// <summary>
        /// No message.
        /// </summary>
        public override IEnumerator OnAddStatus(BattleManager battleManager, Battler battler, params object[] extraData)
        {
            yield break;
        }

        /// <summary>
        /// No message.
        /// </summary>
        public override IEnumerator OnRemoveStatus(BattleManager battleManager,
                                                   Battler battler,
                                                   bool playAnimation = true)
        {
            yield break;
        }
    }
}