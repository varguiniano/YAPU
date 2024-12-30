using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Volatile status for the move Imprison.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/Imprison", fileName = "Imprisoning")]
    public class Imprisoning : VolatileStatus
    {
        /// <summary>
        /// Callback for when this status is added.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="extraData">Extra data this status may need.</param>
        public override IEnumerator OnAddStatus(BattleManager battleManager, Battler battler, params object[] extraData)
        {
            yield break;
        }

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
            yield break;
        }

        /// <summary>
        /// Callback when retrieving the list of moves a monster can use.
        /// </summary>
        /// <param name="owner">Owner of the status.</param>
        /// <param name="other">Reference to the other battler.</param>
        /// <param name="usableMoves">The previous list of usable moves.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The new list of usable moves.</returns>
        public override List<MoveSlot> OnRetrieveUsableMovesForOtherBattler(Battler owner,
                                                                            Battler other,
                                                                            List<MoveSlot> usableMoves,
                                                                            BattleManager battleManager) =>
            // Affect only opponents.
            battleManager.Battlers.GetTypeAndIndexOfBattler(owner).Type
         == battleManager.Battlers.GetTypeAndIndexOfBattler(other).Type
                ? usableMoves
                : usableMoves.Where(slot => owner.CurrentMoves.All(ownerSlot => ownerSlot.Move != slot.Move)).ToList();

        /// <summary>
        /// Callback for when another battler is about to use a move.
        /// </summary>
        /// <param name="owner">Owner of the status.</param>
        /// <param name="user">Reference to the user of the move.</param>
        /// <param name="move">Move they will use.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="targets">Targets of the move. Can be modified.</param>
        /// <param name="hasBeenReflected"></param>
        /// <param name="finished">Callback stating if the move will still be used, if the targets are modified and the new targets for the move.</param>
        public override IEnumerator OnOtherBattlerAboutToUseAMove(Battler owner,
                                                                  Battler user,
                                                                  Move move,
                                                                  BattleManager battleManager,
                                                                  List<(BattlerType Type, int Index)> targets,
                                                                  bool hasBeenReflected,
                                                                  Action<bool, bool,
                                                                      List<(BattlerType Type, int Index)>> finished)
        {
            // Affect only opponents.
            finished.Invoke(battleManager.Battlers.GetTypeAndIndexOfBattler(owner).Type
                         == battleManager.Battlers.GetTypeAndIndexOfBattler(user).Type
                         || owner.CurrentMoves.All(slot => slot.Move != move),
                            false,
                            targets);

            yield break;
        }
    }
}