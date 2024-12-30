using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Data class for the Tormented status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/Tormented", fileName = "Tormented")]
    public class Tormented : VolatileStatus
    {
        /// <summary>
        /// Callback for when this status is removed.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="playAnimation">Play the remove animation?</param>
        public override IEnumerator OnRemoveStatus(BattleManager battleManager, Battler battler, bool playAnimation = true)
        {
            yield break;
        }

        /// <summary>
        /// Callback when retrieving the list of moves a monster can use.
        /// </summary>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="usableMoves">The previous list of usable moves.</param>
        /// <returns>The new list of usable moves.</returns>
        public override List<MoveSlot> OnRetrieveUsableMoves(Battler battler, List<MoveSlot> usableMoves) =>
            base.OnRetrieveUsableMoves(battler, usableMoves)
                .Where(slot => slot.Move != battler.LastPerformedAction.LastMove)
                .ToList();
    }
}