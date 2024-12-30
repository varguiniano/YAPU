using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Data class for a volatile status that force the battler to always use the same move.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/ForceUseMove", fileName = "ForceUseMove")]
    public class ForceUseMove : VolatileStatus
    {
        /// <summary>
        /// Dictionary to store which battlers are forced to use which moves.
        /// Since this is a scriptable object, all battlers are pointing to the same ForceUseMove instance.
        /// Therefore we need a dictionary.
        /// </summary>
        private readonly Dictionary<Battler, Move> forcedMoves = new();

        /// <summary>
        /// Register the battler and the move it is forced to use.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Battler it is being added to.</param>
        /// <param name="extraData">The move to lock in the first position of the array.</param>
        public override IEnumerator OnAddStatus(BattleManager battleManager, Battler battler, params object[] extraData)
        {
            Move move = extraData[0] as Move;

            if (move == null)
            {
                Logger.Error("Move not stated in extra data array!");
                yield break;
            }

            // Could we have a situation in which two instances of this status have to be on the same battler at once?
            forcedMoves[battler] = move;

            yield return base.OnAddStatus(battleManager, battler, extraData);
        }

        /// <summary>
        /// Don't show anything on tick.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        public override IEnumerator OnTickStatus(BattleManager battleManager, Battler battler)
        {
            yield break;
        }

        /// <summary>
        /// Clear the battler from the dictionary when the status is removed.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">The battler is being removed from.</param>
        /// <param name="playAnimation">Play the remove animation?</param>
        public override IEnumerator OnRemoveStatus(BattleManager battleManager,
                                                   Battler battler,
                                                   bool playAnimation = true)
        {
            if (forcedMoves.ContainsKey(battler)) forcedMoves.Remove(battler);

            return base.OnRemoveStatus(battleManager, battler, playAnimation);
        }

        /// <summary>
        /// Callback when retrieving the list of moves a monster can use.
        /// </summary>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="usableMoves">The previous list of usable moves.</param>
        /// <returns>The new list of usable moves.</returns>
        public override List<MoveSlot> OnRetrieveUsableMoves(Battler battler, List<MoveSlot> usableMoves) =>
            base.OnRetrieveUsableMoves(battler, usableMoves).Where(slot => slot.Move == forcedMoves[battler]).ToList();
    }
}