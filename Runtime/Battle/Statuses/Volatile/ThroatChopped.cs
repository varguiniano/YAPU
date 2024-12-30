using System;
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
    /// Data class for the ThroatChopped status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/ThroatChopped", fileName = "ThroatChopped")]
    public class ThroatChopped : VolatileStatus
    {
        /// <summary>
        /// Callback for when the battler is about to use a move.
        /// </summary>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="move">Move they will use.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="finished">Callback stating if the move will still be used.</param>
        public override IEnumerator OnAboutToUseAMove(Battler battler,
                                                      Move move,
                                                      BattleManager battleManager,
                                                      List<(BattlerType Type, int Index)> targets,
                                                      Action<bool> finished)
        {
            bool use = true;

            yield return base.OnAboutToUseAMove(battler, move, battleManager, targets, shouldUse => use &= shouldUse);

            if (move.SoundBased)
                yield return DialogManager.ShowDialogAndWait("Status/Volatile/ThroatChopped/Start",
                                                             switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed,
                                                             localizableModifiers: false,
                                                             modifiers: battler.GetNameOrNickName(battleManager
                                                                .Localizer));

            finished.Invoke(use && !move.SoundBased);
        }

        /// <summary>
        /// Callback when retrieving the list of moves a monster can use.
        /// </summary>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="usableMoves">The previous list of usable moves.</param>
        /// <returns>The new list of usable moves.</returns>
        public override List<MoveSlot> OnRetrieveUsableMoves(Battler battler, List<MoveSlot> usableMoves) =>
            base.OnRetrieveUsableMoves(battler, usableMoves).Where(slot => !slot.Move.SoundBased).ToList();
    }
}