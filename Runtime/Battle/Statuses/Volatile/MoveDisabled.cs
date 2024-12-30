using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Volatile status for the move Disable.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/MoveDisabled", fileName = "MoveDisabled")]
    public class MoveDisabled : VolatileStatus
    {
        /// <summary>
        /// Dictionary of battlers and the move they have disabled.
        /// </summary>
        private readonly Dictionary<Battler, Move> disabledMoves = new();

        /// <summary>
        /// Show a dialog when added?
        /// </summary>
        [SerializeField]
        [FoldoutGroup("Configuration")]
        private bool ShowAddDialog = true;

        /// <summary>
        /// Callback for when this status is added.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="extraData">Extra data this status may need.</param>
        public override IEnumerator OnAddStatus(BattleManager battleManager, Battler battler, params object[] extraData)
        {
            Move move = (Move) extraData[0];

            disabledMoves[battler] = move;

            if (ShowAddDialog)
                yield return DialogManager.ShowDialogAndWait(LocalizableStatusStartKey,
                                                             localizableModifiers: false,
                                                             modifiers: new[]
                                                                        {
                                                                            battler.GetNameOrNickName(battleManager
                                                                               .Localizer),
                                                                            battleManager.Localizer
                                                                                [move.LocalizableName]
                                                                        },
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);
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
            disabledMoves.Remove(battler);

            yield break;
        }

        /// <summary>
        /// Called when the battler has ended.
        /// </summary>
        /// <param name="battler">Battler the status is attached to.</param>
        public override IEnumerator OnBattleEnded(Battler battler)
        {
            yield return base.OnBattleEnded(battler);

            disabledMoves.Clear();
        }

        /// <summary>
        /// Callback when retrieving the list of moves a monster can use.
        /// </summary>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="usableMoves">The previous list of usable moves.</param>
        /// <returns>The new list of usable moves.</returns>
        public override List<MoveSlot> OnRetrieveUsableMoves(Battler battler, List<MoveSlot> usableMoves) =>
            base.OnRetrieveUsableMoves(battler, usableMoves)
                .Where(slot => !disabledMoves.ContainsKey(battler) || slot.Move != disabledMoves[battler])
                .ToList();
    }
}