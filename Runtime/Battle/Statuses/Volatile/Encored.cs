using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Volatile status for the move Encore.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/Encore", fileName = "EncoredStatus")]
    public class Encored : VolatileStatus
    {
        /// <summary>
        /// Dictionary of battlers and the move they have encored.
        /// </summary>
        private readonly Dictionary<Battler, Move> encoredMoves = new();

        /// <summary>
        /// Callback for when this status is added.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="extraData">Extra data this status may need.</param>
        public override IEnumerator OnAddStatus(BattleManager battleManager, Battler battler, params object[] extraData)
        {
            Move move = (Move) extraData[0];

            encoredMoves[battler] = move;

            yield return DialogManager.ShowDialogAndWait(LocalizableStatusStartKey,
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        battler.GetNameOrNickName(battleManager
                                                                           .Localizer),
                                                                        battleManager.Localizer[move.LocalizableName]
                                                                    },
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);
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
            yield return base.OnRemoveStatus(battleManager, battler);

            encoredMoves.Remove(battler);
        }

        /// <summary>
        /// Called when the battler has ended.
        /// </summary>
        /// <param name="battler">Battler the status is attached to.</param>
        public override IEnumerator OnBattleEnded(Battler battler)
        {
            yield return base.OnBattleEnded(battler);

            encoredMoves.Clear();
        }

        /// <summary>
        /// Callback when retrieving the list of moves a monster can use.
        /// </summary>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="usableMoves">The previous list of usable moves.</param>
        /// <returns>The new list of usable moves.</returns>
        public override List<MoveSlot> OnRetrieveUsableMoves(Battler battler, List<MoveSlot> usableMoves) =>
            base.OnRetrieveUsableMoves(battler, usableMoves)
                .Where(slot => !encoredMoves.ContainsKey(battler) || slot.Move == encoredMoves[battler])
                .ToList();
    }
}